namespace Asun.Platform.ClientIntegration;

public sealed record ClientProgramSurface(
    ClientProgramWorkspaceSnapshot Snapshot,
    IReadOnlyList<ClientProgramDisplayItem> Items,
    ClientProgramDisplayItem? SelectedItem,
    string SelectionText);

public static class ClientProgramSurfaceRuntime
{
    public static ClientProgramSurface Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var program=snapshot.Program
            ?? new ClientProgramWorkspaceSnapshot(
                ClientProgramLoadStatus.Empty,
                null,
                null,
                null,
                0,
                null,
                Array.Empty<string>());

        var items=snapshot.ProgramItems;
        var selected=program.SelectedStepId is Guid stepId
            ? items.FirstOrDefault(item=>item.StepId==stepId)
            : null;

        var selectionText=selected is null
            ? "No Program step selected."
            : $"Selected step {selected.Order}: {selected.Name}";

        return new ClientProgramSurface(
            program,
            items,
            selected,
            selectionText);
    }
}
