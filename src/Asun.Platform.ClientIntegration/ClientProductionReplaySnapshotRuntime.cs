using System.Security.Cryptography;
using System.Text;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientProductionReplaySnapshot(
    Guid ProgramId,
    Version ProgramVersion,
    Guid ProductionSessionId,
    ClientExecutionStatus Status,
    int FrameCount,
    string ProductionReportFingerprint,
    string ReplayFingerprint)
{
    public string? QualityFingerprint { get; init; }
};

public static class ClientProductionReplaySnapshotRuntime
{
    public static ClientProductionReplaySnapshot Create(
        ClientWorkspaceSnapshot workspace,
        ProductionSessionReport report,
        ClientQualityWorkspaceSnapshot quality)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(quality);

        var errors=Validate(workspace,report,quality);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var programId=workspace.ProgramId!.Value;
        var version=workspace.ProgramVersion!;
        var canonical=string.Join("|",
            programId,
            version,
            report.SessionId,
            report.FrameCount,
            report.Fingerprint,
            quality.Fingerprint ?? string.Empty,
            workspace.Status);

        return new ClientProductionReplaySnapshot(
            programId,
            version,
            report.SessionId,
            workspace.Status,
            report.FrameCount,
            report.Fingerprint,
            Hash(canonical))
        {
            QualityFingerprint=quality.Fingerprint
        };
    }

    public static IReadOnlyList<string> Validate(
        ClientWorkspaceSnapshot workspace,
        ProductionSessionReport report,
        ClientQualityWorkspaceSnapshot quality)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(quality);

        var errors=new List<string>();
        if(workspace.ProgramId is null || workspace.ProgramId==Guid.Empty)
            errors.Add("Client workspace Program identity must be present.");
        if(workspace.ProgramVersion is null)
            errors.Add("Client workspace Program version must be present.");
        if(workspace.ActiveSessionId is null || workspace.ActiveSessionId==Guid.Empty)
            errors.Add("Client workspace session identity must be present.");
        if(workspace.Status!=ClientExecutionStatus.Completed)
            errors.Add("Replay snapshot requires a completed client execution.");
        if(workspace.ActiveSessionId!=report.SessionId)
            errors.Add("Client workspace session identity must match the Production report.");
        if(workspace.LastFrameCount!=report.FrameCount)
            errors.Add("Client workspace frame count must match the Production report.");
        if(workspace.LastReportFingerprint!=report.Fingerprint)
            errors.Add("Client workspace report fingerprint must match the Production report.");
        if(report.FrameCount<=0)
            errors.Add("Production report frame count must be positive.");
        if(report.Fingerprint.Length!=64 || !IsLowerHex(report.Fingerprint))
            errors.Add("Production report fingerprint must be 64 lowercase hexadecimal characters.");
        if(!quality.IsBound || string.IsNullOrWhiteSpace(quality.Fingerprint))
            errors.Add("Replay snapshot requires an authoritative bound Quality result.");
        else if(!IsLowerHex(quality.Fingerprint))
            errors.Add("Quality fingerprint must be 64 lowercase hexadecimal characters.");

        return errors;
    }

    public static bool IsValid(
        ClientWorkspaceSnapshot workspace,
        ProductionSessionReport report,
        ClientQualityWorkspaceSnapshot quality)=>
        Validate(workspace,report,quality).Count==0;

    public static bool IsEquivalent(
        ClientProductionReplaySnapshot left,
        ClientProductionReplaySnapshot right)=>
        left.ReplayFingerprint==right.ReplayFingerprint;

    private static bool IsLowerHex(string value)=>
        value is not null &&
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
