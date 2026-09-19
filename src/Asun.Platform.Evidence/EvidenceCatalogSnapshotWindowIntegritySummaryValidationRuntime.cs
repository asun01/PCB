namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowIntegritySummary summary)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(summary);

        var errors=new List<string>(
            EvidenceCatalogSnapshotWindowValidationRuntime.Validate(window));

        if(summary.EntryCount!=window.Count)
            errors.Add("Window summary entry count must match the window.");

        var first=window.Count==0 ? null : window.Entries[0].Sequence;
        var last=window.Count==0 ? null : window.Entries[^1].Sequence;

        if(summary.FirstSequence!=first)
            errors.Add("Window summary first sequence must match the window.");
        if(summary.LastSequence!=last)
            errors.Add("Window summary last sequence must match the window.");

        if(summary.WindowFingerprint.Length!=64 ||
           !summary.WindowFingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Window summary fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint!=summary.WindowFingerprint)
        {
            errors.Add("Window summary fingerprint does not match the window.");
        }

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowIntegritySummary summary)=>
        Validate(window,summary).Count==0;
}
