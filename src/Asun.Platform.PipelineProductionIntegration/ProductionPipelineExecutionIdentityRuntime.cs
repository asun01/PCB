using System.Security.Cryptography;
using System.Text;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.PipelineProductionIntegration;

public sealed record ProductionPipelineExecutionIdentity(
    Guid SessionId,
    string ProgramFingerprint,
    string PipelineFingerprint,
    int FrameCount,
    string ProductionFingerprint,
    string ReplayAuditFingerprint,
    string Fingerprint);

public static class ProductionPipelineExecutionIdentityRuntime
{
    public static ProductionPipelineExecutionIdentity Create(
        ProductionSessionDefinition definition,
        ProductionSessionReport report)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        if(!ProductionSessionValidationRuntime.IsValid(definition,report))
            throw new ArgumentException("Production session is invalid.",nameof(report));

        var audit=ProductionPipelineReplayAuditRuntime.Create(definition,report);
        var pipelineFingerprint=CreatePipelineFingerprint(definition.Pipeline);
        var fingerprint=CreateFingerprint(
            report.SessionId,
            report.ProgramFingerprint,
            pipelineFingerprint,
            report.FrameCount,
            report.Fingerprint,
            audit.Fingerprint);

        return new ProductionPipelineExecutionIdentity(
            report.SessionId,
            report.ProgramFingerprint,
            pipelineFingerprint,
            report.FrameCount,
            report.Fingerprint,
            audit.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        ProductionPipelineExecutionIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(identity);

        var errors=new List<string>();
        if(!ProductionSessionValidationRuntime.IsValid(definition,report))
            errors.Add("Production session is invalid.");

        ProductionPipelineReplayAudit? audit=null;
        try
        {
            audit=ProductionPipelineReplayAuditRuntime.Create(definition,report);
        }
        catch(ArgumentException exception)
        {
            errors.Add(exception.Message);
        }

        var pipelineFingerprint=CreatePipelineFingerprint(definition.Pipeline);
        if(identity.SessionId!=report.SessionId)
            errors.Add("Execution identity session id must match.");
        if(identity.ProgramFingerprint!=report.ProgramFingerprint)
            errors.Add("Execution identity program fingerprint must match.");
        if(identity.PipelineFingerprint!=pipelineFingerprint)
            errors.Add("Execution identity pipeline fingerprint must match.");
        if(identity.FrameCount!=report.FrameCount)
            errors.Add("Execution identity frame count must match.");
        if(identity.ProductionFingerprint!=report.Fingerprint)
            errors.Add("Execution identity production fingerprint must match.");
        if(audit is not null && identity.ReplayAuditFingerprint!=audit.Fingerprint)
            errors.Add("Execution identity replay audit fingerprint must match.");
        if(identity.Fingerprint.Length!=64 || !identity.Fingerprint.All(Uri.IsHexDigit))
            errors.Add("Execution identity fingerprint must be 64 hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=CreateFingerprint(
            report.SessionId,
            report.ProgramFingerprint,
            pipelineFingerprint,
            report.FrameCount,
            report.Fingerprint,
            audit!.Fingerprint);
        if(expected!=identity.Fingerprint)
            errors.Add("Execution identity fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        ProductionPipelineExecutionIdentity identity)=>
        Validate(definition,report,identity).Count==0;

    internal static string CreatePipelineFingerprint(PipelineDefinition<CapturedFrame> pipeline)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        var canonical=string.Join(
            "|",
            pipeline.Stages
                .OrderBy(stage=>stage.Order)
                .ThenBy(stage=>stage.Name,StringComparer.Ordinal)
                .Select(stage=>$"{stage.Order}:{stage.Name.Length}:{stage.Name}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    internal static string CreateFingerprint(
        Guid sessionId,
        string programFingerprint,
        string pipelineFingerprint,
        int frameCount,
        string productionFingerprint,
        string replayAuditFingerprint)
    {
        var canonical=string.Join(
            "|",
            sessionId,
            programFingerprint,
            pipelineFingerprint,
            frameCount,
            productionFingerprint,
            replayAuditFingerprint);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }
}
