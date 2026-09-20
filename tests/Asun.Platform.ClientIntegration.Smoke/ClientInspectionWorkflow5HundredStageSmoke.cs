using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientInspectionWorkflow5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(new ClientInspectionWorkspaceSnapshot(
    new ClientWorkspaceSnapshot(
        Guid.NewGuid(),
        new Version(1,0,0),
        Guid.NewGuid(),
        ClientExecutionStatus.Completed,
        frames,
        null,
        null)
    {
        TargetFrameCount=3,
        FramesProcessed=3
    },
    null,
    null,
    null,
    new ClientProductionRunHistorySnapshot(5,historyCount+1,0,Enumerable.Range(0,historyCount).Select(i=>
        new ClientProductionRunHistoryEntry(
            i+1,Guid.NewGuid(),Guid.NewGuid(),3,new string((char)('a'+i),64),true,"artifact")).ToArray()))
{
    Program=new ClientProgramWorkspaceSnapshot(
        ClientProgramLoadStatus.Ready,
        Guid.NewGuid(),
        "Program",
        new Version(1,0,0),
        3,
        new string('p',64),
        Array.Empty<string>()),
    Acquisition=new ClientAcquisitionWorkspaceSnapshot(
        ClientAcquisitionState.Ready,
        new ClientAcquisitionDescriptor("simulation","Simulation",true),
        null,
        true),
    Quality=new ClientQualityWorkspaceSnapshot(
        Guid.NewGuid(),
        1,
        1,
        1,
        0,
        0,
        0,
        new string('q',64),
        true,
        Array.Empty<ClientQualityFindingDisplayItem>())
});
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.CanAct &&
                  workflow.HistoryCount==1,
                  "Completed Production with Quality and history must enter Results review");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
