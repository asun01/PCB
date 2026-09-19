var failures = new List<string>();

void Assert(bool condition, string message)
{
    if (!condition)
        failures.Add(message);
}

PcbCoordinateValidationHundredStageSmoke.Run(Assert);
PcbFeatureIdValidationHundredStageSmoke.Run(Assert);
PcbBoardDefinitionValidationHundredStageSmoke.Run(Assert);
PcbFeatureReferenceValidationHundredStageSmoke.Run(Assert);
PcbFeatureCollectionValidationHundredStageSmoke.Run(Assert);

if (failures.Count > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Domain.Pcb smoke tests passed.");
return 0;
