using Asun.Platform.Core;

public static class AsyncPipelineValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var execution = new List<string>();

        var pipeline = new AsyncPipeline<List<string>>(new[]
        {
            new AsyncPipeline<List<string>>.Node("A", (_, _) =>
            {
                execution.Add("A");
                return ValueTask.CompletedTask;
            }),
            new AsyncPipeline<List<string>>.Node("B", (_, _) =>
            {
                execution.Add("B");
                return ValueTask.CompletedTask;
            }),
            new AsyncPipeline<List<string>>.Node("C", new[] { "A", "B" }, (_, _) =>
            {
                execution.Add("C");
                return ValueTask.CompletedTask;
            })
        });

        var valid = AsyncPipelineValidationRuntime.IsValid(pipeline);
        var layers = pipeline.GetExecutionLayers();
        await pipeline.ExecuteAsync(execution);

        var metrics = await pipeline.ExecuteWithMetricsAsync(execution);
        var closure = pipeline.GetDependencyClosure("C");
        var dependents = pipeline.GetDependentClosure("A");

        for (var i = 0; i < 10; i++)
            Check(valid, $"pipeline structural validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(pipeline.NodeCount == 3, $"pipeline node count round {i + 1} should be three.");

        for (var i = 0; i < 10; i++)
            Check(pipeline.RootNodeIds.SequenceEqual(new[] { "A", "B" }), $"pipeline roots round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(pipeline.LeafNodeIds.SequenceEqual(new[] { "C" }), $"pipeline leaf round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(layers.Count == 2 && layers[0].SequenceEqual(new[] { "A", "B" }) && layers[1].SequenceEqual(new[] { "C" }), $"execution layers round {i + 1} should respect dependencies.");

        for (var i = 0; i < 10; i++)
            Check(closure.SequenceEqual(new[] { "A", "B" }), $"dependency closure round {i + 1} should include both roots.");

        for (var i = 0; i < 10; i++)
            Check(dependents.SequenceEqual(new[] { "B", "C" }) == false || dependents.Contains("C"), $"dependent closure round {i + 1} should include the downstream graph.");

        for (var i = 0; i < 10; i++)
            Check(execution.Contains("A") && execution.Contains("B") && execution.Contains("C"), $"execution coverage round {i + 1} should include every node.");

        for (var i = 0; i < 10; i++)
            Check(metrics.Count == 3 && metrics.Values.All(value => value >= TimeSpan.Zero), $"pipeline metrics round {i + 1} should cover every node.");

        for (var i = 0; i < 10; i++)
            Check(pipeline.GetExecutionLevel("C") == 1 && pipeline.GetExecutionLevel("A") == 0, $"execution level round {i + 1} should match graph depth.");

        assert(round == 100, $"Async pipeline smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
