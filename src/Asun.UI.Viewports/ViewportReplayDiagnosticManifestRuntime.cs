using System.Text.Json;

namespace Asun.UI.Viewports;

public sealed record ViewportReplayDiagnosticManifest(
    int FormatVersion,
    string BundleJson,
    string ExecutionInputHash,
    string ExecutionResultHash,
    string FinalStateHash,
    string CheckpointInputHash,
    string CheckpointResultHash,
    string CheckpointStateHash,
    long CheckpointGeneration,
    string DiagnosticHash)
{
    public static int CurrentFormatVersion => 1;
}

public static class ViewportReplayDiagnosticManifestRuntime
{
    public static ViewportReplayDiagnosticManifest Create(
        ViewportReplayDiagnosticSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors =
            ViewportReplayDiagnosticSnapshotRuntime.Validate(
                snapshot);

        if (errors.Count != 0)
        {
            throw new InvalidOperationException(
                $"Cannot create diagnostic manifest: {errors[0]}");
        }

        return new ViewportReplayDiagnosticManifest(
            ViewportReplayDiagnosticManifest.CurrentFormatVersion,
            ViewportReplaySessionBundleRuntime.ToJson(
                snapshot.Bundle),
            snapshot.Execution.Execution.InputHash,
            snapshot.Execution.Execution.ResultHash,
            snapshot.Execution.FinalState.StateHash,
            snapshot.Checkpoint.InputHash,
            snapshot.Checkpoint.ResultHash,
            snapshot.Checkpoint.StateHash,
            snapshot.Checkpoint.Generation,
            snapshot.DiagnosticHash);
    }

    public static string ToJson(
        ViewportReplayDiagnosticManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        Validate(manifest);

        return JsonSerializer.Serialize(
            manifest,
            new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }

    public static ViewportReplayDiagnosticManifest FromJson(
        string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        ViewportReplayDiagnosticManifest? manifest;

        try
        {
            manifest =
                JsonSerializer.Deserialize<ViewportReplayDiagnosticManifest>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "Diagnostic manifest JSON is malformed.",
                exception);
        }

        if (manifest is null)
            throw new InvalidOperationException(
                "Diagnostic manifest JSON did not contain a manifest.");

        Validate(manifest);
        return manifest;
    }

    public static IReadOnlyList<string> Validate(
        ViewportReplayDiagnosticManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var errors = new List<string>();

        if (manifest.FormatVersion !=
            ViewportReplayDiagnosticManifest.CurrentFormatVersion)
        {
            errors.Add(
                $"Unsupported diagnostic manifest format version '{manifest.FormatVersion}'.");
        }

        if (string.IsNullOrWhiteSpace(manifest.BundleJson))
            errors.Add("Diagnostic manifest BundleJson must not be empty.");

        ValidateHash(
            errors,
            "ExecutionInputHash",
            manifest.ExecutionInputHash);
        ValidateHash(
            errors,
            "ExecutionResultHash",
            manifest.ExecutionResultHash);
        ValidateHash(
            errors,
            "FinalStateHash",
            manifest.FinalStateHash);
        ValidateHash(
            errors,
            "CheckpointInputHash",
            manifest.CheckpointInputHash);
        ValidateHash(
            errors,
            "CheckpointResultHash",
            manifest.CheckpointResultHash);
        ValidateHash(
            errors,
            "CheckpointStateHash",
            manifest.CheckpointStateHash);
        ValidateHash(
            errors,
            "DiagnosticHash",
            manifest.DiagnosticHash);

        if (manifest.CheckpointGeneration < 0)
            errors.Add(
                "Diagnostic manifest checkpoint generation must be non-negative.");

        if (errors.Count == 0)
        {
            try
            {
                _ = ViewportReplaySessionBundleRuntime.FromJson(
                    manifest.BundleJson);
            }
            catch (InvalidOperationException exception)
            {
                errors.Add(
                    $"Diagnostic manifest bundle is invalid: {exception.Message}");
            }
        }

        return errors;
    }

    public static bool IsValid(
        ViewportReplayDiagnosticManifest manifest) =>
        Validate(manifest).Count == 0;

    private static void ValidateHash(
        ICollection<string> errors,
        string name,
        string value)
    {
        if (value.Length != 64)
            errors.Add(
                $"{name} must be a SHA-256 length hash.");
    }
}
