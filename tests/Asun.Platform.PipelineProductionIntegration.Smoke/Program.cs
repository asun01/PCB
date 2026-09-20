var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionPipelineReplayAuditHundredStageSmoke.RunAsync(Check);
await ProductionPipelineReleaseHandoffHundredStageSmoke.RunAsync(Check);
await ProductionPipelineExecutionIdentity1HundredStageSmoke.RunAsync(Check);
await ProductionPipelineExecutionIdentity2HundredStageSmoke.RunAsync(Check);
await ProductionPipelineExecutionIdentity3HundredStageSmoke.RunAsync(Check);
await ProductionPipelineExecutionIdentity4HundredStageSmoke.RunAsync(Check);
await ProductionPipelineExecutionIdentity5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.PipelineProductionIntegration smoke tests passed.");
return 0;
