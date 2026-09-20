using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration.Smoke;

public static class ProductionCaptureSessionReconciliation1HundredStageSmoke
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
                var result=ProductionCaptureSessionReconciliationRuntime.Create(r,new CaptureSessionSnapshot(
    3,
    FrameSequence.Create(1),
    FrameSequence.Create(3),
    Enumerable.Range(1,3).Select(i=>"capture-"+i.ToString()).ToArray()));
                Check(ProductionCaptureSessionReconciliationRuntime.IsValid(r,new CaptureSessionSnapshot(
    3,
    FrameSequence.Create(1),
    FrameSequence.Create(3),
    Enumerable.Range(1,3).Select(i=>"capture-"+i.ToString()).ToArray())) &&
                      result.FrameCount==3 &&
                      result.FirstSequence==1 &&
                      result.LastSequence==3 &&
                      result.ReconciliationFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "happy-path reconciliation must be deterministic and complete");
            }
        }
        return Task.CompletedTask;
    }
}
