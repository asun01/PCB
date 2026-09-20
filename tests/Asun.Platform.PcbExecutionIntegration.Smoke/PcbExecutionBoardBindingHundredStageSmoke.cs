using Asun.Domain.Pcb;
using Asun.Platform.PcbExecutionIntegration;

public static class PcbExecutionBoardBindingHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(
            Guid.Parse("D0000000-0000-0000-0000-000000000001"),
            "PCB-BOARD-001",
            100,
            80,
            4);
        var assembly=PcbAssemblySnapshotRuntime.Create(
            board,
            Array.Empty<PcbComponentReference>());
        var execution=new PcbExecutionSnapshot(
            assembly.Fingerprint,
            Guid.Parse("D1000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            1,
            1,
            Guid.Parse("D2000000-0000-0000-0000-000000000001"),
            new string('d',64),
            new string('e',64));
        var binding=PcbExecutionBoardBindingRuntime.Create(assembly,execution);
        var copy=binding with {};
        var tampered=binding with {ExecutionFingerprint=new string('f',64)};
        var invalidExecution=execution with {Fingerprint="bad"};
        var descriptor=PcbExecutionBoardBindingRuntime.CreateReplayDescriptor(binding);

        for(var i=0;i<10;i++) Check(binding.AssemblyFingerprint==assembly.Fingerprint && binding.ProductionSessionId!=Guid.Empty,"Binding should preserve valid PCB assembly and session identity.");
        for(var i=0;i<10;i++) Check(binding.ProductionSessionId==execution.ProductionSessionId,"Binding should preserve Production session identity.");
        for(var i=0;i<10;i++) Check(binding.ExecutionFingerprint==execution.Fingerprint,"Binding should preserve execution identity.");
        for(var i=0;i<10;i++) Check(binding.BindingFingerprint.Length==64,"Binding fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbExecutionBoardBindingRuntime.Validate(assembly,execution,binding).Count==0,"Valid board binding should validate.");
        for(var i=0;i<10;i++) Check(PcbExecutionBoardBindingRuntime.Validate(assembly,execution,tampered).Count>0,"Execution identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(PcbExecutionBoardBindingRuntime.CreateCanonicalKey(binding).Length==64,"Canonical board key should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbExecutionBoardBindingRuntime.IsEquivalent(binding,copy),"Equivalent board bindings should be recognized.");
        for(var i=0;i<10;i++) Check(!PcbExecutionBoardBindingRuntime.IsEquivalent(binding,tampered),"Tampered board bindings should not be equivalent.");
        for(var i=0;i<10;i++) Check(descriptor.ExecutionFingerprint==execution.Fingerprint && PcbExecutionBoardBindingRuntime.Validate(assembly,invalidExecution,binding).Count>0,"Replay descriptor should preserve execution identity and malformed execution should be rejected.");

        assert(round==100,$"PCB execution board binding smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
