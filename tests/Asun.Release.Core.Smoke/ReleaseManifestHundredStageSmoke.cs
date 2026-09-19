using Asun.Release.Core;

public static class ReleaseManifestHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var identity=new ReleaseIdentity(
            "Asun PCB",
            new Version(1,0,0),
            "stable");
        var artifacts=new[]{
            new ReleaseArtifact("app.exe",new string('a',64),100),
            new ReleaseArtifact("config.json",new string('b',64),50)
        };
        var manifest=ReleaseManifestRuntime.Create(identity,artifacts);
        var invalid=manifest with {Fingerprint=new string('c',64)};

        for(var i=0;i<10;i++) Check(identity.IsValid,"Release identity should validate.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==2,"Manifest should retain two artifacts.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts[0].Path=="app.exe","Artifacts should use canonical path ordering.");
        for(var i=0;i<10;i++) Check(ReleaseManifestValidationRuntime.IsValid(manifest),"Release manifest should validate.");
        for(var i=0;i<10;i++) Check(!ReleaseManifestValidationRuntime.IsValid(invalid),"Tampered manifest should be rejected.");
        for(var i=0;i<10;i++) Check(manifest.Fingerprint.Length==64,"Release manifest fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(manifest.Fingerprint.All(Uri.IsHexDigit),"Release manifest fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(ReleaseManifestRuntime.Create(identity,artifacts)==manifest,"Manifest creation should be deterministic.");
        for(var i=0;i<10;i++) Check(ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Valid non-empty manifest should be release-ready.");
        for(var i=0;i<10;i++) Check(ReleaseReadinessRuntime.Evaluate(manifest).ArtifactCount==2,"Readiness should preserve artifact count.");

        assert(round==100,$"Release manifest smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
