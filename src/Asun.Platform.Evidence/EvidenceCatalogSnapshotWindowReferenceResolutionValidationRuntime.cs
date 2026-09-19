namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowReferenceResolution result)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(result);

        var errors=new List<string>();
        errors.AddRange(EvidenceCatalogSnapshotWindowValidationRuntime.Validate(window));
        errors.AddRange(EvidenceReferenceSetValidationRuntime.Validate(result.ReferenceSet));

        var expectedWindowFingerprint=
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint;

        if(result.WindowFingerprint!=expectedWindowFingerprint)
            errors.Add("Window reference resolution fingerprint must match the source window.");

        if(result.Entries.Count!=window.Count)
            errors.Add("Window reference resolution must contain one entry per window snapshot.");

        var expectedBySequence=window.Entries
            .ToDictionary(entry=>entry.Sequence);

        var sequences=result.Entries.Select(entry=>entry.Sequence).ToArray();

        if(sequences.Distinct().Count()!=sequences.Length)
            errors.Add("Window reference resolution sequences must be unique.");

        if(!sequences.SequenceEqual(window.Entries.Select(entry=>entry.Sequence)))
            errors.Add("Window reference resolution sequences must match source window ordering.");

        foreach(var entry in result.Entries)
        {
            if(!expectedBySequence.TryGetValue(entry.Sequence,out var source))
            {
                errors.Add("Window reference resolution contains an unknown sequence.");
                continue;
            }

            errors.AddRange(
                EvidenceReferenceResolutionValidationRuntime.Validate(
                    source.Envelope.Snapshot,
                    result.ReferenceSet,
                    entry.Resolution));
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowReferenceResolution result)=>
        Validate(window,result).Count==0;
}
