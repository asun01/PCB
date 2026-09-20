using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;

namespace Asun.Platform.CaptureEvidenceIntegration.Smoke;

public static class ProductionCaptureSessionEvidenceClosure4HundredStageSmoke
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
                var provenance=fixture.Provenance.ToArray();
                provenance[i1%2]=provenance[i1%2] with
                {
                    Width=i1%2==0 ? 0 : provenance[i1%2].Width,
                    PayloadFingerprint=i1%2==0 ? provenance[i1%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group1<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i2%2]=provenance[i2%2] with
                {
                    Width=i2%2==0 ? 0 : provenance[i2%2].Width,
                    PayloadFingerprint=i2%2==0 ? provenance[i2%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group2<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i3%2]=provenance[i3%2] with
                {
                    Width=i3%2==0 ? 0 : provenance[i3%2].Width,
                    PayloadFingerprint=i3%2==0 ? provenance[i3%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group3<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i4%2]=provenance[i4%2] with
                {
                    Width=i4%2==0 ? 0 : provenance[i4%2].Width,
                    PayloadFingerprint=i4%2==0 ? provenance[i4%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group4<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i5%2]=provenance[i5%2] with
                {
                    Width=i5%2==0 ? 0 : provenance[i5%2].Width,
                    PayloadFingerprint=i5%2==0 ? provenance[i5%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group5<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i6%2]=provenance[i6%2] with
                {
                    Width=i6%2==0 ? 0 : provenance[i6%2].Width,
                    PayloadFingerprint=i6%2==0 ? provenance[i6%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group6<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i7%2]=provenance[i7%2] with
                {
                    Width=i7%2==0 ? 0 : provenance[i7%2].Width,
                    PayloadFingerprint=i7%2==0 ? provenance[i7%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group7<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i8%2]=provenance[i8%2] with
                {
                    Width=i8%2==0 ? 0 : provenance[i8%2].Width,
                    PayloadFingerprint=i8%2==0 ? provenance[i8%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group8<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i9%2]=provenance[i9%2] with
                {
                    Width=i9%2==0 ? 0 : provenance[i9%2].Width,
                    PayloadFingerprint=i9%2==0 ? provenance[i9%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group9<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        for(var group10=0;group10<10;group10++)
        {
            for(var i10=0;i10<10;i10++)
            {
                round++;
                var fixture=CaptureEvidenceClosureFixtureRuntime.Create();
                var provenance=fixture.Provenance.ToArray();
                provenance[i10%2]=provenance[i10%2] with
                {
                    Width=i10%2==0 ? 0 : provenance[i10%2].Width,
                    PayloadFingerprint=i10%2==0 ? provenance[i10%2].PayloadFingerprint : "9999999999999999999999999999999999999999999999999999999999999999"
                };
                Check(
                    !ProductionCaptureSessionEvidenceClosureRuntime.IsValid(
                        fixture.Production,fixture.Capture,provenance,fixture.Evidence) &&
                    (group10<9 || round==100),
                    "capture provenance drift must block the Evidence closure");
            }
        }
        return Task.CompletedTask;
    }
}
