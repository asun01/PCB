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
            .Select(node => node with
            {
                Dependencies = node.Dependencies?.ToArray()
                    ?? throw new ArgumentNullException(nameof(node.Dependencies))
            })
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

        while (remaining.Count > 0)
        {
            var roots = remaining
                .Where(pair => pair.Value == 0)
                .Select(pair => pair.Key)
                .ToArray();

            if (roots.Length == 0)
            {
                throw new ArgumentException("Pipeline graph contains a dependency cycle.", nameof(nodes));
            }

            foreach (var root in roots)
            {
                remaining.Remove(root);

                foreach (var dependent in nodes.Where(
                    node => node.Dependencies.Contains(root, StringComparer.Ordinal)))
                {
                    remaining[dependent.Id]--;
                }
            }
        }
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
