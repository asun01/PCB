using Asun.Domain.Quality;

public static class QualityFindingIdValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var findingId = QualityFindingId.Create("  SPI-FAIL-001  ");
        var direct = new QualityFindingId("AOI-002");
        var blank = new QualityFindingId(" ");

        var blankRejected = false;
        try
        {
            _ = QualityFindingId.Create(" ");
        }
        catch (ArgumentException)
        {
            blankRejected = true;
        }

        for (var i = 0; i < 10; i++)
            Check(findingId.IsValid && findingId.Value == "SPI-FAIL-001", $"normalized finding id round {i + 1} should be stable.");

        for (var i = 0; i < 10; i++)
            Check(findingId.ToString() == "SPI-FAIL-001", $"finding id string round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(direct.IsValid && direct.Value == "AOI-002", $"direct finding id round {i + 1} should remain valid.");

        for (var i = 0; i < 10; i++)
            Check(!blank.IsValid, $"blank finding id round {i + 1} should be invalid.");

        for (var i = 0; i < 10; i++)
            Check(blankRejected, $"blank factory round {i + 1} should reject invalid input.");

        for (var i = 0; i < 10; i++)
            Check(QualityFindingIdValidationRuntime.IsValid(findingId), $"finding id validator round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(!QualityFindingIdValidationRuntime.IsValid(blank), $"invalid finding id validator round {i + 1} should fail.");

        for (var i = 0; i < 10; i++)
            Check(findingId != direct, $"finding identity round {i + 1} should remain unique.");

        for (var i = 0; i < 10; i++)
            Check(QualityFindingId.Create(findingId.Value) == findingId, $"recreated finding id round {i + 1} should preserve equality.");

        for (var i = 0; i < 10; i++)
            Check(QualityFindingId.Create("SPI-FAIL-001").Equals(findingId), $"normalized equality round {i + 1} should remain stable.");

        assert(round == 100, $"Quality finding-id smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
