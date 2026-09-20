using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.PcbExecutionIntegration;

public sealed record PcbExecutionMeasurementComponentBinding(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    int MeasurementFactCount,
    IReadOnlyList<PcbFeatureId> ComponentIds,
    IReadOnlyList<string> Designators,
    string MeasurementBindingsFingerprint,
    string BindingFingerprint);

public static class PcbExecutionMeasurementComponentBindingRuntime
{
    public static PcbExecutionMeasurementComponentBinding Create(
        PcbExecutionSnapshot executionSnapshot,
        PcbAssemblySnapshot assembly,
        IReadOnlyList<ProductionMeasurementPcbBinding> measurementBindings)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(measurementBindings);

        var errors=Validate(executionSnapshot,assembly,measurementBindings);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var ordered=measurementBindings.OrderBy(binding=>binding.Sequence).ToArray();
        var bindingEntries=ordered.Select(binding=>
            ProductionMeasurementPcbBindingRuntime.CreateCanonicalKey(binding));
        var measurementFingerprint=Hash(string.Join("
",bindingEntries));

        var canonical=string.Join("|",
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.MeasurementFactCount,
            measurementFingerprint,
            string.Join(",",ordered.Select(binding=>binding.ComponentId.Value)),
            string.Join(",",ordered.Select(binding=>binding.Designator)));

        return new PcbExecutionMeasurementComponentBinding(
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.MeasurementFactCount,
            ordered.Select(binding=>binding.ComponentId).ToArray(),
            ordered.Select(binding=>binding.Designator).ToArray(),
            measurementFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        PcbAssemblySnapshot assembly,
        IReadOnlyList<ProductionMeasurementPcbBinding> measurementBindings)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(measurementBindings);

        var errors=new List<string>();

        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            errors.Add("PCB assembly snapshot is invalid.");

        if(executionSnapshot.AssemblyFingerprint!=assembly.Fingerprint)
            errors.Add("Execution snapshot assembly fingerprint must match the PCB assembly.");

        if(executionSnapshot.ProductionSessionId==Guid.Empty)
            errors.Add("Execution snapshot Production session id cannot be empty.");

        if(executionSnapshot.MeasurementFactCount<0)
            errors.Add("Execution snapshot measurement fact count cannot be negative.");

        if(executionSnapshot.MeasurementFactCount!=measurementBindings.Count)
            errors.Add("Measurement binding count must match the execution snapshot measurement fact count.");

        var componentsById=assembly.Components
            .Where(component=>component.Id.IsValid)
            .ToDictionary(component=>component.Id);
        var componentsByDesignator=assembly.Components
            .Where(component=>!string.IsNullOrWhiteSpace(component.Designator))
            .ToDictionary(component=>component.Designator,StringComparer.Ordinal);

        if(measurementBindings.Select(binding=>binding.Sequence).Distinct().Count()!=measurementBindings.Count)
            errors.Add("Measurement binding sequences must be unique.");

        foreach(var binding in measurementBindings)
        {
            if(binding.Sequence<0)
                errors.Add($"Measurement binding {binding.Sequence} sequence must be non-negative.");
            if(!binding.ComponentId.IsValid)
                errors.Add($"Measurement binding {binding.Sequence} component id is invalid.");
            if(string.IsNullOrWhiteSpace(binding.Designator))
                errors.Add($"Measurement binding {binding.Sequence} designator cannot be blank.");
            if(binding.ComponentId.IsValid && !componentsById.ContainsKey(binding.ComponentId))
                errors.Add($"Measurement binding {binding.Sequence} component id is not present in the PCB assembly.");
            if(!string.IsNullOrWhiteSpace(binding.Designator) && !componentsByDesignator.ContainsKey(binding.Designator))
                errors.Add($"Measurement binding {binding.Sequence} designator is not present in the PCB assembly.");

            foreach(var value in new[]
            {
                binding.ProductionInputFingerprint,
                binding.CalibrationFingerprint,
                binding.ObservationFingerprint,
                binding.BindingFingerprint
            })
            {
                if(value is null || value.Length!=64 ||
                   !value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
                    errors.Add($"Measurement binding {binding.Sequence} contains a malformed fingerprint.");
            }
        }

        foreach(var binding in measurementBindings)
        {
            if(binding.ComponentId.IsValid &&
               componentsById.TryGetValue(binding.ComponentId,out var component) &&
               !string.Equals(component.Designator,binding.Designator,StringComparison.Ordinal))
            {
                errors.Add($"Measurement binding {binding.Sequence} component/designator identity is inconsistent.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        PcbAssemblySnapshot assembly,
        IReadOnlyList<ProductionMeasurementPcbBinding> measurementBindings)=>
        Validate(executionSnapshot,assembly,measurementBindings).Count==0;

    public static bool IsEquivalent(
        PcbExecutionMeasurementComponentBinding left,
        PcbExecutionMeasurementComponentBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
