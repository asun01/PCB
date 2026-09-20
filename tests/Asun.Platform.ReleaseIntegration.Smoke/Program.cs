var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionEvidenceReleaseProjectionHundredStageSmoke.RunAsync(Check);
await ProductionReleaseReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await ProductionReleaseReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await ProductionReleaseReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await ProductionReleaseReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await ProductionReleaseReplayDescriptor5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.ReleaseIntegration smoke tests passed.");
return 0;
