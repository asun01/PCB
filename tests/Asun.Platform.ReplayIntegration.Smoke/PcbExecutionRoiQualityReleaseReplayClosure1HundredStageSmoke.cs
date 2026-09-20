using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.QualityReleaseIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class PcbExecutionRoiQualityReleaseReplayClosure1HundredStageSmoke
{
    private static string H(char c)=>new(c,64);

    private static PcbExecutionRoiReplayBinding RoiBinding(bool ready=true, string? qualityRun=null)
    {
        var session=Guid.Parse("00000000-0000-0000-0000-000000000101");
        var quality=Guid.Parse(qualityRun??"00000000-0000-0000-0000-000000000202");
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
        for(var group0=0;group0<1;group0++)
        {
            for(var iteration0=0;iteration0<10;iteration0++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group1=0;group1<1;group1++)
        {
            for(var iteration1=0;iteration1<10;iteration1++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group2=0;group2<1;group2++)
        {
            for(var iteration2=0;iteration2<10;iteration2++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group3=0;group3<1;group3++)
        {
            for(var iteration3=0;iteration3<10;iteration3++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group4=0;group4<1;group4++)
        {
            for(var iteration4=0;iteration4<10;iteration4++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group5=0;group5<1;group5++)
        {
            for(var iteration5=0;iteration5<10;iteration5++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group6=0;group6<1;group6++)
        {
            for(var iteration6=0;iteration6<10;iteration6++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group7=0;group7<1;group7++)
        {
            for(var iteration7=0;iteration7<10;iteration7++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group8=0;group8<1;group8++)
        {
            for(var iteration8=0;iteration8<10;iteration8++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        for(var group9=0;group9<1;group9++)
        {
            for(var iteration9=0;iteration9<10;iteration9++)
            {
                round++;
                var binding=RoiBinding();
                var release=QualityRelease(binding.QualityRunId);
                var closure=PcbExecutionRoiQualityReleaseReplayClosureRuntime.Create(binding,release);
                Check(PcbExecutionRoiQualityReleaseReplayClosureRuntime.IsValidClosure(binding,release,closure) &&
                      closure.ProductionSessionId==binding.ProductionSessionId &&
                      closure.QualityRunId==binding.QualityRunId &&
                      closure.ReleaseReady &&
                      closure.ClosureFingerprint.Length==64 &&
                      (group<9 || round==100),
                      "clean ROI/quality release replay closure must be valid");
            }
        }
        Check(round==100,"acceptance matrix must execute exactly 100 rounds");
        return Task.CompletedTask;
    }
}
