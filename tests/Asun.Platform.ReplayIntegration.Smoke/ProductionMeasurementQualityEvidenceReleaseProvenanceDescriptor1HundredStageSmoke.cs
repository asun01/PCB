using Asun.Platform.ReleaseIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Production.Runtime;

public static class ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor1HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var provenance=new ProductionFrameProvenance(
            new Asun.Device.Contracts.FrameSequence(7),
            640,
            480,
            "Mono8",
            DateTimeOffset.Parse("2026-09-20T05:00:00Z"),
            new string('b',64));
        var releaseBinding=new ProductionMeasurementQualityEvidenceReleaseBinding(
            7,
            new string('b',64),
            Guid.Parse("E7000000-0000-0000-0000-000000000001"),
            "C1",
            new string('d',64),
            new string('e',64),
            true,
            new string('f',64));
        var replayBinding=new ProductionQualityEvidenceReleaseReplayBinding(
            Guid.Parse("E7000000-0000-0000-0000-000000000010"),
            Guid.Parse("E7000000-0000-0000-0000-000000000011"),
            new string('d',64),
            new string('e',64),
            true,
            new string('f',64));
        var replayDescriptor=ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(releaseBinding,replayBinding);
        var descriptor=ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(provenance,replayDescriptor);
for(var i=0;i<10;i++) Check(descriptor.Sequence==provenance.Sequence.Value,"Sequence should match provenance.");
for(var i=0;i<10;i++) Check(descriptor.ProductionInputFingerprint==provenance.PayloadFingerprint,"Production input identity should match payload.");
for(var i=0;i<10;i++) Check(descriptor.Width==provenance.Width && descriptor.Height==provenance.Height,"Dimensions should match.");
for(var i=0;i<10;i++) Check(descriptor.PixelFormat==provenance.PixelFormat,"Pixel format should match.");
for(var i=0;i<10;i++) Check(descriptor.CapturedAtUtc==provenance.CapturedAtUtc,"Capture timestamp should match.");
for(var i=0;i<10;i++) Check(descriptor.ReleaseReplayDescriptorFingerprint==replayDescriptor.DescriptorFingerprint,"Replay identity should match.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.IsValid(provenance,replayDescriptor,descriptor),"Canonical provenance descriptor should validate.");
for(var i=0;i<10;i++) Check(descriptor.ProvenanceFingerprint.Length==64,"Provenance fingerprint should be fixed width.");
for(var i=0;i<10;i++) Check(descriptor.ProvenanceFingerprint.All(Uri.IsHexDigit),"Provenance fingerprint should be hexadecimal.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(provenance,replayDescriptor).ProvenanceFingerprint==descriptor.ProvenanceFingerprint,"Creation should be deterministic.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
