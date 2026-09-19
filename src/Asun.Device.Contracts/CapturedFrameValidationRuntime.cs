namespace Asun.Device.Contracts;

public static class CapturedFrameValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        CapturedFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var errors=new List<string>();

        if(!frame.Metadata.IsValid)
            errors.Add("Captured frame metadata must be valid.");

        if(frame.Payload.Length==0)
            errors.Add("Captured frame payload cannot be empty.");

        if(frame.PayloadFingerprint.Length!=64 ||
           !frame.PayloadFingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Captured frame fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else
        {
            var expected=CapturedFrame.Create(
                frame.Metadata,
                frame.Payload);

            if(expected.PayloadFingerprint!=frame.PayloadFingerprint)
                errors.Add("Captured frame fingerprint does not match the payload.");
        }

        return errors;
    }

    public static bool IsValid(
        CapturedFrame frame)=>
        Validate(frame).Count==0;
}
