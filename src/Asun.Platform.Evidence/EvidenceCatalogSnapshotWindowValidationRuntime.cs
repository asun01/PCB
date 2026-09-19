namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        var errors=new List<string>();
        long? previousSequence=null;
        var sequences=new HashSet<long>();

        foreach(var entry in window.Entries)
        {
            if(entry.Sequence<=0)
                errors.Add("Evidence snapshot window sequences must be positive.");

            if(!sequences.Add(entry.Sequence))
                errors.Add("Evidence snapshot window sequences must be unique.");

            if(previousSequence is not null && entry.Sequence<=previousSequence.Value)
                errors.Add("Evidence snapshot window entries must be ordered by ascending sequence.");

            previousSequence=entry.Sequence;
            errors.AddRange(EvidenceCatalogSnapshotEnvelopeValidationRuntime.Validate(entry.Envelope));
        }

        return errors;
    }

    public static bool IsValid(EvidenceCatalogSnapshotWindow window)=>
        Validate(window).Count==0;
}
