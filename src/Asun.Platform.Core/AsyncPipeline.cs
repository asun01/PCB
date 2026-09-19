namespace Asun.Platform.Core;

/// <summary>
/// A dependency-checked asynchronous DAG executor. It provides graph scheduling
/// only; domain state, retry policy, ownership, and acceptance rules remain
/// outside this infrastructure primitive.
/// </summary>
public sealed class AsyncPipeline<TContext>
{
    private readonly IReadOnlyList<Node> _nodes;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<Node>> _dependents;

    public AsyncPipeline(IEnumerable<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        _nodes = nodes
            .Select(SnapshotNode)
            .ToArray();

        if (_nodes.Count == 0)
        {
            throw new ArgumentException("At least one pipeline node is required.", nameof(nodes));
        }

        ValidateGraph(_nodes);

        var dependents = _nodes.ToDictionary(
            node => node.Id,
            _ => new List<Node>(),
            StringComparer.Ordinal);

        foreach (var node in _nodes)
        {
            foreach (var dependency in node.Dependencies)
                dependents[dependency].Add(node);
        }

        _dependents = dependents.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyList<Node>)pair.Value,
            StringComparer.Ordinal);
    }

    public int NodeCount => _nodes.Count;

    public IReadOnlyList<string> NodeIds =>
        _nodes.Select(node => node.Id).ToArray();

    public IReadOnlyList<string> RootNodeIds =>
        _nodes
            .Where(node => node.Dependencies.Count == 0)
            .Select(node => node.Id)
            .ToArray();

    public IReadOnlyList<string> LeafNodeIds =>
        _nodes
            .Where(node => _dependents[node.Id].Count == 0)
            .Select(node => node.Id)
            .ToArray();

    public bool ContainsNode(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return _nodes.Any(node => string.Equals(node.Id, id, StringComparison.Ordinal));
    }

    public bool TryGetNode(string id, out Node? node)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            node = null;
            return false;
        }

        node = _nodes.FirstOrDefault(
            candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));

        return node is not null;
    }

    public ValueTask ExecuteAsync(
        TContext context,
        CancellationToken cancellationToken = default) =>
        ExecuteCoreAsync(context, cancellationToken, null);

    public async ValueTask<IReadOnlyDictionary<string, TimeSpan>> ExecuteWithMetricsAsync(
        TContext context,
        CancellationToken cancellationToken = default)
    {
        var timings = new Dictionary<string, TimeSpan>(StringComparer.Ordinal);
        await ExecuteCoreAsync(context, cancellationToken, timings);
        return timings;
    }

    private async ValueTask ExecuteCoreAsync(
        TContext context,
        CancellationToken cancellationToken,
        Dictionary<string, TimeSpan>? timings)
    {
        var pendingDependencies = _nodes.ToDictionary(
            node => node.Id,
            node => node.Dependencies.Count,
            StringComparer.Ordinal);

        var ready = _nodes
            .Where(node => pendingDependencies[node.Id] == 0)
            .ToList();

        var completedCount = 0;

        while (ready.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = ready.ToArray();
            ready.Clear();

            var completedNodes = await Task.WhenAll(
                batch.Select(async node =>
                {
                    var start = System.Diagnostics.Stopwatch.GetTimestamp();
                    await node.ExecuteAsync(context, cancellationToken);
                    return (
                        node.Id,
                        Elapsed: System.Diagnostics.Stopwatch.GetElapsedTime(start));
                }));

            if (timings is not null)
            {
                foreach (var result in completedNodes)
                    timings[result.Id] = result.Elapsed;
            }

            completedCount += batch.Length;

            foreach (var node in batch)
            {
                foreach (var dependent in _dependents[node.Id])
                {
                    var remaining = --pendingDependencies[dependent.Id];

                    if (remaining == 0)
                        ready.Add(dependent);
                }
            }
        }

        if (completedCount != _nodes.Count)
            throw new InvalidOperationException("Pipeline graph cannot make progress.");
    }

    private static Node SnapshotNode(Node node)
    {
        ArgumentNullException.ThrowIfNull(node);

        return node with
        {
            Dependencies = node.Dependencies?.ToArray()
                ?? throw new ArgumentNullException(nameof(node.Dependencies))
        };
    }

    public static void Validate(IEnumerable<Node> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        var snapshot = nodes.Select(SnapshotNode).ToArray();

        if (snapshot.Length == 0)
            throw new ArgumentException("At least one pipeline node is required.", nameof(nodes));

        ValidateGraph(snapshot);
    }

    private static void ValidateGraph(IReadOnlyList<Node> nodes)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        foreach (var node in nodes)
        {
            ArgumentNullException.ThrowIfNull(node.Dependencies);

            if (string.IsNullOrWhiteSpace(node.Id))
            {
                throw new ArgumentException("Every pipeline node requires a non-empty identifier.", nameof(nodes));
            }

            if (!ids.Add(node.Id))
            {
                throw new ArgumentException($"Duplicate pipeline node identifier: '{node.Id}'.", nameof(nodes));
            }

            ArgumentNullException.ThrowIfNull(node.ExecuteAsync);
        }

        foreach (var node in nodes)
        {
            var dependencies = new HashSet<string>(StringComparer.Ordinal);

            foreach (var dependency in node.Dependencies)
            {
                if (!dependencies.Add(dependency))
                {
                    throw new ArgumentException(
                        $"Node '{node.Id}' contains duplicate dependency '{dependency}'.",
                        nameof(nodes));
                }
                if (!ids.Contains(dependency))
                {
                    throw new ArgumentException(
                        $"Node '{node.Id}' depends on unknown node '{dependency}'.",
                        nameof(nodes));
                }

                if (string.Equals(node.Id, dependency, StringComparison.Ordinal))
                {
                    throw new ArgumentException(
                        $"Node '{node.Id}' cannot depend on itself.",
                        nameof(nodes));
                }
            }
        }

        var remaining = nodes.ToDictionary(
            node => node.Id,
            node => node.Dependencies.Count,
            StringComparer.Ordinal);

        var dependents = nodes.ToDictionary(
            node => node.Id,
            _ => new List<string>(),
            StringComparer.Ordinal);

        foreach (var node in nodes)
        {
            foreach (var dependency in node.Dependencies)
                dependents[dependency].Add(node.Id);
        }

        var ready = new Queue<string>(
            remaining
                .Where(pair => pair.Value == 0)
                .Select(pair => pair.Key));

        while (ready.TryDequeue(out var root))
        {
            if (!remaining.Remove(root))
                continue;

            foreach (var dependent in dependents[root])
            {
                var unresolved = --remaining[dependent];

                if (unresolved == 0)
                    ready.Enqueue(dependent);
            }
        }

        if (remaining.Count != 0)
            throw new ArgumentException(
                "Pipeline graph contains a dependency cycle.",
                nameof(nodes));
    }

    public sealed record Node(
        string Id,
        IReadOnlyCollection<string> Dependencies,
        Func<TContext, CancellationToken, ValueTask> ExecuteAsync)
    {
        public Node(
            string id,
            Func<TContext, CancellationToken, ValueTask> executeAsync)
            : this(id, Array.Empty<string>(), executeAsync)
        {
            ArgumentNullException.ThrowIfNull(executeAsync);
        }
    }
}
