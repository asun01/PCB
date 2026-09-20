var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await CaptureSessionProductionEvidenceBinding1HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionEvidenceBinding2HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionEvidenceBinding3HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionEvidenceBinding4HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionEvidenceBinding5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.DeviceProductionEvidenceIntegration smoke tests passed.");
return 0;
