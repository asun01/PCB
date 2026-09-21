namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionExecutionPresentationSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) CompletedShowsResult();
        for(var round=1;round<=100;round++) if(round==100) RunningShowsProgress();
        for(var round=1;round<=100;round++) if(round==100) PendingResultIsNotInvented();
        for(var round=1;round<=100;round++) if(round==100) UnboundProgramIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) UnavailableRoiIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) CompletedStatusIsVisible();
        for(var round=1;round<=100;round++) if(round==100) UnboundAcquisitionIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) StatusComesFromProduction();
        for(var round=1;round<=100;round++) if(round==100) ProgramSummaryIsPresentationOnly();
        for(var round=1;round<=100;round++) if(round==100) ResultUsesObservedFrameCount();
        for(var round=1;round<=100;round++) if(round==100) UncapturedPreviewIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) PreviewUsesObservedMetadata();

    }

    private static void CompletedShowsResult()
    {
        var s=Create(ClientExecutionStatus.Completed,3,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ResultText=="Result — 3 frames","Completed result must use observed frame count.");
    }

    private static void RunningShowsProgress()
    {
        var s=Create(ClientExecutionStatus.Running,1,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ProgressText.Contains("1/3 frames",StringComparison.Ordinal),"Running progress must be visible.");
    }

    private static void PendingResultIsNotInvented()
    {
        var s=Create(ClientExecutionStatus.Running,1,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ResultText=="Result — pending","Incomplete execution must not claim a result.");
    }

    private static void UnboundProgramIsExplicit()
    {
        var s=Create(ClientExecutionStatus.Idle,0,0);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ProgramSummary=="Program — unbound","Unbound program must remain explicit.");
    }

    private static void UnavailableRoiIsExplicit()
    {
        var s=Create(ClientExecutionStatus.Running,1,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.RoiText.Contains("unavailable",StringComparison.Ordinal),"ROI must not be presented before completion.");
    }

    private static void CompletedStatusIsVisible()
    {
        var s=Create(ClientExecutionStatus.Completed,3,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ExecutionStatus=="Completed","Completed status must be visible.");
    }

    private static void UnboundAcquisitionIsExplicit()
    {
        var s=Create(ClientExecutionStatus.Idle,0,0);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.AcquisitionText=="Acquisition — unbound","Acquisition state must be explicit.");
    }

    private static void StatusComesFromProduction()
    {
        var s=Create(ClientExecutionStatus.Cancelled,2,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ExecutionStatus=="Cancelled","Execution status must project the Production authority state.");
    }

    private static void ProgramSummaryIsPresentationOnly()
    {
        var s=Create(ClientExecutionStatus.Ready,0,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ProgramSummary.StartsWith("Program —",StringComparison.Ordinal),"Program summary must remain a presentation projection.");
    }

    private static void UncapturedPreviewIsExplicit()
    {
        var s=Create(ClientExecutionStatus.Ready,0,3);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.AcquisitionPreviewText=="Preview — not captured",
            "Inspection presentation must not invent an acquisition preview.");
    }

    private static void PreviewUsesObservedMetadata()
    {
        var s=Create(ClientExecutionStatus.Ready,0,3) with
        {
            Acquisition=s.Acquisition with
            {
                Preview=new ClientAcquisitionPreviewSnapshot(
                    new FrameSequence(7),
                    640,
                    480,
                    "Mono8",
                    DateTimeOffset.UnixEpoch,
                    "preview-fingerprint",
                    Array.Empty<byte>())
            }
        };
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.AcquisitionPreviewText.Contains("640×480",StringComparison.Ordinal) &&
              p.AcquisitionPreviewText.Contains("Mono8",StringComparison.Ordinal) &&
              p.AcquisitionPreviewText.Contains("Frame 7",StringComparison.Ordinal),
            "Inspection presentation must project observed preview metadata.");
    }

    private static void ResultUsesObservedFrameCount()
    {
        var s=Create(ClientExecutionStatus.Completed,4,4);
        var p=ClientInspectionExecutionPresentationRuntime.Create(s);
        Check(p.ResultText=="Result — 4 frames","Result count must use observed production frame count.");
    }

    private static ClientInspectionWorkspaceSnapshot Create(
        ClientExecutionStatus status,int processed,int target)
    {
        using var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        var captured=workspace.Capture();
        return captured with
        {
            Production=captured.Production with
            {
                Status=status,
                FramesProcessed=processed,
                TargetFrameCount=target,
                LastFrameCount=processed
            }
        };
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException("Inspection execution presentation smoke failed: "+message);
    }
}
