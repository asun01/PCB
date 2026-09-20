var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await QualityReleaseFactProjectionHundredStageSmoke.RunAsync(Check);
await QualityReleaseReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await QualityReleaseReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await QualityReleaseReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await QualityReleaseReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await QualityReleaseReplayDescriptor5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.QualityReleaseIntegration smoke tests passed.");
return 0;
