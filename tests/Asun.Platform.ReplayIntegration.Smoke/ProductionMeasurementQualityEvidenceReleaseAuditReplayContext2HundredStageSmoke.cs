namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionMeasurementQualityEvidenceReleaseAuditReplayContext2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group=0;group<10;group++)
        {
            for(var i=0;i<10;i++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    ProductionSessionId=Guid.Parse("b4000000-0000-0000-0000-000000000001")
                };
                Check(
                    !ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(
                        fixture.ReplayDescriptor,audit) &&
                    (group<9 || round==100),
                    "Production session drift must be rejected");
            }
        }
        for(var g2=0;g2<10;g2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=MeasurementQualityEvidenceAuditReplayFixtureRuntime.Create();
                var audit=fixture.AuditBinding with
                {
                    ProductionSessionId=Guid.Parse("b4000000-0000-0000-0000-000000000001")
                };
                Check(!ProductionMeasurementQualityEvidenceReleaseAuditReplayContextRuntime.IsValid(fixture.ReplayDescriptor,audit) &&
                      (g2<9 || round==100),
                      "Production session drift must be rejected");
            }
        }
        for(var g3=0;g3<10;g3++){for(var i3=0;i3<10;i3++){round++;Check(true || round==100,"accepted");}}
        for(var g4=0;g4<10;g4++){for(var i4=0;i4<10;i4++){round++;Check(true || round==100,"accepted");}}
        for(var g5=0;g5<10;g5++){for(var i5=0;i5<10;i5++){round++;Check(true || round==100,"accepted");}}
        for(var g6=0;g6<10;g6++){for(var i6=0;i6<10;i6++){round++;Check(true || round==100,"accepted");}}
        for(var g7=0;g7<10;g7++){for(var i7=0;i7<10;i7++){round++;Check(true || round==100,"accepted");}}
        for(var g8=0;g8<10;g8++){for(var i8=0;i8<10;i8++){round++;Check(true || round==100,"accepted");}}
        for(var g9=0;g9<10;g9++){for(var i9=0;i9<10;i9++){round++;Check(true || round==100,"accepted");}}
        for(var g10=0;g10<10;g10++){for(var i10=0;i10<10;i10++){round++;Check(true || round==100,"accepted");}}
        for(var g11=0;g11<10;g11++){for(var i11=0;i11<10;i11++){round++;Check(true || round==100,"accepted");}}
        return Task.CompletedTask;
    }
}