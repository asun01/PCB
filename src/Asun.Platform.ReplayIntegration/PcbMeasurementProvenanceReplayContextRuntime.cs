using System.Security.Cryptography;
using System.Text;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public sealed record PcbMeasurementProvenanceReplayContext(
    long Sequence,
    string ProductionInputFingerprint,
    string ComponentId,
    string Designator,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc,
    string ReleaseReplayDescriptorFingerprint,
    string BindingFingerprint);

public static class PcbMeasurementProvenanceReplayContextRuntime
{
    public static PcbMeasurementProvenanceReplayContext Create(
        ProductionMeasurementPcbBinding measurementBinding,
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor)
    {
        ArgumentNullException.ThrowIfNull(measurementBinding);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(provenanceDescriptor);

        var errors=Validate(measurementBinding,provenance,provenanceDescriptor);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            measurementBinding.Sequence,
            measurementBinding.ProductionInputFingerprint,
            measurementBinding.ComponentId.Value.Length,
            measurementBinding.ComponentId.Value,
            measurementBinding.Designator.Length,
            measurementBinding.Designator,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat.Length,
            provenance.PixelFormat,
            provenance.CapturedAtUtc.UtcTicks,
            provenanceDescriptor.ReleaseReplayDescriptorFingerprint,
            provenanceDescriptor.ProvenanceFingerprint);

        return new PcbMeasurementProvenanceReplayContext(
            measurementBinding.Sequence,
            measurementBinding.ProductionInputFingerprint,
            measurementBinding.ComponentId.Value,
            measurementBinding.Designator,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat,
            provenance.CapturedAtUtc,
            provenanceDescriptor.ReleaseReplayDescriptorFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementPcbBinding measurementBinding,
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor)
    {
        ArgumentNullException.ThrowIfNull(measurementBinding);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(provenanceDescriptor);

        var errors=new List<string>();

        if(measurementBinding.Sequence<=0)
            errors.Add("Measurement binding sequence must be positive.");
        if(!measurementBinding.ComponentId.IsValid)
            errors.Add("Measurement binding component identity is invalid.");
        if(string.IsNullOrWhiteSpace(measurementBinding.Designator))
            errors.Add("Measurement binding designator cannot be blank.");

        if(provenance.Sequence.Value<=0)
            errors.Add("Production provenance sequence must be positive.");
        if(!IsLowerHex(measurementBinding.ProductionInputFingerprint))
            errors.Add("Measurement Production input fingerprint is malformed.");
        if(!IsLowerHex(provenance.PayloadFingerprint))
            errors.Add("Production provenance payload fingerprint is malformed.");
        if(measurementBinding.ProductionInputFingerprint!=provenance.PayloadFingerprint)
            errors.Add("Measurement and provenance Production input fingerprints must match.");

        if(provenance.Width<=0 || provenance.Height<=0)
            errors.Add("Production provenance dimensions must be positive.");
        if(string.IsNullOrWhiteSpace(provenance.PixelFormat))
            errors.Add("Production provenance pixel format cannot be blank.");

        if(provenanceDescriptor.Sequence!=provenance.Sequence.Value ||
           provenanceDescriptor.Sequence!=measurementBinding.Sequence)
            errors.Add("Measurement, provenance, and provenance-descriptor sequences must match.");
        if(provenanceDescriptor.ProductionInputFingerprint!=provenance.PayloadFingerprint)
            errors.Add("Provenance descriptor Production input fingerprint must match the frame provenance.");
        if(provenanceDescriptor.Width!=provenance.Width ||
           provenanceDescriptor.Height!=provenance.Height)
            errors.Add("Provenance descriptor dimensions must match the frame provenance.");
        if(provenanceDescriptor.PixelFormat!=provenance.PixelFormat)
            errors.Add("Provenance descriptor pixel format must match the frame provenance.");
        if(provenanceDescriptor.CapturedAtUtc!=provenance.CapturedAtUtc)
            errors.Add("Provenance descriptor capture timestamp must match the frame provenance.");

        if(!IsLowerHex(provenanceDescriptor.ReleaseReplayDescriptorFingerprint))
            errors.Add("Release replay descriptor fingerprint is malformed.");
        if(!IsLowerHex(provenanceDescriptor.ProvenanceFingerprint))
            errors.Add("Provenance descriptor fingerprint is malformed.");

        return errors;
    }

    public static bool IsValid(
        ProductionMeasurementPcbBinding measurementBinding,
        ProductionFrameProvenance provenance,
        ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor provenanceDescriptor)=>
        Validate(measurementBinding,provenance,provenanceDescriptor).Count==0;

    public static bool IsEquivalent(
        PcbMeasurementProvenanceReplayContext left,
        PcbMeasurementProvenanceReplayContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
