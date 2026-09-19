using Asun.Device.Contracts;
using Asun.Platform.Pipeline;

namespace Asun.Production.Runtime;

public sealed record ProductionFrameExecution(
    FrameSequence Sequence,
    string InputFingerprint,
    PipelineExecutionReport PipelineReport);
