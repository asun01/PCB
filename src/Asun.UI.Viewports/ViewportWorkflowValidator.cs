namespace Asun.UI.Viewports;

public readonly record struct ViewportWorkflowValidation(
    bool IsValid,
    int CommandCount,
    int ErrorCount,
    IReadOnlyList<string> Errors);

/// <summary>
/// Validates replayable workflow batches before execution. Validation is deliberately
/// structural and deterministic; it does not invent domain measurement semantics.
/// </summary>
public static class ViewportWorkflowValidator
{
    public static ViewportWorkflowValidation Validate(
        IEnumerable<ViewportWorkflowCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        var errors = new List<string>();
        var count = 0;

        foreach (var command in commands)
        {
            count++;

            if (!Enum.IsDefined(command.Operation))
            {
                errors.Add($"Command {count} has an undefined operation.");
                continue;
            }

            switch (command.Operation)
            {
                case ViewportWorkflowOperation.ResizeViewport:
                case ViewportWorkflowOperation.AddRectangle:
                case ViewportWorkflowOperation.AddEllipse:
                    if (!float.IsFinite(command.Vector.X) ||
                        !float.IsFinite(command.Vector.Y) ||
                        command.Vector.X <= 0 ||
                        command.Vector.Y <= 0)
                    {
                        errors.Add($"Command {count} contains invalid positive vector values.");
                    }

                    break;

                case ViewportWorkflowOperation.Zoom:
                    if (!double.IsFinite(command.Value) ||
                        command.Value <= 0 ||
                        !double.IsFinite(command.SecondaryValue))
                    {
                        errors.Add($"Command {count} contains an invalid zoom factor or scale.");
                    }

                    break;

                case ViewportWorkflowOperation.Pan:
                case ViewportWorkflowOperation.PanClamped:
                case ViewportWorkflowOperation.CenterOnImagePoint:
                case ViewportWorkflowOperation.TranslateSelected:
                case ViewportWorkflowOperation.DuplicateSelected:
                    if (!float.IsFinite(command.Vector.X) ||
                        !float.IsFinite(command.Vector.Y))
                    {
                        errors.Add($"Command {count} contains non-finite vector data.");
                    }

                    break;
            }
        }

        return new ViewportWorkflowValidation(
            errors.Count == 0,
            count,
            errors.Count,
            errors);
    }
}
