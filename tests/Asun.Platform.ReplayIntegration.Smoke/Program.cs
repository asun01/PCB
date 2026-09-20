var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}


await ProductionQualityEvidenceReplayBundleHundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReplayDescriptor5HundredStageSmoke.RunAsync(Check);

await ProductionQualityEvidenceReleaseReplayBinding1HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReleaseReplayBinding2HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReleaseReplayBinding3HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReleaseReplayBinding4HundredStageSmoke.RunAsync(Check);
await ProductionQualityEvidenceReleaseReplayBinding5HundredStageSmoke.RunAsync(Check);

await UnifiedReplayClosure1HundredStageSmoke.RunAsync(Check);
await UnifiedReplayClosure2HundredStageSmoke.RunAsync(Check);
await UnifiedReplayClosure3HundredStageSmoke.RunAsync(Check);
await UnifiedReplayClosure4HundredStageSmoke.RunAsync(Check);
await UnifiedReplayClosure5HundredStageSmoke.RunAsync(Check);

await ProductionMeasurementQualityEvidenceReleaseReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceReleaseReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceReleaseReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceReleaseReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceReleaseReplayDescriptor5HundredStageSmoke.RunAsync(Check);


if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.ReplayIntegration smoke tests passed.");
return 0;
