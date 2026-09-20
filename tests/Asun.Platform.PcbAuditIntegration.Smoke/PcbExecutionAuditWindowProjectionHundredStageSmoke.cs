using Asun.Domain.Quality;
using Asun.Platform.PcbAuditIntegration;
using Asun.Platform.PcbEvidenceEnvelopeIntegration;

public static class PcbExecutionAuditWindowProjectionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var envelope=new PcbProductionEvidenceEnvelope(
            new string('a',64),
            Guid.Parse("C8000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            new string('d',64),
            new string('e',64),
            new string('f',64));
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("C9000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("CA000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("CB000000-0000-0000-0000-000000000001"),
                        1,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var projection=PcbExecutionAuditWindowProjectionRuntime.Create(envelope,run);
        var tampered=projection with {AuditEnvelopeCount=0};
        var tamperedRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("C9000000-0000-0000-0000-000000000002"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("CC000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("CD000000-0000-0000-0000-000000000001"),
                        2,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });

        for(var i=0;i<10;i++) Check(run.ResultCount==1,"Audit source Quality run should contain one result.");
        for(var i=0;i<10;i++) Check(projection.QualityRunId==run.RunId,"Audit projection should retain Quality identity.");
        for(var i=0;i<10;i++) Check(projection.EvidenceEnvelopeFingerprint==envelope.Fingerprint,"Audit projection should retain envelope identity.");
        for(var i=0;i<10;i++) Check(projection.AuditEnvelopeCount==1,"Audit projection should report one audit envelope.");
        for(var i=0;i<10;i++) Check(projection.AuditWindowFingerprint.Length==64,"Audit window fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(projection.Fingerprint.Length==64,"Audit projection fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbExecutionAuditWindowProjectionValidationRuntime.IsValid(envelope,run,projection),"Audit window projection should validate.");
        for(var i=0;i<10;i++) Check(!PcbExecutionAuditWindowProjectionValidationRuntime.IsValid(envelope,run,tampered),"Audit count tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbExecutionAuditWindowProjectionValidationRuntime.IsValid(envelope,tamperedRun,projection),"Quality run identity/sequence drift should be rejected.");
        for(var i=0;i<10;i++) Check(run.RunId!=tamperedRun.RunId,"Tampered audit source should have a distinct identity.");

        assert(round==100,$"PCB audit window smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
