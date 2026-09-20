using System.Security.Cryptography;
using System.Text;
using Asun.Device.Impl;
using Asun.Production.Runtime;

namespace Asun.Platform.DeviceProductionIntegration;

public sealed record CaptureSessionProductionBinding(
    Guid ProductionSessionId,
    long CapturedCount,
    long FirstSequence,
    long LastSequence,
    string ProductionFingerprint,
    string CaptureInputFingerprint,
    string Fingerprint);

public static class CaptureSessionProductionBindingRuntime
{
    public static CaptureSessionProductionBinding Create(
        CaptureSessionSnapshot captureSession,
        ProductionSessionReport productionReport)
    {
        ArgumentNullException.ThrowIfNull(captureSession);
        ArgumentNullException.ThrowIfNull(productionReport);

        var errors=ValidateInputs(captureSession,productionReport);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(productionReport));

        var captureFingerprint=CreateCaptureInputFingerprint(captureSession);
        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            captureSession.CapturedCount,
            captureSession.FirstSequence!.Value,
            captureSession.LastSequence!.Value,
            productionReport.Fingerprint,
            captureFingerprint);

        return new CaptureSessionProductionBinding(
            productionReport.SessionId,
            captureSession.CapturedCount,
            captureSession.FirstSequence!.Value,
            captureSession.LastSequence!.Value,
            productionReport.Fingerprint,
            captureFingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        CaptureSessionSnapshot captureSession,
        ProductionSessionReport productionReport,
        CaptureSessionProductionBinding binding)
    {
        ArgumentNullException.ThrowIfNull(captureSession);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=ValidateInputs(captureSession,productionReport);
        if(binding.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Capture/Production binding session identity must match.");
        if(binding.CapturedCount!=captureSession.CapturedCount)
            errors.Add("Capture/Production binding frame count must match.");
        if(captureSession.FirstSequence is null || binding.FirstSequence!=captureSession.FirstSequence.Value)
            errors.Add("Capture/Production binding first sequence must match.");
        if(captureSession.LastSequence is null || binding.LastSequence!=captureSession.LastSequence.Value)
            errors.Add("Capture/Production binding last sequence must match.");
        if(binding.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Capture/Production binding Production fingerprint must match.");

        var captureFingerprint=CreateCaptureInputFingerprint(captureSession);
        if(binding.CaptureInputFingerprint!=captureFingerprint)
            errors.Add("Capture/Production binding input fingerprint must match.");
        if(binding.Fingerprint.Length!=64 || !IsLowerHex(binding.Fingerprint))
            errors.Add("Capture/Production binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                productionReport.SessionId,
                captureSession.CapturedCount,
                captureSession.FirstSequence!.Value,
                captureSession.LastSequence!.Value,
                productionReport.Fingerprint,
                captureFingerprint);

            if(expected!=binding.Fingerprint)
                errors.Add("Capture/Production binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        CaptureSessionSnapshot captureSession,
        ProductionSessionReport productionReport,
        CaptureSessionProductionBinding binding)=>
        Validate(captureSession,productionReport,binding).Count==0;

    internal static string CreateCaptureInputFingerprint(
        CaptureSessionSnapshot captureSession)
    {
        var canonical=string.Join(
            "|",
            captureSession.CapturedCount,
            captureSession.FirstSequence?.Value,
            captureSession.LastSequence?.Value,
            string.Join(
                ",",
                captureSession.Fingerprints));

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static string CreateFingerprint(
        Guid productionSessionId,
        long capturedCount,
        long firstSequence,
        long lastSequence,
        string productionFingerprint,
        string captureInputFingerprint)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            capturedCount,
            firstSequence,
            lastSequence,
            productionFingerprint,
            captureInputFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static List<string> ValidateInputs(
        CaptureSessionSnapshot captureSession,
        ProductionSessionReport productionReport)
    {
        var errors=new List<string>();

        if(captureSession.CapturedCount<=0)
            errors.Add("Capture session count must be positive.");
        if(captureSession.Fingerprints.Count!=captureSession.CapturedCount)
            errors.Add("Capture session fingerprint count must equal captured count.");
        if(captureSession.FirstSequence is null || captureSession.LastSequence is null)
            errors.Add("Capture session sequence boundaries are required.");
        if(productionReport.SessionId==Guid.Empty)
            errors.Add("Production session identity must be valid.");

        if(productionReport.Fingerprint.Length!=64 ||
           !IsLowerHex(productionReport.Fingerprint))
            errors.Add("Production report fingerprint must be valid.");

        if(productionReport.FrameCount<=0)
            errors.Add("Production report frame count must be positive.");

        if(productionReport.FrameCount!=captureSession.CapturedCount)
            errors.Add("Production frame count must match capture session count.");

        if(productionReport.Frames.Count!=captureSession.Fingerprints.Count)
            errors.Add("Production frame count and capture fingerprint count must match.");

        if(productionReport.Frames.Count==captureSession.Fingerprints.Count)
        {
            var orderedProductionFrames=productionReport.Frames
                .OrderBy(frame=>frame.Sequence.Value)
                .ToArray();

            if(orderedProductionFrames.Length>0 &&
               captureSession.FirstSequence is FrameSequence first &&
               first.Value!=orderedProductionFrames[0].Sequence.Value)
                errors.Add("Capture first sequence must match Production first sequence.");

            if(orderedProductionFrames.Length>0 &&
               captureSession.LastSequence is FrameSequence last &&
               last.Value!=orderedProductionFrames[^1].Sequence.Value)
                errors.Add("Capture last sequence must match Production last sequence.");

            for(var index=0;index<orderedProductionFrames.Length;index++)
            {
                var productionFrame=orderedProductionFrames[index];

                if(productionFrame.InputFingerprint!=captureSession.Fingerprints[index])
                    errors.Add($"Capture frame fingerprint at index {index} must match Production input fingerprint.");
            }
        }

        return errors;
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
