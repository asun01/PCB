using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;

namespace Asun.Platform.QualityIntegration;

public sealed record PcbPlacementQualityReplayDescriptor(
    PcbFeatureId ComponentId,
    long Sequence,
    Guid ResultId,
    Guid SnapshotId,
    string EvaluationFingerprint,
    string DescriptorFingerprint);

public static class PcbPlacementQualityReplayDescriptorRuntime
{
    public static PcbPlacementQualityReplayDescriptor Create(PcbPlacementQualityEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        if(!PcbPlacementQualityEvaluationValidationRuntime.IsValid(evaluation))
            throw new ArgumentException("Placement quality evaluation is invalid.",nameof(evaluation));

        var result=evaluation.Result;
        var canonical=string.Join("|",
            evaluation.Observation.ComponentId.Value,
            result.Sequence,
            result.ResultId,
            result.SnapshotId,
            evaluation.Fingerprint);
        var fingerprint=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        return new PcbPlacementQualityReplayDescriptor(
            evaluation.Observation.ComponentId,
            result.Sequence,
            result.ResultId,
            result.SnapshotId,
            evaluation.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        PcbPlacementQualityEvaluation evaluation,
        PcbPlacementQualityReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        ArgumentNullException.ThrowIfNull(descriptor);
        var errors=new List<string>();
        if(!PcbPlacementQualityEvaluationValidationRuntime.IsValid(evaluation))
            errors.Add("Placement quality evaluation is invalid.");
        if(descriptor.ComponentId!=evaluation.Observation.ComponentId)
            errors.Add("Replay descriptor component identity must match.");
        if(descriptor.Sequence!=evaluation.Result.Sequence)
            errors.Add("Replay descriptor sequence must match.");
        if(descriptor.ResultId!=evaluation.Result.ResultId)
            errors.Add("Replay descriptor result identity must match.");
        if(descriptor.SnapshotId!=evaluation.Result.SnapshotId)
            errors.Add("Replay descriptor snapshot identity must match.");
        if(descriptor.EvaluationFingerprint!=evaluation.Fingerprint)
            errors.Add("Replay descriptor evaluation fingerprint must match.");
        if(descriptor.DescriptorFingerprint.Length!=64 || !descriptor.DescriptorFingerprint.All(Uri.IsHexDigit))
            errors.Add("Replay descriptor fingerprint must be 64 hexadecimal characters.");
        if(errors.Count>0)
            return errors;
        var expected=Create(evaluation);
        if(expected.DescriptorFingerprint!=descriptor.DescriptorFingerprint)
            errors.Add("Replay descriptor fingerprint does not match canonical content.");
        return errors;
    }

    public static bool IsValid(PcbPlacementQualityEvaluation evaluation,PcbPlacementQualityReplayDescriptor descriptor)=>
        Validate(evaluation,descriptor).Count==0;

    public static bool IsEquivalent(PcbPlacementQualityReplayDescriptor left,PcbPlacementQualityReplayDescriptor right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.DescriptorFingerprint==right.DescriptorFingerprint;
    }
}
