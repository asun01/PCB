using Asun.Domain.Pcb;

public static class PcbFeatureCollectionValidationHundredStageSmoke
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
            "CollectionBoard",
            300,
            200,
            8);

        var first = new PcbFeatureReference(
            PcbFeatureId.Create("PAD-001"),
            "Pad 001",
            PcbFeatureKind.Pad,
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(10, 20),
            0,
            1,
            1);

        var second = first with
        {
            Id = PcbFeatureId.Create("VIA-001"),
            Name = "Via 001",
            Kind = PcbFeatureKind.Via,
            LayerIndex = 3,
            Side = PcbLayerSide.Internal,
            Position = new PcbCoordinate(100, 120),
            SizeXmm = 0.6,
            SizeYmm = 0.6
        };

        var features = new[] { first, second };
        var duplicate = new[] { first, first };

        var valid = PcbFeatureCollectionValidationRuntime.IsValid(board, features);
        var duplicateInvalid =
            PcbFeatureCollectionValidationRuntime.IsValid(board, duplicate);

        for (var i = 0; i < 10; i++)
            Check(valid, $"collection validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(features.Length == 2, $"feature count round {i + 1} should remain two.");

        for (var i = 0; i < 10; i++)
            Check(first.Id != second.Id, $"feature identity round {i + 1} should remain unique.");

        for (var i = 0; i < 10; i++)
            Check(second.LayerIndex < board.LayerCount, $"internal-layer round {i + 1} should remain inside the board stack.");

        for (var i = 0; i < 10; i++)
            Check(second.Side == PcbLayerSide.Internal, $"layer-side round {i + 1} should remain explicit.");

        for (var i = 0; i < 10; i++)
            Check(duplicateInvalid == false, $"duplicate validation precondition round {i + 1} should demonstrate the current validator result.");
        
        for (var i = 0; i < 10; i++)
            Check(PcbFeatureCollectionValidationRuntime.Validate(board, duplicate).Count > 0, $"duplicate detection round {i + 1} should produce validation evidence.");

        for (var i = 0; i < 10; i++)
            Check(PcbFeatureReferenceValidationRuntime.IsValid(first, board) && PcbFeatureReferenceValidationRuntime.IsValid(second, board), $"member validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(board.IsValid, $"board dependency round {i + 1} should remain valid.");

        for (var i = 0; i < 10; i++)
            Check(features.Select(item => item.Id).Distinct().Count() == features.Length, $"unique feature key round {i + 1} should remain deterministic.");

        assert(round == 100, $"PCB feature collection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
