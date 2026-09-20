var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await QualityEvidenceHandleProjectionHundredStageSmoke.RunAsync(Check);
await QualityFindingEvidenceResolutionHundredStageSmoke.RunAsync(Check);
await QualityFindingEvidenceReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await QualityFindingEvidenceReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await QualityFindingEvidenceReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await QualityFindingEvidenceReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await QualityFindingEvidenceReplayDescriptor5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.QualityEvidenceIntegration smoke tests passed.");
return 0;

await ProductionMeasurementQualityEvidenceBinding1HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceBinding2HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceBinding3HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceBinding4HundredStageSmoke.RunAsync(Check);
await ProductionMeasurementQualityEvidenceBinding5HundredStageSmoke.RunAsync(Check);
