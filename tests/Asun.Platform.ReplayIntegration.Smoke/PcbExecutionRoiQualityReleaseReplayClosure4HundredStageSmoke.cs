using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.QualityReleaseIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class PcbExecutionRoiQualityReleaseReplayClosure4HundredStageSmoke
{
    private static string H(char c)=>new(c,64);

    private static PcbExecutionRoiReplayBinding RoiBinding(bool ready=true, Guid? qualityRunOverride=null)
    {
        var session=Guid.Parse("00000000-0000-0000-0000-000000000101");
        var quality=qualityRunOverride ?? Guid.Parse("00000000-0000-0000-0000-000000000202");
        return new PcbExecutionRoiReplayBinding(
            session,
            quality,
            H('a'),
            H('b'),
            H('c'),
            ready,
            "logical-artifact.pcb",
            H('d'));
    }

    private static QualityReleaseReplayDescriptor QualityRelease(Guid qualityRun,bool ready=true)
    {
        return new QualityReleaseReplayDescriptor(
            qualityRun,
            H('e'),
            H('f'),
            ready,
            H('g'),
            H('h'));
    }

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                var tampered=closure with { RoiBindingFingerprint=H('z') };
                Check(!PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,tampered) &&
                      round<=100,
                      "ROI binding identity tamper must be rejected");
        }
        if(round!=100)
            throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");

        return Task.CompletedTask;
    }
}
