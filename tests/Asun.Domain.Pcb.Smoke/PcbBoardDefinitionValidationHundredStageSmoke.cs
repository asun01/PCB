using Asun.Domain.Pcb;

public static class PcbBoardDefinitionValidationHundredStageSmoke
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

        var center = board.Center;
        var invalidId = board with { BoardId = Guid.Empty };
        var invalidSize = board with { WidthMm = 0 };
        var invalidLayers = board with { LayerCount = 0 };

        for (var i = 0; i < 10; i++)
            Check(board.IsValid, $"board validity round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(board.WidthMm == 200 && board.HeightMm == 100, $"board dimensions round {i + 1} should remain explicit.");

        for (var i = 0; i < 10; i++)
            Check(board.LayerCount == 6, $"layer count round {i + 1} should remain six.");

        for (var i = 0; i < 10; i++)
            Check(center == new PcbCoordinate(100, 50), $"board center round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(PcbBoardDefinitionValidationRuntime.IsValid(board), $"board validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(!invalidId.IsValid, $"empty board-id round {i + 1} should fail.");

        for (var i = 0; i < 10; i++)
            Check(!invalidSize.IsValid, $"zero-width board round {i + 1} should fail.");

        for (var i = 0; i < 10; i++)
            Check(!invalidLayers.IsValid, $"zero-layer board round {i + 1} should fail.");

        for (var i = 0; i < 10; i++)
            Check(invalidId.Center == board.Center, $"record-geometry fallback round {i + 1} should remain deterministic without invoking IsValid.");

        for (var i = 0; i < 10; i++)
        {
            var invalidCenterThrows = false;
            try
            {
                _ = invalidSize.Center;
            }
            catch (InvalidOperationException)
            {
                invalidCenterThrows = true;
            }

            Check(invalidCenterThrows, $"invalid center access round {i + 1} should reject invalid board state.");
        }

        assert(round == 100, $"PCB board definition smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
