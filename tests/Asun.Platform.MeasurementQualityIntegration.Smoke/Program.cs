var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await MeasurementQualityEvaluationHundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementQualityProvenanceBinding1HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementQualityProvenanceBinding2HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementQualityProvenanceBinding3HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementQualityProvenanceBinding4HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementQualityProvenanceBinding5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.MeasurementQualityIntegration smoke tests passed.");
return 0;
