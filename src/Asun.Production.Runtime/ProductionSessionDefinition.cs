using Asun.Device.Contracts;
using Asun.Program.Core;
using Asun.Platform.Pipeline;

namespace Asun.Production.Runtime;

public sealed record ProductionSessionDefinition(
    Guid SessionId,
    ProgramExecutionPlan ProgramPlan,
    PipelineDefinition<CapturedFrame> Pipeline,
    int FrameCount);
