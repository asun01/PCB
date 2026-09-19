namespace Asun.Program.Core;

public sealed record ProgramExecutionPlan(
    Guid ProgramId,
    Version Version,
    IReadOnlyList<ProgramStep> Steps,
    string Fingerprint);
