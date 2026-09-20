namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionMeasurementQualityEvidenceReleaseAuditReplayContext5HundredStageSmoke
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
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group0<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group1<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group2<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group3<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group4<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group5<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group6<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group7<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group8<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var left=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                var right=ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.Create(
                    fixture.ReplayDescriptor,fixture.AuditBinding);
                Check(
                    ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsEquivalent(left,right) &&
                    left.ReplayDescriptorFingerprint==right.ReplayDescriptorFingerprint &&
                    left.AuditReplayBindingFingerprint==right.AuditReplayBindingFingerprint &&
                    (group9<9 || round==100),
                    "equivalent audit replay inputs must converge");
            }
        }
        return Task.CompletedTask;
    }
}
