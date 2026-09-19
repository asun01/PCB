namespace Asun.Domain.Pcb;

public static class PcbBoardDefinitionValidationRuntime
{
    public static IReadOnlyList<string> Validate(PcbBoardDefinition board)
    {
        ArgumentNullException.ThrowIfNull(board);

        var errors = new List<string>();

        if (board.BoardId == Guid.Empty)
            errors.Add("Board id cannot be empty.");

        if (string.IsNullOrWhiteSpace(board.Name))
            errors.Add("Board name cannot be blank.");

        if (!double.IsFinite(board.WidthMm) ||
            !double.IsFinite(board.HeightMm) ||
            board.WidthMm <= 0 ||
            board.HeightMm <= 0)
        {
            errors.Add("Board dimensions must be finite and positive.");
        }

        if (board.LayerCount <= 0)
            errors.Add("Board layer count must be positive.");

        return errors;
    }

    public static bool IsValid(PcbBoardDefinition board) =>
        Validate(board).Count == 0;
}
