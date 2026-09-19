using Asun.Platform.Evidence;

public static class EvidenceReferenceSetHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://002")
        });
        var invalid=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://002"),
            EvidenceHandle.Create("")
        });

        for(var i=0;i<10;i++) Check(EvidenceReferenceSetValidationRuntime.IsValid(referenceSet),"Canonical reference set should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceReferenceSetValidationRuntime.IsValid(invalid),"Invalid reference handle should be rejected.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles.Count==2,"Reference set count should remain two.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles[0].Value=="frame://001","Reference ordering should be canonical.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles[1].Value=="frame://002","Reference ordering should remain canonical.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles.Distinct().Count()==2,"Reference set handles should be unique.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles.All(handle=>handle.IsValid),"Reference set handles should remain valid.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceSetValidationRuntime.Validate(referenceSet).Count==0,"Canonical reference set should produce no errors.");
        for(var i=0;i<10;i++) Check(new EvidenceReferenceSet(referenceSet.Handles.ToArray())==referenceSet,"Reference set reconstruction should be deterministic.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles.SequenceEqual(referenceSet.Handles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)),"Reference handles should remain sorted.");

        assert(round==100,$"Evidence reference set smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
