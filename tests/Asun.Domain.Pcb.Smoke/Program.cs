var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

PcbCoordinateValidationHundredStageSmoke.Run(Check);
PcbFeatureIdValidationHundredStageSmoke.Run(Check);
PcbFeatureReferenceValidationHundredStageSmoke.Run(Check);
PcbFeatureCollectionValidationHundredStageSmoke.Run(Check);
PcbBoardDefinitionValidationHundredStageSmoke.Run(Check);
PcbComponentReferenceValidationHundredStageSmoke.Run(Check);
PcbComponentCollectionHundredStageSmoke.Run(Check);
PcbComponentLookupHundredStageSmoke.Run(Check);
PcbAssemblySnapshotHundredStageSmoke.Run(Check);
PcbPlacementObservationHundredStageSmoke.Run(Check);
PcbPlacementObservationSetHundredStageSmoke.Run(Check);
await CalibratedPcbPlacementObservationHundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Domain.Pcb smoke tests passed.");
return 0;
