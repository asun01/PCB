namespace Asun.Program.Core;

public sealed record InspectionProgram(
    Guid ProgramId,
    string Name,
    Version Version,
    IReadOnlyList<ProgramStep> Steps);
