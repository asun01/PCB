using System.Security.Cryptography;
using System.Text;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiInteractionContext(
    Guid ProductionSessionId,
    int ProductionFrameCount,
    long FirstProductionSequence,
    long LastProductionSequence,
    string ProductionFingerprint,
    string RoiFingerprint,
    string InputRecoveryFingerprint,
    int RoiCount,
    Guid? SelectedRoiId,
    string BindingFingerprint);

public static class ProductionRoiInteractionContextRuntime
{
    public static ProductionRoiInteractionContext Create(
        ProductionSessionReport productionReport,
        ViewportRoiInputRecoverySnapshot roiSnapshot)
    {
        ArgumentNullException.ThrowIfNull(productionReport);

        var errors=Validate(productionReport,roiSnapshot);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var frames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var canonical=string.Join("|",
            productionReport.SessionId,
            productionReport.FrameCount,
            frames.First().Sequence.Value,
            frames.Last().Sequence.Value,
            productionReport.Fingerprint,
            roiSnapshot.RoiFingerprint,
            roiSnapshot.Fingerprint,
            roiSnapshot.Roi.Items.Count,
            roiSnapshot.Roi.Document.SelectedId);

        return new ProductionRoiInteractionContext(
            productionReport.SessionId,
            productionReport.FrameCount,
            frames.First().Sequence.Value,
            frames.Last().Sequence.Value,
            productionReport.Fingerprint,
            roiSnapshot.RoiFingerprint,
            roiSnapshot.Fingerprint,
            roiSnapshot.Roi.Items.Count,
            roiSnapshot.Roi.Document.SelectedId,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ViewportRoiInputRecoverySnapshot roiSnapshot)
    {
        ArgumentNullException.ThrowIfNull(productionReport);

        var errors=new List<string>();
        if(productionReport.SessionId==Guid.Empty)
            errors.Add("Production session id cannot be empty.");
        if(productionReport.FrameCount<=0 || productionReport.Frames.Count!=productionReport.FrameCount)
            errors.Add("Production frame count must match the frame collection.");

        var frames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        if(frames.Length>0)
        {
            if(frames.Select(frame=>frame.Sequence.Value).Distinct().Count()!=frames.Length)
                errors.Add("Production frame sequences must be unique.");
            if(frames.First().Sequence.Value<=0 || frames.Last().Sequence.Value<frames.First().Sequence.Value)
                errors.Add("Production frame sequence bounds are invalid.");
        }

        errors.AddRange(RoiDocumentValidationRuntime.ValidateSnapshot(roiSnapshot.Roi.Document));

        if(string.IsNullOrWhiteSpace(roiSnapshot.RoiFingerprint) ||
           roiSnapshot.RoiFingerprint.Length!=64 ||
           !IsLowerHex(roiSnapshot.RoiFingerprint))
            errors.Add("ROI fingerprint must be 64 lowercase hexadecimal characters.");

        if(string.IsNullOrWhiteSpace(roiSnapshot.Fingerprint) ||
           roiSnapshot.Fingerprint.Length!=64 ||
           !IsLowerHex(roiSnapshot.Fingerprint))
            errors.Add("ROI interaction fingerprint must be 64 lowercase hexadecimal characters.");

        if(string.IsNullOrWhiteSpace(productionReport.Fingerprint) ||
           productionReport.Fingerprint.Length!=64 ||
           !IsLowerHex(productionReport.Fingerprint))
            errors.Add("Production fingerprint must be 64 lowercase hexadecimal characters.");

        if(roiSnapshot.ProcessedEvents<0 || roiSnapshot.RejectedEvents<0)
            errors.Add("ROI input event counters cannot be negative.");

        if(roiSnapshot.Roi.Document.SelectedId is Guid selected &&
           roiSnapshot.Roi.Items.All(item=>item.Id!=selected))
            errors.Add("Selected ROI id must exist in the ROI snapshot.");

        if(roiSnapshot.Roi.Items.Select(item=>item.Id).Distinct().Count()!=roiSnapshot.Roi.Items.Count)
            errors.Add("ROI ids must be unique in the viewport snapshot.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ViewportRoiInputRecoverySnapshot roiSnapshot)=>
        Validate(productionReport,roiSnapshot).Count==0;

    public static bool IsEquivalent(
        ProductionRoiInteractionContext left,
        ProductionRoiInteractionContext right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.BindingFingerprint==right.BindingFingerprint;
    }

    private static bool IsLowerHex(string value)=>
        value.All(character=>Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
