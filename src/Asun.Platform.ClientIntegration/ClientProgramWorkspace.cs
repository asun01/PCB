using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;
using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration;

public enum ClientProgramLoadStatus
{
    Empty,
    Ready,
    Invalid
}

public sealed record ClientProgramWorkspaceSnapshot(
    ClientProgramLoadStatus Status,
    Guid? ProgramId,
    string? Name,
    Version? Version,
    int StepCount,
    string? ExecutionPlanFingerprint,
    IReadOnlyList<string> Errors)
{
    public Guid? SelectedStepId { get; init; }
};

public sealed class ClientProgramWorkspace
{
    private InspectionProgram? _program;
    private ProgramExecutionPlan? _executionPlan;
    private IReadOnlyList<string> _errors=Array.Empty<string>();
    private Guid? _selectedStepId;

    public ClientProgramWorkspaceSnapshot Snapshot
    {
        get
        {
            if(_program is null)
            {
                return new ClientProgramWorkspaceSnapshot(
                    ClientProgramLoadStatus.Empty,
                    null,
                    null,
                    null,
                    0,
                    null,
                    Array.Empty<string>())
                {
                    SelectedStepId=null
                };
            }

            return new ClientProgramWorkspaceSnapshot(
                _errors.Count==0
                    ? ClientProgramLoadStatus.Ready
                    : ClientProgramLoadStatus.Invalid,
                _program.ProgramId,
                _program.Name,
                _program.Version,
                _program.Steps.Count,
                _executionPlan?.Fingerprint,
                _errors.ToArray())
            {
                SelectedStepId=_selectedStepId
            };
        }
    }

    public InspectionProgram? CurrentProgram=>_program;

    public ProgramExecutionPlan? ExecutionPlan=>_executionPlan;

    public ClientProgramWorkspaceSnapshot Load(
        InspectionProgram program)
    {
        ArgumentNullException.ThrowIfNull(program);

        _program=program;
        _errors=InspectionProgramValidationRuntime.Validate(program);

        if(_errors.Count==0)
            _executionPlan=ProgramExecutionPlanRuntime.Create(program);
        else
            _executionPlan=null;

        _selectedStepId=_errors.Count==0
            ? program.Steps.OrderBy(step=>step.Order).ThenBy(step=>step.StepId)
                .Select(step=>(Guid?)step.StepId).FirstOrDefault()
            : null;

        return Snapshot;
    }

    public ProgramStep? SelectedStep
    {
        get
        {
            if(_program is null || _selectedStepId is not Guid selected)
                return null;

            return _program.Steps.FirstOrDefault(step=>step.StepId==selected);
        }
    }

    public bool SelectStep(Guid stepId)
    {
        if(_program is null || _errors.Count!=0)
            return false;

        var step=_program.Steps.FirstOrDefault(candidate=>candidate.StepId==stepId);
        if(step is null)
            return false;

        _selectedStepId=step.StepId;
        return true;
    }

    public ProductionSessionDefinition CreateSessionDefinition(
        Guid sessionId,
        PipelineDefinition<CapturedFrame> pipeline,
        int frameCount)
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        if(sessionId==Guid.Empty)
            throw new ArgumentException("Client Production session identity cannot be empty.",nameof(sessionId));
        if(frameCount<=0)
            throw new ArgumentOutOfRangeException(nameof(frameCount));
        if(_executionPlan is null)
            throw new InvalidOperationException("A valid client program must be loaded before creating a Production session.");

        return new ProductionSessionDefinition(
            sessionId,
            _executionPlan,
            pipeline,
            frameCount);
    }

    public void Reset()
    {
        _program=null;
        _executionPlan=null;
        _errors=Array.Empty<string>();
        _selectedStepId=null;
    }
}
