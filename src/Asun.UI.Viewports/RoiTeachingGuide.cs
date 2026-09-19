namespace Asun.UI.Viewports;

public enum RoiTeachingAction
{
    PointerDown,
    PointerMove,
    PointerUp,
    Cancel
}

public readonly record struct RoiTeachingStep(
    string Id,
    string Prompt,
    RoiTeachingAction Action,
    RoiInteractionKind? Interaction,
    bool Repeatable)
{
    public RoiTeachingStep(
        string id,
        string prompt,
        RoiTeachingAction action,
        RoiInteractionKind? interaction = null,
        bool repeatable = true)
        : this(
            ValidateText(id, nameof(id)),
            ValidateText(prompt, nameof(prompt)),
            action,
            interaction,
            repeatable)
    {
    }

    private static string ValidateText(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value cannot be empty.", name)
            : value;
}

public readonly record struct RoiTeachingSnapshot(
    bool IsActive,
    bool IsCompleted,
    int CurrentStepIndex,
    RoiTeachingStep? CurrentStep);

/// <summary>
/// UI-neutral teaching sequence state machine. A caller may feed editor events into it
/// and render the current prompt/animation without coupling the guide to any UI toolkit.
/// </summary>
public sealed class RoiTeachingGuide
{
    private readonly object _sync = new();
    private readonly IReadOnlyList<RoiTeachingStep> _steps;

    private int _currentIndex;
    private bool _active;

    public RoiTeachingGuide(IEnumerable<RoiTeachingStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        var copied = steps.ToArray();
        if (copied.Length == 0)
            throw new ArgumentException("At least one teaching step is required.", nameof(steps));

        _steps = Array.AsReadOnly(copied);
    }

    public IReadOnlyList<RoiTeachingStep> Steps => _steps;

    public RoiTeachingSnapshot Snapshot
    {
        get
        {
            lock (_sync)
            {
                return CreateSnapshotUnsafe();
            }
        }
    }

    public void Start(bool restart = false)
    {
        lock (_sync)
        {
            if (restart)
                _currentIndex = 0;

            _active = true;
        }
    }

    public void Stop()
    {
        lock (_sync)
            _active = false;
    }

    public void Reset()
    {
        lock (_sync)
        {
            _currentIndex = 0;
            _active = false;
        }
    }

    public bool Observe(RoiEditorEvent editorEvent)
    {
        lock (_sync)
        {
            if (!_active || _currentIndex >= _steps.Count)
                return false;

            var step = _steps[_currentIndex];
            if (!Matches(step, editorEvent))
                return false;

            _currentIndex++;

            if (_currentIndex >= _steps.Count)
            {
                if (step.Repeatable)
                    _currentIndex = 0;
                else
                    _active = false;
            }

            return true;
        }
    }

    private RoiTeachingSnapshot CreateSnapshotUnsafe()
    {
        var completed = !_active && _currentIndex >= _steps.Count;
        var current =
            _currentIndex < _steps.Count
                ? _steps[_currentIndex]
                : null;

        return new RoiTeachingSnapshot(
            _active,
            completed,
            _currentIndex,
            current);
    }

    private static bool Matches(
        RoiTeachingStep step,
        RoiEditorEvent editorEvent)
    {
        var actionMatches = step.Action switch
        {
            RoiTeachingAction.PointerDown =>
                editorEvent.Kind == RoiEditorEventKind.PointerDown,

            RoiTeachingAction.PointerMove =>
                editorEvent.Kind == RoiEditorEventKind.PointerMove,

            RoiTeachingAction.PointerUp =>
                editorEvent.Kind == RoiEditorEventKind.PointerUp,

            RoiTeachingAction.Cancel =>
                editorEvent.Kind == RoiEditorEventKind.Cancelled,

            _ => false
        };

        if (!actionMatches)
            return false;

        return step.Interaction is null ||
               step.Interaction.Value == editorEvent.Interaction;
    }
}
