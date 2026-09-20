var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionRenderFrameProjectionHundredStageSmoke.RunAsync(Check);
await ProductionRenderReplayFrameIntegrityHundredStageSmoke.RunAsync(Check);
await ProductionCaptureRenderProvenanceHundredStageSmoke.RunAsync(Check);
await ProductionRenderEvidenceReferenceHundredStageSmoke.RunAsync(Check);
await ProductionRenderEvidenceReplayDescriptor1HundredStageSmoke.RunAsync(Check);
await ProductionRenderEvidenceReplayDescriptor2HundredStageSmoke.RunAsync(Check);
await ProductionRenderEvidenceReplayDescriptor3HundredStageSmoke.RunAsync(Check);
await ProductionRenderEvidenceReplayDescriptor4HundredStageSmoke.RunAsync(Check);
await ProductionRenderEvidenceReplayDescriptor5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.RenderIntegration smoke tests passed.");
return 0;
