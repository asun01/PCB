namespace Asun.Platform.Pipeline;

public sealed record PipelineStage<T>(
    int Order,
    string Name,
    Func<T,T> Execute)
{
    public bool IsValid=>
        Order>0 &&
        !string.IsNullOrWhiteSpace(Name) &&
        Execute is not null;
}
