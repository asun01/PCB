namespace Asun.Platform.Contracts;

/// <summary>
/// Identifies a unit entering a platform execution pipeline without imposing
/// production Owner/State semantics.
/// </summary>
public readonly record struct ExecutionRequestId(string Value)
{
    public override string ToString() => Value;
}

/// <summary>
/// Immutable request envelope for a non-domain-specific execution.
/// </summary>
public sealed record ExecutionRequest(
    ExecutionRequestId Id,
    DateTimeOffset RequestedAtUtc);

/// <summary>
/// Immutable timing data for an execution attempt.
/// </summary>
public sealed record ExecutionTiming(
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc)
{
    public TimeSpan Duration => CompletedAtUtc - StartedAtUtc;
}
