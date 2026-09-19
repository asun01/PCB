namespace Asun.UI.Viewports;

public sealed class RoiCommandHistoryRuntime
{
    private readonly object _sync = new();
    private readonly Stack<string> _commands = new();

    public void Push(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) throw new ArgumentException(nameof(command));
        lock (_sync) _commands.Push(command);
    }

    public bool TryPop(out string command)
    {
        lock (_sync)
        {
            if (_commands.Count == 0) { command = string.Empty; return false; }
            command = _commands.Pop();
            return true;
        }
    }

    public IReadOnlyList<string> Snapshot()
    {
        lock (_sync) return _commands.ToArray();
    }
}
