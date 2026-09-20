# PHASE1 70001-70500 INTEGRATION CHECKPOINT — 2026-09-20

## Closed product chain

**Client Workspace**
→ **Production Command Boundary**
→ **Production Runtime**
→ **Simulation Frame Source**
→ **WPF Client Feedback**

### New client integration

`Asun.Platform.ClientIntegration`

Core runtime:

`ClientProductionWorkspace`

The workspace provides an application-level command boundary over the existing `ProductionSessionRuntime`:

- Load a `ProductionSessionDefinition`;
- start through an explicit production runner port;
- propagate cancellation;
- reject concurrent start;
- retain client projection status;
- retain last Production report frame count/fingerprint;
- reset only the client projection and loaded definition.

It does not become a second Production fact authority.

### Simulation path

`ClientSimulationSessionFactory` is explicitly simulation-only.

The WPF shell exposes a development-only **Run Simulation** command. It exercises:

**WPF Shell → Client Workspace → Production Runtime → Simulated Frame Source → Production Report**

The UI labels this path as simulation and does not claim HALCON, hardware, or customer-production execution.

### Acceptance

Five exact-100-round ClientIntegration Smoke matrices were added for load, start, cancellation, reload, and reset behavior.

Static audit requirements are enforced:

- 10 loop groups;
- 10 actual Check call sites;
- explicit round==100;
- balanced delimiters;
- no TODO;
- no NotImplementedException;
- no tautological Check(true).

No authoritative build/test/CI success is claimed.
