using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Production.Runtime;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.SimulationIntegration;

public sealed class ClientSimulationQualityRunProvider :
    IClientQualityRunProvider
{
    public static ClientSimulationQualityRunProvider Instance { get; }=new();

    public ClientQualityProviderDescriptor Descriptor { get; }=
        new(
            "simulation-quality",
            "Deterministic Simulation Quality",
            true);

    public QualityInspectionRun Create(
        ProductionSessionReport productionReport)
    {
        ArgumentNullException.ThrowIfNull(productionReport);

        var results=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .Select(CreateResult)
            .ToArray();

        var run=new QualityInspectionRun(
            CreateGuid(
                "simulation-quality-run",
                productionReport.SessionId,
                productionReport.Fingerprint),
            results);

        if(!QualityInspectionRunValidationRuntime.IsValid(run))
            throw new InvalidOperationException(
                "Simulation Quality provider produced an invalid Quality run.");

        return run;
    }

    private static QualityInspectionResult CreateResult(
        ProductionFrameExecution frame)
    {
        var sequence=frame.Sequence.Value;
        var resultId=CreateGuid(
            "simulation-quality-result",
            sequence,
            frame.InputFingerprint);

        var snapshotId=CreateGuid(
            "simulation-quality-snapshot",
            sequence,
            frame.InputFingerprint);

        var findingId=QualityFindingId.Create(
            $"SIM-F-{sequence:0000}");

        var (outcome,severity,message)=sequence%3 switch
        {
            0=>(
                QualityOutcome.Fail,
                QualitySeverity.Critical,
                "Simulation outcome only: deterministic development fault case."),
            1=>(
                QualityOutcome.Pass,
                QualitySeverity.None,
                "Simulation outcome only: deterministic pass case."),
            _=>(
                QualityOutcome.Review,
                QualitySeverity.Warning,
                "Simulation outcome only: deterministic review case.")
        };

        var evidenceKey=QualityEvidenceKey.Create(
            $"simulation://quality/{sequence}/{frame.InputFingerprint[..12]}");

        var findings=new QualityFindingSet(new[]
        {
            new QualityFinding(
                findingId,
                "SIMULATION_FRAME_RESULT",
                outcome,
                severity,
                message)
        });

        var evidence=new QualityFindingEvidenceSet(new[]
        {
            new QualityFindingEvidenceLink(
                findingId,
                evidenceKey)
        });

        return new QualityInspectionResult(
            resultId,
            new QualityInspectionSnapshot(
                snapshotId,
                sequence,
                findings,
                evidence));
    }

    private static Guid CreateGuid(
        string prefix,
        params object[] values)
    {
        var canonical=prefix+"|"+string.Join("|",values);
        var hash=SHA256.HashData(
            Encoding.UTF8.GetBytes(canonical));

        hash[6]=(byte)((hash[6]&0x0F)|0x50);
        hash[8]=(byte)((hash[8]&0x3F)|0x80);

        return new Guid(hash.AsSpan(0,16));
    }
}
