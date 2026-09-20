var failures=new List<string>();

void Check(bool condition,string message)
{
    if(!condition)
        failures.Add(message);
}

await PcbEvidenceReleaseFactProjectionHundredStageSmoke.RunAsync(Check);
await PcbEvidenceReleaseAuditTrace1HundredStageSmoke.RunAsync(Check);
await PcbEvidenceReleaseAuditTrace2HundredStageSmoke.RunAsync(Check);
await PcbEvidenceReleaseAuditTrace3HundredStageSmoke.RunAsync(Check);
await PcbEvidenceReleaseAuditTrace4HundredStageSmoke.RunAsync(Check);
await PcbEvidenceReleaseAuditTrace5HundredStageSmoke.RunAsync(Check);

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.PcbEvidenceReleaseIntegration smoke tests passed.");
return 0;
