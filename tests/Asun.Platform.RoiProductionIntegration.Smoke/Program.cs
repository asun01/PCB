var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await ProductionRoiInteractionContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiInteractionContext5HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderReplayContext5HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderEvidenceReplayContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderEvidenceReplayContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderEvidenceReplayContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderEvidenceReplayContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderEvidenceReplayContext5HundredStageSmoke.RunAsync(Check);
await ProductionRoiMeasurementQualityContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiMeasurementQualityContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiMeasurementQualityContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiMeasurementQualityContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiMeasurementQualityContext5HundredStageSmoke.RunAsync(Check);
await ProductionRoiQualityEvidenceReplayContext1HundredStageSmoke.RunAsync(Check);
await ProductionRoiQualityEvidenceReplayContext2HundredStageSmoke.RunAsync(Check);
await ProductionRoiQualityEvidenceReplayContext3HundredStageSmoke.RunAsync(Check);
await ProductionRoiQualityEvidenceReplayContext4HundredStageSmoke.RunAsync(Check);
await ProductionRoiQualityEvidenceReplayContext5HundredStageSmoke.RunAsync(Check);





if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.RoiProductionIntegration smoke tests passed.");
return 0;

await ProductionRoiRenderFrameProvenanceBinding1HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderFrameProvenanceBinding2HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderFrameProvenanceBinding3HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderFrameProvenanceBinding4HundredStageSmoke.RunAsync(Check);
await ProductionRoiRenderFrameProvenanceBinding5HundredStageSmoke.RunAsync(Check);
