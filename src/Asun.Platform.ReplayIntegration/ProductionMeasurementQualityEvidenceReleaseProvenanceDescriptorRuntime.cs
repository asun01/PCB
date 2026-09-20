using System.Security.Cryptography;
using System.Text;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor(
    long Sequence,
    string ProductionInputFingerprint,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc,
    string ReleaseReplayDescriptorFingerprint,
    string ProvenanceFingerprint);

public static class ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime
{
    public static ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor Create(
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(replayDescriptor);

        var errors=ValidateInputs(provenance,replayDescriptor);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(provenance));

        var fingerprint=CreateFingerprint(
            provenance.Sequence.Value,
            provenance.PayloadFingerprint,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat,
            provenance.CapturedAtUtc,
            replayDescriptor.DescriptorFingerprint);

        return new ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor(
            provenance.Sequence.Value,
            provenance.PayloadFingerprint,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat,
            provenance.CapturedAtUtc,
            replayDescriptor.DescriptorFingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(replayDescriptor);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=ValidateInputs(provenance,replayDescriptor);

        if(descriptor.Sequence!=provenance.Sequence.Value)
            errors.Add("Provenance descriptor sequence must match.");
        if(descriptor.ProductionInputFingerprint!=provenance.PayloadFingerprint)
            errors.Add("Provenance descriptor Production input fingerprint must match.");
        if(descriptor.Width!=provenance.Width || descriptor.Height!=provenance.Height)
            errors.Add("Provenance descriptor dimensions must match.");
        if(descriptor.PixelFormat!=provenance.PixelFormat)
            errors.Add("Provenance descriptor pixel format must match.");
        if(descriptor.CapturedAtUtc!=provenance.CapturedAtUtc)
            errors.Add("Provenance descriptor capture timestamp must match.");
        if(descriptor.ReleaseReplayDescriptorFingerprint!=replayDescriptor.DescriptorFingerprint)
            errors.Add("Provenance descriptor replay identity must match.");
        if(descriptor.ProvenanceFingerprint.Length!=64 || !IsLowerHex(descriptor.ProvenanceFingerprint))
            errors.Add("Provenance descriptor fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                provenance.Sequence.Value,
                provenance.PayloadFingerprint,
                provenance.Width,
                provenance.Height,
                provenance.PixelFormat,
                provenance.CapturedAtUtc,
                replayDescriptor.DescriptorFingerprint);
            if(expected!=descriptor.ProvenanceFingerprint)
                errors.Add("Provenance descriptor fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor descriptor)=>
        Validate(provenance,replayDescriptor,descriptor).Count==0;

    internal static string CreateFingerprint(
        long sequence,
        string productionInputFingerprint,
        long width,
        long height,
        string pixelFormat,
        DateTimeOffset capturedAtUtc,
        string releaseReplayDescriptorFingerprint)
    {
        var canonical=string.Join(
            "|",
            sequence,
            productionInputFingerprint,
            width,
            height,
            pixelFormat.Length,
            pixelFormat,
            capturedAtUtc.UtcTicks,
            releaseReplayDescriptorFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseReplayDescriptor replayDescriptor)
    {
        var errors=new List<string>();

        if(provenance.Sequence.Value<=0)
            errors.Add("Production provenance sequence must be positive.");
        if(!IsLowerHex(provenance.PayloadFingerprint))
            errors.Add("Production provenance payload fingerprint must be 64 lowercase hexadecimal characters.");
        if(provenance.Width<=0 || provenance.Height<=0)
            errors.Add("Production provenance dimensions must be positive.");
        if(string.IsNullOrWhiteSpace(provenance.PixelFormat))
            errors.Add("Production provenance pixel format is required.");
        if(!IsLowerHex(replayDescriptor.DescriptorFingerprint))
            errors.Add("Replay descriptor fingerprint must be 64 lowercase hexadecimal characters.");
        if(!IsLowerHex(replayDescriptor.ProductionInputFingerprint))
            errors.Add("Replay descriptor Production input fingerprint must be 64 lowercase hexadecimal characters.");
        if(replayDescriptor.Sequence!=provenance.Sequence.Value)
            errors.Add("Production provenance and replay descriptor sequences must match.");
        if(replayDescriptor.ProductionInputFingerprint!=provenance.PayloadFingerprint)
            errors.Add("Production provenance payload fingerprint must match replay Production input fingerprint.");

        return errors;
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
