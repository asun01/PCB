var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionSimulationReplayBindingHundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderReplayHundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderReplayDescriptorHundredStageSmoke.RunAsync(Check);
await ProductionSimulationReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await ProductionSimulationReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await ProductionSimulationReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await ProductionSimulationReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await ProductionSimulationReplayDescriptor5HundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderProvenance1HundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderProvenance2HundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderProvenance3HundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderProvenance4HundredStageSmoke.RunAsync(Check);
await ProductionSimulationRenderProvenance5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.SimulationIntegration smoke tests passed.");
return 0;
