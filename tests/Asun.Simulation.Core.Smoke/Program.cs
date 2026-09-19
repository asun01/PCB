var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

SimulationScenarioHundredStageSmoke.Run(Check);
SimulationObservationIntegrityHundredStageSmoke.Run(Check);
SimulationSessionHundredStageSmoke.Run(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Simulation.Core smoke tests passed.");
return 0;
