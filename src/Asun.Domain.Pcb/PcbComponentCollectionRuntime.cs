namespace Asun.Domain.Pcb;

public static class PcbComponentCollectionRuntime
{
    public static IReadOnlyList<PcbComponentReference> Create(
        PcbBoardDefinition board,
        IEnumerable<PcbComponentReference> components)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(components);

        if(!board.IsValid)
            throw new ArgumentException("Component collection requires a valid board.",nameof(board));

        var materialized=components.ToArray();

        foreach(var component in materialized)
        {
            if(!PcbComponentReferenceValidationRuntime.IsValid(component,board))
                throw new ArgumentException("Component collection contains an invalid component.",nameof(components));
        }

        if(materialized.GroupBy(component=>component.Designator,StringComparer.Ordinal)
            .Any(group=>group.Count()>1))
        {
            throw new ArgumentException("Component designators must be unique.",nameof(components));
        }

        return materialized
            .OrderBy(component=>component.Designator,StringComparer.Ordinal)
            .ToArray();
    }
}
