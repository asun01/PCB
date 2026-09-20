using Asun.Platform.ReplayIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class PcbExecutionRoiQualityReleaseAuditReplayClosure2HundredStageSmoke
{
    private static string H(char c)=>new(c,64);

    private static PcbExecutionRoiQualityReleaseReplayClosure RoiQualityRelease()
    {
        return new PcbExecutionRoiQualityReleaseReplayClosure(
            Guid.Parse("00000000-0000-0000-0000-000000000101"),
            Guid.Parse("00000000-0000-0000-0000-000000000202"),
            H('a'),
            H('f'),
            H('b'),
            true,
            "logical-artifact.pcb",
            H('e'),
            H('c'),
            H('d'));
    }

    private static ProductionMeasurementQualityEvidenceReleaseAuditReplayContext AuditContext(
        Guid? session=null,
        Guid? quality=null,
        string manifest=null!)
    {
        return new ProductionMeasurementQualityEvidenceReleaseAuditReplayContext(
            session ?? Guid.Parse("00000000-0000-0000-0000-000000000101"),
            quality ?? Guid.Parse("00000000-0000-0000-0000-000000000202"),
            1,
            H('1'),
            Guid.Parse("00000000-0000-0000-0000-000000000303"),
            H('2'),
            manifest ?? H('f'),
            1,
            H('c'),
            H('7'),
            H('8'));
    }

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var left=RoiQualityRelease();
            var audit=AuditContext(session:Guid.NewGuid());
            Check(PcbExecutionRoiQualityReleaseAuditReplayClosureRuntime.Validate(left,audit).Count>0,
                  "audit Production session drift must be rejected");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
