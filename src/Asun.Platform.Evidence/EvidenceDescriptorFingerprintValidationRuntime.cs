namespace Asun.Platform.Evidence;

public static class EvidenceDescriptorFingerprintValidationRuntime
{
    public static IReadOnlyList<string> ValidateFingerprint(
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors=new List<string>();

        if(fingerprint.Length!=64)
            errors.Add("Evidence descriptor fingerprint must be 64 hexadecimal characters.");

        if(fingerprint.Any(character=>!Uri.IsHexDigit(character)))
            errors.Add("Evidence descriptor fingerprint must contain only hexadecimal characters.");

        return errors;
    }

    public static bool IsValidFingerprint(string fingerprint)=>
        ValidateFingerprint(fingerprint).Count==0;

    public static IReadOnlyList<string> ValidateDescriptor(
        EvidenceDescriptor descriptor,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(fingerprint);

        var errors=EvidenceDescriptorValidationRuntime.Validate(descriptor).ToList();
        errors.AddRange(ValidateFingerprint(fingerprint));

        if(errors.Count==0)
        {
            var expected=EvidenceDescriptorFingerprintRuntime.CreateFingerprint(descriptor);

            if(!string.Equals(expected,fingerprint,StringComparison.Ordinal))
                errors.Add("Evidence descriptor fingerprint does not match the descriptor.");
        }

        return errors;
    }

    public static bool IsValidDescriptor(
        EvidenceDescriptor descriptor,
        string fingerprint)=>
        ValidateDescriptor(descriptor,fingerprint).Count==0;
}
