using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualityFindingDisplayItem(
    string FindingId,
    string RuleCode,
    string Outcome,
    string Severity,
    string Message,
    int EvidenceCount);

public sealed record ClientQualityWorkspaceSnapshot(
    Guid? RunId,
    int ResultCount,
    int FindingCount,
    int PassCount,
    int FailCount,
    int ReviewCount,
    int EvidenceLinkCount,
    string? Fingerprint,
    bool IsBound,
    IReadOnlyList<ClientQualityFindingDisplayItem> Findings);

public sealed class ClientQualityWorkspace
{
    private QualityInspectionRun? _run;

    public ClientQualityWorkspaceSnapshot Capture()
    {
        if(_run is null)
            return new ClientQualityWorkspaceSnapshot(
                null,0,0,0,0,0,0,null,false,
                Array.Empty<ClientQualityFindingDisplayItem>());

        if(!QualityInspectionRunValidationRuntime.IsValid(_run))
            throw new InvalidOperationException("Bound Quality inspection run is invalid.");

        var findings=_run.Results
            .OrderBy(result=>result.Sequence)
            .ThenBy(result=>result.SnapshotId)
            .ThenBy(result=>result.ResultId)
            .SelectMany(result=>result.Findings.Findings.Select(finding=>
                new ClientQualityFindingDisplayItem(
                    finding.Id.Value,
                    finding.RuleCode,
                    finding.Outcome.ToString(),
                    finding.Severity.ToString(),
                    finding.Message,
                    result.Evidence.ForFinding(finding.Id).Count)))
            .ToArray();

        var resultCount=_run.ResultCount;
        var findingCount=findings.Length;
        var passCount=findings.Count(item=>item.Outcome==QualityOutcome.Pass.ToString());
        var failCount=findings.Count(item=>item.Outcome==QualityOutcome.Fail.ToString());
        var reviewCount=findings.Count(item=>item.Outcome==QualityOutcome.Review.ToString());
        var evidenceCount=findings.Sum(item=>item.EvidenceCount);

        var canonical=string.Join(
            "|",
            _run.RunId,
            resultCount,
            findingCount,
            passCount,
            failCount,
            reviewCount,
            evidenceCount,
            string.Join("
",findings.Select(item=>
                string.Join("|",
                    item.FindingId,
                    item.RuleCode,
                    item.Outcome,
                    item.Severity,
                    item.Message,
                    item.EvidenceCount))));

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();

        return new ClientQualityWorkspaceSnapshot(
            _run.RunId,
            resultCount,
            findingCount,
            passCount,
            failCount,
            reviewCount,
            evidenceCount,
            fingerprint,
            true,
            findings);
    }

    public void Bind(QualityInspectionRun run)
    {
        ArgumentNullException.ThrowIfNull(run);
        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            throw new ArgumentException("Quality inspection run is invalid.",nameof(run));

        _run=run;
    }

    public void Clear() => _run=null;
}
