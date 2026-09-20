using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Metrology.Core;

namespace Asun.Platform.MetrologyProductionIntegration;

public sealed record ProductionMeasurementPcbBinding(
    long Sequence,
    string ProductionInputFingerprint,
    PcbFeatureId ComponentId,
    string Designator,
    string CalibrationFingerprint,
    string ObservationFingerprint,
    string BindingFingerprint);

public sealed record ProductionMeasurementPcbReplayDescriptor(
    long Sequence,
    string ProductionInputFingerprint,
    PcbFeatureId ComponentId,
    string ObservationFingerprint,
    string BindingFingerprint);

public static class ProductionMeasurementPcbBindingRuntime
{
    public static ProductionMeasurementPcbBinding Create(
        ProductionMeasurementFact fact,
        CalibratedPcbPlacementObservation observation)
    {
        ArgumentNullException.ThrowIfNull(observation);

        if(fact.Sequence<0)
            throw new ArgumentOutOfRangeException(nameof(fact));
        if(string.IsNullOrWhiteSpace(fact.ProductionInputFingerprint))
            throw new ArgumentException("Production input fingerprint is required.",nameof(fact));
        if(!observation.Observation.ComponentId.IsValid)
            throw new ArgumentException("PCB component identity is invalid.",nameof(observation));

        if(fact.ObservationFingerprint!=observation.Fingerprint)
            throw new ArgumentException("Measurement fact must belong to the calibrated observation.",nameof(fact));

        var fingerprint=CreateFingerprint(
            fact.Sequence,
            fact.ProductionInputFingerprint,
            observation.Observation.ComponentId,
            observation.Observation.Designator,
            observation.CalibrationFingerprint,
            observation.Fingerprint);

        return new ProductionMeasurementPcbBinding(
            fact.Sequence,
            fact.ProductionInputFingerprint,
            observation.Observation.ComponentId,
            observation.Observation.Designator,
            observation.CalibrationFingerprint,
            observation.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionMeasurementFact fact,
        CalibratedPcbPlacementObservation observation,
        ProductionMeasurementPcbBinding binding)
    {
        ArgumentNullException.ThrowIfNull(observation);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();
        if(fact.Sequence<0)
            errors.Add("Measurement fact sequence must be non-negative.");
        if(string.IsNullOrWhiteSpace(fact.ProductionInputFingerprint))
            errors.Add("Production input fingerprint is required.");
        if(!observation.Observation.ComponentId.IsValid)
            errors.Add("PCB component identity is invalid.");
        if(fact.ObservationFingerprint!=observation.Fingerprint)
            errors.Add("Measurement fact must belong to the calibrated observation.");
        if(binding.Sequence!=fact.Sequence)
            errors.Add("Binding sequence must match.");
        if(binding.ProductionInputFingerprint!=fact.ProductionInputFingerprint)
            errors.Add("Binding Production input fingerprint must match.");
        if(binding.ComponentId!=observation.Observation.ComponentId)
            errors.Add("Binding component identity must match.");
        if(binding.Designator!=observation.Observation.Designator)
            errors.Add("Binding designator must match.");
        if(binding.CalibrationFingerprint!=observation.CalibrationFingerprint)
            errors.Add("Binding calibration fingerprint must match.");
        if(binding.ObservationFingerprint!=observation.Fingerprint)
            errors.Add("Binding observation fingerprint must match.");

        if(binding.BindingFingerprint.Length!=64 ||
           !binding.BindingFingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
            errors.Add("Measurement PCB binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=CreateFingerprint(
            fact.Sequence,
            fact.ProductionInputFingerprint,
            observation.Observation.ComponentId,
            observation.Observation.Designator,
            observation.CalibrationFingerprint,
            observation.Fingerprint);
        if(expected!=binding.BindingFingerprint)
            errors.Add("Measurement PCB binding fingerprint does not match canonical content.");

        return errors;
    }

    public static string CreateCanonicalKey(
        ProductionMeasurementPcbBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(string.Join(
                    "|",
                    binding.Sequence,
                    binding.ProductionInputFingerprint,
                    binding.ComponentId.Value,
                    binding.ObservationFingerprint))))
            .ToLowerInvariant();
    }

    public static bool IsEquivalent(
        ProductionMeasurementPcbBinding left,
        ProductionMeasurementPcbBinding right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return left.BindingFingerprint==right.BindingFingerprint;
    }

    public static ProductionMeasurementPcbReplayDescriptor CreateReplayDescriptor(
        ProductionMeasurementPcbBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        return new ProductionMeasurementPcbReplayDescriptor(
            binding.Sequence,
            binding.ProductionInputFingerprint,
            binding.ComponentId,
            binding.ObservationFingerprint,
            binding.BindingFingerprint);
    }

    internal static string CreateFingerprint(
        long sequence,
        string productionInputFingerprint,
        PcbFeatureId componentId,
        string designator,
        string calibrationFingerprint,
        string observationFingerprint)
    {
        var canonical=string.Join(
            "|",
            sequence,
            productionInputFingerprint.Length,
            productionInputFingerprint,
            componentId.Value.Length,
            componentId.Value,
            designator.Length,
            designator,
            calibrationFingerprint,
            observationFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }
}
