var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionCaptureEvidenceProjectionHundredStageSmoke.RunAsync(Check);
await ProductionCaptureEvidenceCanonical1HundredStageSmoke.RunAsync(Check);
await ProductionCaptureEvidenceCanonical2HundredStageSmoke.RunAsync(Check);
await ProductionCaptureEvidenceCanonical3HundredStageSmoke.RunAsync(Check);
await ProductionCaptureEvidenceCanonical4HundredStageSmoke.RunAsync(Check);
await ProductionCaptureEvidenceCanonical5HundredStageSmoke.RunAsync(Check);
await ProductionCaptureSessionReconciliation1HundredStageSmoke.RunAsync(Check);
await ProductionCaptureSessionReconciliation2HundredStageSmoke.RunAsync(Check);
await ProductionCaptureSessionReconciliation3HundredStageSmoke.RunAsync(Check);
await ProductionCaptureSessionReconciliation4HundredStageSmoke.RunAsync(Check);
await ProductionCaptureSessionReconciliation5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.CaptureEvidenceIntegration smoke tests passed.");
return 0;
