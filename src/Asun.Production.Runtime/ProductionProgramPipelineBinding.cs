using Asun.Program.Core;

namespace Asun.Production.Runtime;

public sealed record ProductionProgramPipelineStageBinding(
    Guid ProgramStepId,
    ProgramStepKind ProgramStepKind,
    int ProgramOrder,
    string ProgramName,
    int PipelineOrder,
    string PipelineName);

public sealed record ProductionProgramPipelineBinding(
    Guid ProgramId,
    string PlanFingerprint,
    IReadOnlyList<ProductionProgramPipelineStageBinding> Stages,
    string Fingerprint);