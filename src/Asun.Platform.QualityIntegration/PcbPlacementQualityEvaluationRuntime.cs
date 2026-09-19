using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;

namespace Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityEvaluationRuntime
{
    public static PcbPlacementQualityEvaluation Evaluate(
        PcbPlacementObservation observation,
        long sequence,
        Guid resultId,
        Guid snapshotId,
        Func<PcbPlacementObservation,QualityFinding> ruleEvaluator)
    {
        ArgumentNullException.ThrowIfNull(observation);
        ArgumentNullException.ThrowIfNull(ruleEvaluator);

        if(!PcbPlacementObservationValidationRuntime.IsValid(observation))
            throw new ArgumentException("PCB placement observation is invalid.",nameof(observation));

        if(sequence<0)
            throw new ArgumentOutOfRangeException(nameof(sequence));

        if(resultId==Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(resultId));

        if(snapshotId==Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(snapshotId));

        var finding=ruleEvaluator(observation);
        if(finding is null || !finding.IsValid)
            throw new ArgumentException("Placement quality rule must return a valid finding.",nameof(ruleEvaluator));

        var result=new QualityInspectionResult(
            resultId,
            new QualityInspectionSnapshot(
                snapshotId,
                sequence,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));

        var fingerprint=CreateFingerprint(
            observation,
            result);

        return new PcbPlacementQualityEvaluation(
            observation,
            result,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbPlacementObservation observation,
        QualityInspectionResult result)
    {
        var finding=result.Findings.Findings.Single();
        var builder=new StringBuilder();
        builder.Append(observation.ComponentId).Append('|')
            .Append(observation.Designator.Length).Append(':').Append(observation.Designator).Append('|')
            .Append(observation.MeasuredPosition.X.ToString("R")).Append('|')
            .Append(observation.MeasuredPosition.Y.ToString("R")).Append('|')
            .Append(observation.ErrorDistance.ToString("R")).Append('|')
            .Append(result.ResultId).Append('|')
            .Append(result.SnapshotId).Append('|')
            .Append(result.Sequence).Append('|')
            .Append(finding.Id).Append('|')
            .Append((int)finding.Outcome).Append('|')
            .Append((int)finding.Severity).Append('|')
            .Append(finding.RuleCode.Length).Append(':').Append(finding.RuleCode).Append('|')
            .Append(finding.Message.Length).Append(':').Append(finding.Message);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
