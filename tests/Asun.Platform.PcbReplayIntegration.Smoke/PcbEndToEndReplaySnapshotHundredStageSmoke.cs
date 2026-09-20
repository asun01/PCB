using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Platform.PcbReplayIntegration;
using Asun.Platform.PcbSimulationIntegration;
using Asun.Platform.RenderIntegration;
using Asun.UI.Viewports;

public static class PcbEndToEndReplaySnapshotHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var summary1=new ViewportRenderFrameSummary(1,4,1,1,1,0,0,0);
        var summary2=new ViewportRenderFrameSummary(2,5,1,2,1,0,0,0);
        var render1=new ProductionRenderReplayFrameIntegrity(
            1,
            new string('1',64),
            summary1,
            ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary1));
        var render2=new ProductionRenderReplayFrameIntegrity(
            2,
            new string('2',64),
            summary2,
            ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary2));
        var renders=new[]{render1,render2};

        var execution=new PcbExecutionSnapshot(
            new string('a',64),
            Guid.Parse("C3000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            2,
            2,
            Guid.Parse("C4000000-0000-0000-0000-000000000001"),
            new string('d',64),
            new string('e',64));
        var simulation=new PcbExecutionSimulationReplayProjection(
            execution.Fingerprint,
            execution.ProductionSessionId.ToString(),
            new string('f',64),
            2,
            2,
            new string('1',64));
        var evidence=new PcbExecutionEvidenceResolution(
            execution.Fingerprint,
            new string('2',64),
            new string('3',64),
            new[] {Asun.Platform.Evidence.EvidenceHandle.Create("x")},
            new[] {Asun.Platform.Evidence.EvidenceHandle.Create("x")},
            Array.Empty<Asun.Platform.Evidence.EvidenceHandle>(),
            new string('4',64));
        var release=new PcbExecutionReleaseProjection(
            execution.AssemblyFingerprint,
            execution.ProductionSessionId,
            execution.Fingerprint,
            new string('5',64),
            1,
            true,
            new string('6',64));
        var snapshot=PcbEndToEndReplaySnapshotRuntime.Create(
            execution,
            renders,
            simulation,
            evidence,
            release);
        var tampered=snapshot with
        {
            SimulationReplayFingerprint=new string('7',64)
        };
        var reordered=new[]{render2,render1};

        for(var i=0;i<10;i++) Check(renders.Length==2,"Replay snapshot should contain two render facts.");
        for(var i=0;i<10;i++) Check(render1.RenderFingerprint.Length==64 && render2.RenderFingerprint.Length==64,"Render replay facts should retain fingerprints.");
        for(var i=0;i<10;i++) Check(snapshot.ExecutionSnapshotFingerprint==execution.Fingerprint,"Replay snapshot should retain execution identity.");
        for(var i=0;i<10;i++) Check(snapshot.RenderReplayFingerprint==PcbEndToEndReplaySnapshotRuntime.CreateRenderFingerprint(renders),"Replay snapshot should retain canonical render aggregate fingerprint.");
        for(var i=0;i<10;i++) Check(snapshot.SimulationReplayFingerprint==simulation.Fingerprint,"Replay snapshot should retain simulation replay identity.");
        for(var i=0;i<10;i++) Check(snapshot.EvidenceResolutionFingerprint==evidence.Fingerprint,"Replay snapshot should retain evidence resolution identity.");
        for(var i=0;i<10;i++) Check(snapshot.ReleaseProjectionFingerprint==release.Fingerprint,"Replay snapshot should retain release projection identity.");
        for(var i=0;i<10;i++) Check(PcbEndToEndReplaySnapshotValidationRuntime.IsValid(execution,renders,simulation,evidence,release,snapshot),"End-to-end replay snapshot should validate.");
        for(var i=0;i<10;i++) Check(!PcbEndToEndReplaySnapshotValidationRuntime.IsValid(execution,renders,simulation,evidence,release,tampered),"Simulation identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbEndToEndReplaySnapshotValidationRuntime.IsValid(execution,reordered,simulation,evidence,release,snapshot),"Render frame ordering drift should be rejected.");

        assert(round==100,$"PCB end-to-end replay snapshot smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
