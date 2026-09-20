var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await PcbPlacementQualityEvaluationHundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityBinding1HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityBinding2HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityBinding3HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityBinding4HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityBinding5HundredStageSmoke.RunAsync(Check);
await PcbPlacementQualityReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await PcbPlacementQualityReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await PcbPlacementQualityReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await PcbPlacementQualityReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await PcbPlacementQualityReplayDescriptor5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.QualityIntegration smoke tests passed.");
return 0;
