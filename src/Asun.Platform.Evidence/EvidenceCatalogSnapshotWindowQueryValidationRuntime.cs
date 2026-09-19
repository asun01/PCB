namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowQueryResultSet resultSet)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(resultSet);

        var errors=new List<string>();

        if(!EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window))
            errors.AddRange(EvidenceCatalogSnapshotWindowValidationRuntime.Validate(window));

        if(!EvidenceDescriptorQueryValidationRuntime.IsValid(resultSet.Query))
            errors.AddRange(EvidenceDescriptorQueryValidationRuntime.Validate(resultSet.Query));

        var expectedWindowFingerprint=
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint;

        if(resultSet.WindowFingerprint!=expectedWindowFingerprint)
            errors.Add("Window query result set fingerprint must match the source window.");

        if(resultSet.EntryCount!=resultSet.Results.Count)
            errors.Add("Window query result set entry count must match the result count.");

        if(resultSet.Results.Count!=window.Entries.Count)
            errors.Add("Window query result set must contain one result per window entry.");

        var expectedEntries=window.Entries.ToDictionary(entry=>entry.Sequence);

        foreach(var item in resultSet.Results)
        {
            if(!expectedEntries.TryGetValue(item.Sequence,out var entry))
            {
                errors.Add("Window query result sequence must exist in the source window.");
                continue;
            }

            if(item.Result.Query!=resultSet.Query)
                errors.Add("Every window query result must retain the same query.");

            errors.AddRange(
                EvidenceCatalogQueryResultValidationRuntime.Validate(
                    entry.Envelope.Snapshot,
                    item.Result));
        }

        var resultSequences=resultSet.Results.Select(item=>item.Sequence).ToArray();
        if(resultSequences.Distinct().Count()!=resultSequences.Length)
            errors.Add("Window query result sequences must be unique.");

        if(!resultSequences.SequenceEqual(window.Entries.Select(entry=>entry.Sequence)))
            errors.Add("Window query result sequences must match the source window order.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowQueryResultSet resultSet)=>
        Validate(window,resultSet).Count==0;
}
