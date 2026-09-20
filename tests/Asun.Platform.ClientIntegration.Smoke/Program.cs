var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ClientProductionWorkspace1HundredStageSmoke.RunAsync(Check);
await ClientProductionWorkspace2HundredStageSmoke.RunAsync(Check);
await ClientProductionWorkspace3HundredStageSmoke.RunAsync(Check);
await ClientProductionWorkspace4HundredStageSmoke.RunAsync(Check);
await ClientProductionWorkspace5HundredStageSmoke.RunAsync(Check);

await ClientProductionReplaySnapshot1HundredStageSmoke.RunAsync(Check);
await ClientProductionReplaySnapshot2HundredStageSmoke.RunAsync(Check);
await ClientProductionReplaySnapshot3HundredStageSmoke.RunAsync(Check);
await ClientProductionReplaySnapshot4HundredStageSmoke.RunAsync(Check);
await ClientProductionReplaySnapshot5HundredStageSmoke.RunAsync(Check);

await ClientReleaseProjection1HundredStageSmoke.RunAsync(Check);
await ClientReleaseProjection2HundredStageSmoke.RunAsync(Check);
await ClientReleaseProjection3HundredStageSmoke.RunAsync(Check);
await ClientReleaseProjection4HundredStageSmoke.RunAsync(Check);
await ClientReleaseProjection5HundredStageSmoke.RunAsync(Check);

await ClientProductionRunHistory1HundredStageSmoke.RunAsync(Check);
await ClientProductionRunHistory2HundredStageSmoke.RunAsync(Check);
await ClientProductionRunHistory3HundredStageSmoke.RunAsync(Check);
await ClientProductionRunHistory4HundredStageSmoke.RunAsync(Check);
await ClientProductionRunHistory5HundredStageSmoke.RunAsync(Check);

await ClientRoiInteractionWorkspace1HundredStageSmoke.RunAsync(Check);
await ClientRoiInteractionWorkspace2HundredStageSmoke.RunAsync(Check);
await ClientRoiInteractionWorkspace3HundredStageSmoke.RunAsync(Check);
await ClientRoiInteractionWorkspace4HundredStageSmoke.RunAsync(Check);
await ClientRoiInteractionWorkspace5HundredStageSmoke.RunAsync(Check);

await ClientInspectionWorkspace1HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspace2HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspace3HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspace4HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspace5HundredStageSmoke.RunAsync(Check);

await ClientInspectionWorkspaceControl1HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspaceControl2HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspaceControl3HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspaceControl4HundredStageSmoke.RunAsync(Check);
await ClientInspectionWorkspaceControl5HundredStageSmoke.RunAsync(Check);

await ClientInspectionDiagnostics1HundredStageSmoke.RunAsync(Check);
await ClientInspectionDiagnostics2HundredStageSmoke.RunAsync(Check);
await ClientInspectionDiagnostics3HundredStageSmoke.RunAsync(Check);
await ClientInspectionDiagnostics4HundredStageSmoke.RunAsync(Check);
await ClientInspectionDiagnostics5HundredStageSmoke.RunAsync(Check);

await ClientRunHistoryPresentation1HundredStageSmoke.RunAsync(Check);
await ClientRunHistoryPresentation2HundredStageSmoke.RunAsync(Check);
await ClientRunHistoryPresentation3HundredStageSmoke.RunAsync(Check);
await ClientRunHistoryPresentation4HundredStageSmoke.RunAsync(Check);
await ClientRunHistoryPresentation5HundredStageSmoke.RunAsync(Check);

await ClientCommandAvailability1HundredStageSmoke.RunAsync(Check);
await ClientCommandAvailability2HundredStageSmoke.RunAsync(Check);
await ClientCommandAvailability3HundredStageSmoke.RunAsync(Check);
await ClientCommandAvailability4HundredStageSmoke.RunAsync(Check);
await ClientCommandAvailability5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.ClientIntegration smoke tests passed.");
return 0;
