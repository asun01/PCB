using System.Security.Cryptography;
using System.Text;
using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.PcbExecutionIntegration;

public sealed record PcbExecutionRoiContextBinding(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ProductionFingerprint,
    Guid QualityRunId,
    string EvidenceProjectionFingerprint,
    string PcbExecutionFingerprint,
    string RoiBindingFingerprint,
    string RoiInteractionFingerprint,
    int RoiCount,
    Guid? SelectedRoiId,
    string BindingFingerprint);

public static class PcbExecutionRoiContextBindingRuntime
{
    public static PcbExecutionRoiContextBinding Create(
        PcbExecutionSnapshot executionSnapshot,
        ProductionRoiInteractionContext roiContext)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(roiContext);

        var errors=Validate(executionSnapshot,roiContext);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.ProductionFingerprint,
            executionSnapshot.PipelineAuditFingerprint,
            executionSnapshot.FrameCount,
            executionSnapshot.MeasurementFactCount,
            executionSnapshot.QualityRunId,
            executionSnapshot.EvidenceProjectionFingerprint,
            executionSnapshot.Fingerprint,
            roiContext.RoiFingerprint,
            roiContext.InputRecoveryFingerprint,
            roiContext.RoiCount,
            roiContext.SelectedRoiId,
            roiContext.BindingFingerprint);

        return new PcbExecutionRoiContextBinding(
            executionSnapshot.AssemblyFingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.ProductionFingerprint,
            executionSnapshot.QualityRunId,
            executionSnapshot.EvidenceProjectionFingerprint,
            executionSnapshot.Fingerprint,
            roiContext.BindingFingerprint,
            roiContext.Fingerprint,
            roiContext.RoiCount,
            roiContext.SelectedRoiId,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        ProductionRoiInteractionContext roiContext)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(roiContext);

        var errors=new List<string>();
        errors.AddRange(ValidateSnapshot(executionSnapshot));
        errors.AddRange(ProductionRoiInteractionContextRuntime.ValidateBinding(roiContext));

        if(executionSnapshot.ProductionSessionId!=roiContext.ProductionSessionId)
            errors.Add("ROI context Production session id must match the PCB execution snapshot.");
        if(executionSnapshot.ProductionFingerprint!=roiContext.ProductionFingerprint)
            errors.Add("ROI context Production fingerprint must match the PCB execution snapshot.");
        if(executionSnapshot.FrameCount!=roiContext.ProductionFrameCount)
            errors.Add("ROI context Production frame count must match the PCB execution snapshot.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateBinding(
        PcbExecutionRoiContextBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();
        errors.AddRange(ValidateHex(binding.AssemblyFingerprint,nameof(binding.AssemblyFingerprint)));
        errors.AddRange(ValidateHex(binding.ProductionFingerprint,nameof(binding.ProductionFingerprint)));
        errors.AddRange(ValidateHex(binding.EvidenceProjectionFingerprint,nameof(binding.EvidenceProjectionFingerprint)));
        errors.AddRange(ValidateHex(binding.PcbExecutionFingerprint,nameof(binding.PcbExecutionFingerprint)));
        errors.AddRange(ValidateHex(binding.RoiBindingFingerprint,nameof(binding.RoiBindingFingerprint)));
        errors.AddRange(ValidateHex(binding.RoiInteractionFingerprint,nameof(binding.RoiInteractionFingerprint)));
        errors.AddRange(ValidateHex(binding.BindingFingerprint,nameof(binding.BindingFingerprint)));

        if(binding.ProductionSessionId==Guid.Empty)
            errors.Add("Bound Production session id cannot be empty.");
        if(binding.QualityRunId==Guid.Empty)
            errors.Add("Bound Quality run id cannot be empty.");
        if(binding.RoiCount<0)
            errors.Add("Bound ROI count cannot be negative.");
        if(binding.SelectedRoiId is Guid selected && selected==Guid.Empty)
            errors.Add("Bound selected ROI id cannot be empty.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        ProductionRoiInteractionContext roiContext)=>
        Validate(executionSnapshot,roiContext).Count==0;

    public static bool IsValidBinding(
        PcbExecutionRoiContextBinding binding)=>
        ValidateBinding(binding).Count==0;

    public static bool IsEquivalent(
        PcbExecutionRoiContextBinding left,
        PcbExecutionRoiContextBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static IReadOnlyList<string> ValidateSnapshot(
        PcbExecutionSnapshot snapshot)
    {
        var errors=new List<string>();
        if(snapshot.ProductionSessionId==Guid.Empty)
            errors.Add("PCB execution snapshot Production session id cannot be empty.");
        if(snapshot.FrameCount<=0)
            errors.Add("PCB execution snapshot frame count must be positive.");
        if(snapshot.MeasurementFactCount<0)
            errors.Add("PCB execution snapshot measurement fact count cannot be negative.");
        if(snapshot.QualityRunId==Guid.Empty)
            errors.Add("PCB execution snapshot Quality run id cannot be empty.");

        foreach(var pair in new[]
        {
            (nameof(snapshot.AssemblyFingerprint),snapshot.AssemblyFingerprint),
            (nameof(snapshot.ProductionFingerprint),snapshot.ProductionFingerprint),
            (nameof(snapshot.PipelineAuditFingerprint),snapshot.PipelineAuditFingerprint),
            (nameof(snapshot.EvidenceProjectionFingerprint),snapshot.EvidenceProjectionFingerprint),
            (nameof(snapshot.Fingerprint),snapshot.Fingerprint)
        })
        {
            errors.AddRange(ValidateHex(pair.Item2,pair.Item1));
        }

        return errors;
    }

    private static IReadOnlyList<string> ValidateHex(string value,string name)
    {
        if(value is null || value.Length!=64 || !value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c))
            return new[] { name+" must be 64 lowercase hexadecimal characters." };
        return Array.Empty<string>();
    }

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
