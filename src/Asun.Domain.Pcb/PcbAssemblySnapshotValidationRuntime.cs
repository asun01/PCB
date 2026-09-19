namespace Asun.Domain.Pcb;

public static class PcbAssemblySnapshotValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbAssemblySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors=new List<string>();

        if(!snapshot.Board.IsValid)
            errors.Add("Assembly snapshot board must be valid.");

        errors.AddRange(
            PcbFeatureCollectionValidationRuntime.Validate(
                snapshot.Board,
                snapshot.Components.Cast<PcbFeatureReference>().ToArray()));

        if(!snapshot.Components.SequenceEqual(
            snapshot.Components.OrderBy(component=>component.Designator,StringComparer.Ordinal)))
        {
            errors.Add("Assembly snapshot components must use canonical designator ordering.");
        }

        if(snapshot.Statistics.ComponentCount!=snapshot.Components.Count)
            errors.Add("Assembly snapshot statistics must match component count.");

        if(snapshot.Fingerprint.Length!=64 ||
           !snapshot.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Assembly snapshot fingerprint must be 64 lowercase hexadecimal characters.");
        }

        var expected=PcbAssemblySnapshotRuntime.Create(
            snapshot.Board,
            snapshot.Components);

        if(expected.Fingerprint!=snapshot.Fingerprint)
            errors.Add("Assembly snapshot fingerprint does not match the snapshot.");

        return errors;
    }

    public static bool IsValid(
        PcbAssemblySnapshot snapshot)=>
        Validate(snapshot).Count==0;
}
