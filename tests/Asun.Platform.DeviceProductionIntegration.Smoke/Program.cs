var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await CaptureSessionProductionBinding1HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionBinding2HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionBinding3HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionBinding4HundredStageSmoke.RunAsync(Check);
await CaptureSessionProductionBinding5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.DeviceProductionIntegration smoke tests passed.");
return 0;
