namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionMeasurementQualityEvidenceReleaseAuditReplayContext4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            for(var i0=0;i0<10;i0++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i0%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i0%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i0<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i1%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i1%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i1<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i2%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i2%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i2<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i3%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i3%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i3<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i4%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i4%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i4<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i5%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i5%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i5<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i6%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i6%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i6<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i7%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i7%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i7<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i8%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i8%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i8<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    AuditSequence=i9%2==0 ? 0 : fixture.AuditBinding.AuditSequence,
                    AuditTraceFingerprint=i9%2==0 ? fixture.AuditBinding.AuditTraceFingerprint : "bad"
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (i9<9 || round==100),
                    "Audit sequence/trace drift must be rejected");
            }
        }
        return Task.CompletedTask;
    }
}
