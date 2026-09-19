using Asun.Domain.Pcb;

public static class PcbFeatureReferenceValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var board = new PcbBoardDefinition(
            Guid.NewGuid(),
            "DemoBoard",
            200,
            100,
            6);

        var feature = new PcbFeatureReference(
            PcbFeatureId.Create("PAD-001"),
            "Pad 001",
            PcbFeatureKind.Pad,
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(25, 30),
            0.5,
            1.2,
            0.8);

        var invalidKind = feature with { Kind = PcbFeatureKind.Unknown };
        var invalidLayer = feature with { LayerIndex = 6 };
        var invalidSize = feature with { SizeXmm = -1 };
        var validation =
            PcbFeatureReferenceValidationRuntime.Validate(feature, board);

        for (var i = 0; i < 10; i++)
            Check(feature.IsValid, $"feature validity round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(validation.Count == 0, $"feature validator round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(feature.Position == new PcbCoordinate(25, 30), $"feature position round {i + 1} should remain board-space stable.");

        for (var i = 0; i < 10; i++)
            Check(feature.LayerIndex == 0 && feature.Side == PcbLayerSide.Top, $"feature layer round {i + 1} should remain explicit.");

        for (var i = 0; i < 10; i++)
            Check(feature.Kind == PcbFeatureKind.Pad, $"feature kind round {i + 1} should remain typed.");

        for (var i = 0; i < 10; i++)
            Check(!invalidKind.IsValid && !PcbFeatureReferenceValidationRuntime.IsValid(invalidKind, board), $"unknown-kind round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(!PcbFeatureReferenceValidationRuntime.IsValid(invalidLayer, board), $"out-of-board-layer round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(!invalidSize.IsValid, $"negative-size round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(feature.SizeXmm > 0 && feature.SizeYmm > 0, $"feature size round {i + 1} should remain positive for this pad reference.");

        for (var i = 0; i < 10; i++)
            Check(board.LayerCount == 6 && feature.LayerIndex < board.LayerCount, $"board-layer relation round {i + 1} should remain valid.");

        assert(round == 100, $"PCB feature reference smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
