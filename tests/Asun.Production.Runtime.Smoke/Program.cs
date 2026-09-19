var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionSessionHundredStageSmoke.RunAsync(Check);
await ProductionCancellationHundredStageSmoke.RunAsync(Check);
await ProductionReleaseCandidateHundredStageSmoke.RunAsync(Check);
await BoardProductionSessionHundredStageSmoke.RunAsync(Check);
await ProductionProgramPipelineBindingHundredStageSmoke.RunAsync(Check);
await ProductionQualityInspectionProjectionHundredStageSmoke.RunAsync(Check);
await ProductionEvidenceReferenceProjectionHundredStageSmoke.RunAsync(Check);
await ProductionFrameProvenanceHundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Production.Runtime smoke tests passed.");
return 0;