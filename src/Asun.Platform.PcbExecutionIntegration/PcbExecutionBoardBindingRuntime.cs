using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;

namespace Asun.Platform.PcbExecutionIntegration;

public sealed record PcbExecutionBoardBinding(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ExecutionFingerprint,
    string BindingFingerprint);

public sealed record PcbExecutionBoardReplayDescriptor(
    string BindingFingerprint,
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ExecutionFingerprint);

public static class PcbExecutionBoardBindingRuntime
{
    public static PcbExecutionBoardBinding Create(
        PcbAssemblySnapshot assembly,
        PcbExecutionSnapshot executionSnapshot)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(executionSnapshot);

        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            throw new ArgumentException("PCB assembly snapshot is invalid.",nameof(assembly));
        if(executionSnapshot.AssemblyFingerprint!=assembly.Fingerprint)
            throw new ArgumentException("Execution snapshot must belong to the PCB assembly.",nameof(executionSnapshot));

        var fingerprint=CreateFingerprint(
            assembly.Fingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint);

        return new PcbExecutionBoardBinding(
            assembly.Fingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        PcbAssemblySnapshot assembly,
        PcbExecutionSnapshot executionSnapshot,
        PcbExecutionBoardBinding binding)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();
        if(!PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            errors.Add("PCB assembly snapshot is invalid.");
        if(executionSnapshot.AssemblyFingerprint!=assembly.Fingerprint)
            errors.Add("Execution snapshot must belong to the PCB assembly.");
        if(binding.AssemblyFingerprint!=assembly.Fingerprint)
            errors.Add("Board binding assembly fingerprint must match.");
        if(binding.ProductionSessionId!=executionSnapshot.ProductionSessionId)
            errors.Add("Board binding Production session id must match.");
        if(binding.ExecutionFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Board binding execution fingerprint must match.");

        if(binding.BindingFingerprint.Length!=64 ||
           !binding.BindingFingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
            errors.Add("Board binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=CreateFingerprint(
            assembly.Fingerprint,
            executionSnapshot.ProductionSessionId,
            executionSnapshot.Fingerprint);
        if(expected!=binding.BindingFingerprint)
            errors.Add("Board binding fingerprint does not match canonical content.");

        return errors;
    }

    public static string CreateCanonicalKey(
        PcbExecutionBoardBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(string.Join(
                    "|",
                    binding.AssemblyFingerprint,
                    binding.ProductionSessionId,
                    binding.ExecutionFingerprint))))
            .ToLowerInvariant();
    }

    public static bool IsEquivalent(
        PcbExecutionBoardBinding left,
        PcbExecutionBoardBinding right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return left.BindingFingerprint==right.BindingFingerprint;
    }

    public static PcbExecutionBoardReplayDescriptor CreateReplayDescriptor(
        PcbExecutionBoardBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        return new PcbExecutionBoardReplayDescriptor(
            binding.BindingFingerprint,
            binding.AssemblyFingerprint,
            binding.ProductionSessionId,
            binding.ExecutionFingerprint);
    }

    internal static string CreateFingerprint(
        string assemblyFingerprint,
        Guid productionSessionId,
        string executionFingerprint)
    {
        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(string.Join(
                    "|",
                    assemblyFingerprint,
                    productionSessionId,
                    executionFingerprint))))
            .ToLowerInvariant();
    }
}
