using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Asun.Platform.PcbEvidenceReleaseIntegration;

public sealed record PcbEvidenceReleaseAuditTraceJsonEnvelope(
    int FormatVersion,
    PcbEvidenceReleaseAuditTrace Trace,
    string IntegrityHash);

public static class PcbEvidenceReleaseAuditTraceJsonRuntime
{
    public const int CurrentFormatVersion=1;

    public static string ToJson(PcbEvidenceReleaseAuditTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var errors=PcbEvidenceReleaseAuditTraceRuntime.ValidateStructure(trace);
        if(errors.Count>0)
            throw new InvalidOperationException(
                $"Cannot serialize invalid evidence release audit trace: {errors[0]}");

        var envelope=CreateEnvelope(trace);

        return JsonSerializer.Serialize(
            envelope,
            new JsonSerializerOptions
            {
                WriteIndented=false,
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase
            });
    }

    public static PcbEvidenceReleaseAuditTrace FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        PcbEvidenceReleaseAuditTraceJsonEnvelope? envelope;

        try
        {
            envelope=JsonSerializer.Deserialize<PcbEvidenceReleaseAuditTraceJsonEnvelope>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive=true
                });
        }
        catch(JsonException exception)
        {
            throw new InvalidOperationException(
                "Evidence release audit trace JSON is malformed.",
                exception);
        }

        if(envelope is null || envelope.Trace is null)
            throw new InvalidOperationException(
                "Evidence release audit trace JSON did not contain a valid envelope.");

        ValidateEnvelope(envelope);

        return envelope.Trace;
    }

    public static PcbEvidenceReleaseAuditTraceJsonEnvelope CreateEnvelope(
        PcbEvidenceReleaseAuditTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var errors=PcbEvidenceReleaseAuditTraceRuntime.ValidateStructure(trace);
        if(errors.Count>0)
            throw new ArgumentException(
                string.Join(" ",errors),
                nameof(trace));

        return new PcbEvidenceReleaseAuditTraceJsonEnvelope(
            CurrentFormatVersion,
            trace,
            CreateIntegrityHash(CurrentFormatVersion,trace.Fingerprint));
    }

    public static bool IsEnvelopeValid(
        PcbEvidenceReleaseAuditTraceJsonEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        try
        {
            ValidateEnvelope(envelope);
            return true;
        }
        catch(InvalidOperationException)
        {
            return false;
        }
    }

    private static void ValidateEnvelope(
        PcbEvidenceReleaseAuditTraceJsonEnvelope envelope)
    {
        if(envelope.FormatVersion!=CurrentFormatVersion)
            throw new InvalidOperationException(
                $"Unsupported evidence release audit trace format version '{envelope.FormatVersion}'.");

        var errors=PcbEvidenceReleaseAuditTraceRuntime.ValidateStructure(envelope.Trace);
        if(errors.Count>0)
            throw new InvalidOperationException(
                $"Evidence release audit trace failed structural validation: {errors[0]}");

        var expected=CreateIntegrityHash(
            envelope.FormatVersion,
            envelope.Trace.Fingerprint);

        if(!string.Equals(expected,envelope.IntegrityHash,StringComparison.Ordinal))
            throw new InvalidOperationException(
                "Evidence release audit trace integrity hash does not match.");
    }

    private static string CreateIntegrityHash(
        int formatVersion,
        string traceFingerprint)=>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    string.Join("|",formatVersion,traceFingerprint))))
            .ToLowerInvariant();
}
