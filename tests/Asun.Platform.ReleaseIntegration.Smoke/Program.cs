var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionEvidenceReleaseProjectionHundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.ReleaseIntegration smoke tests passed.");
return 0;
