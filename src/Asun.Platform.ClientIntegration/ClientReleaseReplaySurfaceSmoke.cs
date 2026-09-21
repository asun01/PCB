namespace Asun.Platform.ClientIntegration;

public static class ClientReleaseReplaySurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) EmptySurfaceIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) ReplayAvailabilityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReplayProgramIdentityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReplaySessionIdentityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReplayFrameCountIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReplayFingerprintIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReleaseAvailabilityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReleaseReadinessIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ArtifactPathIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ManifestFingerprintIsProjected();
    }

    private static void EmptySurfaceIsExplicit()
    {
        using var workspace=CreateWorkspace();
        var surface=ClientReleaseReplaySurfaceRuntime.Create(workspace.Capture());
        Check(!surface.ReplayAvailable &&
              !surface.ReleaseAvailable &&
              !surface.ReleaseReady,
            "Empty inspection state must expose Replay/Release as unavailable.");
    }

    private static void ReplayAvailabilityIsProjected()
    {
        var surface=CreateSurface();
        Check(surface.ReplayAvailable,"Replay availability must come from the existing Replay snapshot.");
    }

    private static void ReplayProgramIdentityIsProjected()
    {
        var programId=Guid.NewGuid();
        var surface=CreateSurface(programId);
        Check(surface.ReplayProgramId==programId,
            "Replay Program identity must be projected without recomputation.");
    }

    private static void ReplaySessionIdentityIsProjected()
    {
        var sessionId=Guid.NewGuid();
        var surface=CreateSurface(Guid.NewGuid(),sessionId);
        Check(surface.ReplayProductionSessionId==sessionId,
            "Replay Production session identity must be projected without recomputation.");
    }

    private static void ReplayFrameCountIsProjected()
    {
        var surface=CreateSurface(frameCount:7);
        Check(surface.ReplayFrameCount==7,
            "Replay frame count must use the existing Replay snapshot fact.");
    }

    private static void ReplayFingerprintIsProjected()
    {
        var fingerprint=new string('a',64);
        var surface=CreateSurface(replayFingerprint:fingerprint);
        Check(surface.ReplayFingerprint==fingerprint,
            "Replay fingerprint must be projected unchanged.");
    }

    private static void ReleaseAvailabilityIsProjected()
    {
        var surface=CreateSurface();
        Check(surface.ReleaseAvailable,
            "Release availability must come from the existing Release projection.");
    }

    private static void ReleaseReadinessIsProjected()
    {
        var surface=CreateSurface(releaseReady:true);
        Check(surface.ReleaseReady,
            "Release readiness must be projected from the existing Release projection.");
    }

    private static void ArtifactPathIsProjected()
    {
        var surface=CreateSurface(artifactPath:"release/artifact.bin");
        Check(surface.ReleaseArtifactPath=="release/artifact.bin",
            "Release artifact path must be projected unchanged.");
    }

    private static void ManifestFingerprintIsProjected()
    {
        var fingerprint=new string('b',64);
        var surface=CreateSurface(releaseManifestFingerprint:fingerprint);
        Check(surface.ReleaseManifestFingerprint==fingerprint,
            "Release manifest fingerprint must be projected unchanged.");
    }

    private static ClientReleaseReplaySurface CreateSurface(
        Guid? programId=null,
        Guid? sessionId=null,
        int frameCount=3,
        string? replayFingerprint=null,
        bool releaseReady=true,
        string artifactPath="release/artifact.bin",
        string? releaseManifestFingerprint=null)
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.Capture();
        var resolvedProgramId=programId ?? Guid.NewGuid();
        var resolvedSessionId=sessionId ?? Guid.NewGuid();
        var resolvedReplayFingerprint=replayFingerprint ?? new string('a',64);
        var resolvedManifestFingerprint=releaseManifestFingerprint ?? new string('b',64);
        var qualityFingerprint=new string('e',64);

        return ClientReleaseReplaySurfaceRuntime.Create(
            snapshot with
            {
                Quality=snapshot.Quality with
                {
                    IsBound=true,
                    Fingerprint=qualityFingerprint,
                    RunId=Guid.NewGuid()
                },
                Replay=new ClientProductionReplaySnapshot(
                    resolvedProgramId,
                    new Version(1,0),
                    resolvedSessionId,
                    ClientExecutionStatus.Completed,
                    frameCount,
                    new string('c',64),
                    resolvedReplayFingerprint)
                {
                    QualityFingerprint=qualityFingerprint
                },
                Release=new ClientReleaseProjection(
                    resolvedProgramId,
                    resolvedSessionId,
                    resolvedReplayFingerprint,
                    releaseReady,
                    artifactPath,
                    resolvedManifestFingerprint,
                    new string('d',64))
                {
                    QualityFingerprint=qualityFingerprint
                }
            });
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Release replay surface smoke failed: "+message);
    }
}
