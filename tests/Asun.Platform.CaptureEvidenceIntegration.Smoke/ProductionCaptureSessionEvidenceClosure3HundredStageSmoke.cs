using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;

namespace Asun.Platform.CaptureEvidenceIntegration.Smoke;

public static class ProductionCaptureSessionEvidenceClosure3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i1%2]=evidence[i1%2] with
                {
                    Handles=evidence[i1%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group1<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i2%2]=evidence[i2%2] with
                {
                    Handles=evidence[i2%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group2<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i3%2]=evidence[i3%2] with
                {
                    Handles=evidence[i3%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group3<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i4%2]=evidence[i4%2] with
                {
                    Handles=evidence[i4%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group4<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i5%2]=evidence[i5%2] with
                {
                    Handles=evidence[i5%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group5<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i6%2]=evidence[i6%2] with
                {
                    Handles=evidence[i6%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group6<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i7%2]=evidence[i7%2] with
                {
                    Handles=evidence[i7%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group7<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i8%2]=evidence[i8%2] with
                {
                    Handles=evidence[i8%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group8<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i9%2]=evidence[i9%2] with
                {
                    Handles=evidence[i9%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group9<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        for(var group10=0;group10<10;group10++)
        {
            for(var i10=0;i10<10;i10++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var evidence=fixture.Evidence.ToArray();
                evidence[i10%2]=evidence[i10%2] with
                {
                    Handles=evidence[i10%2].Handles.Reverse().ToArray()
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,fixture.Provenance,evidence) &&
                    (group10<9 || round==100),
                    "non-canonical opaque Evidence handle order must block the closure");
            }
        }
        return Task.CompletedTask;
    }
}
