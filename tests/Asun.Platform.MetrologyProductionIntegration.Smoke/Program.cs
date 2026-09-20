var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionMeasurementFactHundredStageSmoke.RunAsync(Check);
await ProductionMeasurementPcbBindingHundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementProvenanceBinding1HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementProvenanceBinding2HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementProvenanceBinding3HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementProvenanceBinding4HundredStageSmoke.RunAsync(Check);
await ProductionFrameMeasurementProvenanceBinding5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.MetrologyProductionIntegration smoke tests passed.");
return 0;
