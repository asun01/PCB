using System.Numerics;

namespace Asun.UI.Viewports;

public enum RoiKeyboardCommand { Delete, Duplicate, MoveLeft, MoveRight, MoveUp, MoveDown, Escape }

public static class RoiKeyboardCommandRuntime
{
    public static bool Apply(
        RoiDocumentRuntime document,
        RoiKeyboardCommand command,
        float step = 1f)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (!float.IsFinite(step) || step <= 0) throw new ArgumentOutOfRangeException(nameof(step));
        switch (command)
        {
            case RoiKeyboardCommand.Delete: return document.DeleteSelected();
            case RoiKeyboardCommand.Duplicate: return document.DuplicateSelected(new Vector2(step, step)) is not null;
            case RoiKeyboardCommand.MoveLeft: return Move(document, new Vector2(-step, 0));
            case RoiKeyboardCommand.MoveRight: return Move(document, new Vector2(step, 0));
            case RoiKeyboardCommand.MoveUp: return Move(document, new Vector2(0, -step));
            case RoiKeyboardCommand.MoveDown: return Move(document, new Vector2(0, step));
            case RoiKeyboardCommand.Escape: return document.Cancel(new Vector2(0, 0)).EditorEvent.Kind == RoiEditorEventKind.Cancelled;
            default: throw new ArgumentOutOfRangeException(nameof(command));
        }
    }

    private static bool Move(RoiDocumentRuntime document, Vector2 delta) =>
        document.TranslateSelected(delta);
}
