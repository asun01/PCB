using Asun.Platform.Evidence;

public static class EvidenceDescriptorQueryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var valid=new EvidenceDescriptorQuery(EvidenceKind.Image,"image/raw");
        var byMediaType=new EvidenceDescriptorQuery(null,"image/raw");
        var invalid=new EvidenceDescriptorQuery(null,null);
        var invalidKind=new EvidenceDescriptorQuery((EvidenceKind)99,null);

        for(var i=0;i<10;i++) Check(valid.HasCriteria,"Kind and media type query should have criteria.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorQueryValidationRuntime.IsValid(valid),"Valid descriptor query should validate.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorQueryValidationRuntime.IsValid(byMediaType),"Media type query should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceDescriptorQueryValidationRuntime.IsValid(invalid),"Empty descriptor query should be rejected.");
        for(var i=0;i<10;i++) Check(!EvidenceDescriptorQueryValidationRuntime.IsValid(invalidKind),"Undefined query kind should be rejected.");
        for(var i=0;i<10;i++) Check(valid.Kind==EvidenceKind.Image,"Query kind should remain stable.");
        for(var i=0;i<10;i++) Check(valid.MediaType=="image/raw","Query media type should remain stable.");
        for(var i=0;i<10;i++) Check(byMediaType.Kind is null,"Media-only query should retain null kind.");
        for(var i=0;i<10;i++) Check(valid==new EvidenceDescriptorQuery(EvidenceKind.Image,"image/raw"),"Query equality should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorQueryValidationRuntime.Validate(valid).Count==0,"Valid query should produce no validation errors.");

        assert(round==100,$"Evidence descriptor query smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
