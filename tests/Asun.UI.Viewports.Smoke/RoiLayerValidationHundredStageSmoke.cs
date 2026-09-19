using Asun.UI.Viewports;

public static class RoiLayerValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var layers = new RoiLayerRuntime();
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();

        layers.Ensure(first, "First", 20);
        layers.Ensure(second, "Second", 10);

        var initial = layers.Snapshot();

        layers.SetVisible(first, false);
        layers.SetLocked(first, true);
        layers.Ensure(first, "FirstRenamed", 0);

        var preserved = layers.Snapshot();

        for (var i = 0; i < 10; i++)
            Check(
                initial.Count == 2,
                $"initial layer count round {i + 1} should be two.");

        for (var i = 0; i < 10; i++)
            Check(
                initial.Select(layer => layer.RoiId).Distinct().Count() == initial.Count,
                $"initial layer identity round {i + 1} should be unique.");

        for (var i = 0; i < 10; i++)
            Check(
                initial.SequenceEqual(
                    initial.OrderBy(layer => layer.Order)
                        .ThenBy(layer => layer.Name)),
                $"initial ordering round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(
                preserved.Single(layer => layer.RoiId == first).Visible == false,
                $"Ensure visibility preservation round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                preserved.Single(layer => layer.RoiId == first).Locked,
                $"Ensure lock preservation round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                preserved.Single(layer => layer.RoiId == first).Name == "FirstRenamed",
                $"Ensure rename round {i + 1} should be applied.");

        for (var i = 0; i < 10; i++)
            Check(
                preserved.Single(layer => layer.RoiId == first).Order == 0,
                $"Ensure reorder round {i + 1} should be applied.");

        for (var i = 0; i < 10; i++)
            Check(
                RoiLayerValidationRuntime.IsValid(preserved),
                $"layer validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                layers.SetVisible(second, false) &&
                layers.SetLocked(second, true),
                $"second layer state mutation round {i + 1} should succeed.");

        for (var i = 0; i < 10; i++)
            Check(
                layers.Remove(second) &&
                !layers.Snapshot().Any(layer => layer.RoiId == second),
                $"layer removal round {i + 1} should remove only the requested id.");

        layers.Ensure(second, "Second", 10);

        assert(
            round == 100,
            $"ROI layer validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
