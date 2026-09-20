# Smoke Registry

| Smoke | Owner | Scope | Execution evidence |
|---|---|---|---|
| ProductionSessionProgressAcceptanceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Ready → Running progress → Completed; Cancelled; Failed; Reset; target count; sequence; frame metadata; adapter routing | Source registered; execution not claimed |

## Structural contract

- 10 loop groups
- 100 rounds per group
- explicit round==100
- 10 actual Check(...) call sites
- non-tautological assertions
- no TODO
- no NotImplementedException
