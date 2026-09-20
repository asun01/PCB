using System.Security.Cryptography;
using System.Text;
using Asun.Device.Impl;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration;

public sealed record ProductionCaptureSessionReconciliation(
    Guid SessionId,
    int FrameCount,
    long FirstSequence,
    long LastSequence,
    string CaptureFingerprint,
    string ProductionFingerprint,
    string ReconciliationFingerprint);

public static class ProductionCaptureSessionReconciliationRuntime
{
    public static ProductionCaptureSessionReconciliation Create(
        ProductionSessionReport productionReport,
        CaptureSessionSnapshot captureSession)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(captureSession);

        var errors=Validate(productionReport,captureSession);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var fingerprints=string.Join("\n",captureSession.Fingerprints);
        var captureFingerprint=Hash(fingerprints);
        var canonical=string.Join("|",
            productionReport.SessionId,
            productionReport.FrameCount,
            captureSession.FirstSequence!.Value.Value,
            captureSession.LastSequence!.Value.Value,
            captureFingerprint,
            productionReport.Fingerprint);
        var reconciliationFingerprint=Hash(canonical);

        return new ProductionCaptureSessionReconciliation(
            productionReport.SessionId,
            productionReport.FrameCount,
            captureSession.FirstSequence.Value.Value,
            captureSession.LastSequence.Value.Value,
            captureFingerprint,
            productionReport.Fingerprint,
            reconciliationFingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        CaptureSessionSnapshot captureSession)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(captureSession);

        var errors=new List<string>();
        if(productionReport.SessionId==Guid.Empty)
            errors.Add("Production session id cannot be empty.");
        if(productionReport.FrameCount<=0)
            errors.Add("Production frame count must be positive.");
        if(captureSession.CapturedCount<=0)
            errors.Add("Capture session count must be positive.");
        if(captureSession.CapturedCount!=captureSession.Fingerprints.Count)
            errors.Add("Capture count must match fingerprint count.");
        if(captureSession.CapturedCount!=productionReport.FrameCount)
            errors.Add("Capture count must match production frame count.");
        if(captureSession.FirstSequence is null || captureSession.LastSequence is null)
            errors.Add("Capture session sequence bounds must be present.");

        var productionFrames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        if(productionFrames.Length!=productionReport.FrameCount)
            errors.Add("Production frame count must match production frame list.");

        if(captureSession.FirstSequence is not null &&
           captureSession.LastSequence is not null &&
           captureSession.FirstSequence.Value.Value>captureSession.LastSequence.Value.Value)
            errors.Add("Capture first sequence cannot exceed last sequence.");

        if(productionFrames.Length>0 &&
           captureSession.FirstSequence is not null &&
           captureSession.LastSequence is not null)
        {
            if(productionFrames.First().Sequence.Value!=captureSession.FirstSequence.Value.Value)
                errors.Add("Capture first sequence does not match production.");
            if(productionFrames.Last().Sequence.Value!=captureSession.LastSequence.Value.Value)
                errors.Add("Capture last sequence does not match production.");
        }

        if(productionFrames.Select(frame=>frame.Sequence.Value).Distinct().Count()!=productionFrames.Length)
            errors.Add("Production frame sequences must be unique.");

        var count=Math.Min(productionFrames.Length,captureSession.Fingerprints.Count);
        for(var index=0;index<count;index++)
        {
            if(string.IsNullOrWhiteSpace(captureSession.Fingerprints[index]))
                errors.Add($"Capture fingerprint {index} cannot be blank.");
            if(captureSession.Fingerprints[index]!=productionFrames[index].InputFingerprint)
                errors.Add($"Capture fingerprint {index} does not match production input fingerprint.");
        }

        if(string.IsNullOrWhiteSpace(productionReport.Fingerprint))
            errors.Add("Production report fingerprint cannot be blank.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        CaptureSessionSnapshot captureSession)=>
        Validate(productionReport,captureSession).Count==0;

    public static bool IsEquivalent(
        ProductionCaptureSessionReconciliation left,
        ProductionCaptureSessionReconciliation right)=>
        left.ReconciliationFingerprint==right.ReconciliationFingerprint;

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
