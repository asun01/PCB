using Asun.Release.Core;

public static class ReleaseManifestInvalidInputHundredStageSmoke
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
        var validArtifact=new ReleaseArtifact("app.exe",new string('a',64),100);
        var manifest=ReleaseManifestRuntime.Create(identity,new[]{validArtifact});
        var invalidArtifactManifest=manifest with
        {
            Artifacts=new[]{
                validArtifact with {Sha256="bad"}
            }
        };
        var invalidIdentityManifest=manifest with
        {
            Identity=new ReleaseIdentity("",new Version(1,0,0),"stable")
        };
        var invalidReadiness=ReleaseReadinessRuntime.Evaluate(invalidArtifactManifest);

        for(var i=0;i<10;i++) Check(!ReleaseManifestValidationRuntime.IsValid(invalidArtifactManifest),"Invalid artifact should be rejected without throwing.");
        for(var i=0;i<10;i++) Check(!ReleaseManifestValidationRuntime.IsValid(invalidIdentityManifest),"Invalid release identity should be rejected without throwing.");
        for(var i=0;i<10;i++) Check(!invalidReadiness.Ready,"Invalid manifest should not be release-ready.");
        for(var i=0;i<10;i++) Check(invalidReadiness.ArtifactCount==1,"Readiness should still report artifact cardinality.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==1,"Valid source manifest should remain unchanged.");
        for(var i=0;i<10;i++) Check(ReleaseManifestValidationRuntime.IsValid(manifest),"Valid source manifest should remain valid.");
        for(var i=0;i<10;i++) Check(manifest.Fingerprint.Length==64,"Source fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(validArtifact.IsValid,"Valid artifact source should remain valid.");
        for(var i=0;i<10;i++) Check(!invalidArtifactManifest.Artifacts[0].IsValid,"Invalid artifact shape should remain observable.");
        for(var i=0;i<10;i++) Check(!invalidIdentityManifest.Identity.IsValid,"Invalid identity shape should remain observable.");

        assert(round==100,$"Release invalid-input smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
