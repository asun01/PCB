using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration.Smoke;

public static class ProductionCaptureSessionReconciliation3HundredStageSmoke
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
                var c=i%2==0 ? new CaptureSessionSnapshot(
    2,
    FrameSequence.Create(1),
    FrameSequence.Create(2),
    Enumerable.Range(1,2).Select(i=>"capture-"+i.ToString()).ToArray()) : new CaptureSessionSnapshot(
    4,
    FrameSequence.Create(1),
    FrameSequence.Create(4),
    Enumerable.Range(1,4).Select(i=>"capture-"+i.ToString()).ToArray());
                Check(!ProductionCaptureSessionReconciliationRuntime.IsValid(r,c) &&
                      (group<9 || round==100),
                      "capture/production count drift must be rejected");
            }
        }
        return Task.CompletedTask;
    }
}
