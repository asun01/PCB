var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionRoiInteractionContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext5HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext5HundredStageSmoke.RunAsync(Check);


if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.RoiProductionIntegration smoke tests passed.");
return 0;
