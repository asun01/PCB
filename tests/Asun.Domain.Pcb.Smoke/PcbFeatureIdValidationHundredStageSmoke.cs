using Asun.Domain.Pcb;

public static class PcbFeatureIdValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var id = PcbFeatureId.Create("  PAD-001  ");
        var direct = new PcbFeatureId("VIA-002");
        var blank = new PcbFeatureId(" ");

        var blankRejected = false;
        try
        {
            _ = PcbFeatureId.Create(" ");
        }
        catch (ArgumentException)
        {
            blankRejected = true;
        }

        for (var i = 0; i < 10; i++)
            Check(id.IsValid && id.Value == "PAD-001", $"normalized id round {i + 1} should preserve a stable key.");

        for (var i = 0; i < 10; i++)
            Check(id.ToString() == "PAD-001", $"string identity round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(direct.IsValid && direct.Value == "VIA-002", $"direct id round {i + 1} should remain valid.");

        for (var i = 0; i < 10; i++)
            Check(!blank.IsValid, $"blank id round {i + 1} should be invalid.");

        for (var i = 0; i < 10; i++)
            Check(blankRejected, $"factory rejection round {i + 1} should reject blank ids.");

        for (var i = 0; i < 10; i++)
            Check(PcbFeatureIdValidationRuntime.IsValid(id), $"id validator round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(!PcbFeatureIdValidationRuntime.IsValid(blank), $"invalid id validator round {i + 1} should fail.");

        for (var i = 0; i < 10; i++)
            Check(id != direct, $"distinct key round {i + 1} should remain distinct.");

        for (var i = 0; i < 10; i++)
            Check(PcbFeatureId.Create(id.Value) == id, $"recreated id round {i + 1} should preserve identity.");

        for (var i = 0; i < 10; i++)
            Check(PcbFeatureId.Create("PAD-001").Equals(id), $"normalized equality round {i + 1} should remain stable.");

        assert(round == 100, $"PCB feature-id smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
