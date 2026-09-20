using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration.Smoke;

public static class ProductionCaptureSessionReconciliation2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group=0;group<10;group++)
        {
            for(var i=0;i<10;i++)
            {
                round++;
                var r=new ProductionSessionReport(
    Guid.Parse("00000000-0000-0000-0000-000000000001"),
    "program-fingerprint",
    3,
    Enumerable.Range(1,3)
        .Select(i=>new ProductionFrameExecution(
            FrameSequence.Create(i),
            "capture-"+i.ToString(),
            new PipelineExecutionReport(1,new[]{"stage-"+i.ToString()},"pipeline-"+i.ToString())))
        .ToArray(),
    "production-report-fingerprint");
                var c=new CaptureSessionSnapshot(
    3,
    FrameSequence.Create(1),
    FrameSequence.Create(3),
    Enumerable.Range(1,3).Select(i=>"capture-"+i.ToString()).ToArray());
                var fingerprints=c.Fingerprints.ToArray();
                fingerprints[i%3]="tampered-"+round;
                var tampered=new CaptureSessionSnapshot(c.CapturedCount,c.FirstSequence,c.LastSequence,fingerprints);
                Check(!ProductionCaptureSessionReconciliationRuntime.IsValid(r,tampered) &&
                      (group<9 || round==100),
                      "payload fingerprint drift must be rejected");
            }
        }
        return Task.CompletedTask;
    }
}
