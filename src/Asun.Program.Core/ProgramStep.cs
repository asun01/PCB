namespace Asun.Program.Core;

public sealed record ProgramStep(
    Guid StepId,
    int Order,
    ProgramStepKind Kind,
    string Name,
    IReadOnlyList<ProgramParameter> Parameters)
{
    public bool IsValid=>
        StepId!=Guid.Empty &&
        Order>0 &&
        Enum.IsDefined(Kind) &&
        !string.IsNullOrWhiteSpace(Name) &&
        Parameters.All(parameter=>parameter is not null && parameter.IsValid);
}
