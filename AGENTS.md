# Asun Vision Platform — Repository Agent Rules

## Fixed implementation baseline
- Visual Studio 2026
- C# / .NET 10
- WPF
- DevExpress 25.2.3
- HALCON 25.11
- AsunImage is not a project dependency and must not be introduced.
- Hardware-vendor SDKs are outside the current implementation scope; keep stable hardware interfaces/adapters and simulation boundaries only.

## Authority and no-guess policy
The individual documents under docs/ are the active engineering baseline. The 220 baseline documents are the source set; any future generated master is convenience only.
Mandatory read order: architecture → module guide → Contract/Schema → State/Owner/Authority → dependencies → PageContract → FunctionSpec → tests/benchmark/golden → current code.
Never invent HALCON operators/signatures/parameters/version behavior. Never invent DevExpress APIs. Never invent project facts or missing acceptance criteria. Missing facts become Blocker/Unverified/ContractCandidate and are recorded rather than used as guessed defaults.
Do not create duplicate schemas, owners, state machines, event catalogs, fact stores, universal domain objects, or production authorities.
UI, reports, statistics, and AI explanations are projections; they do not become a second production fact authority.
Compilation failure does not authorize changing a Contract. Do not lower quality gates, sampling, coverage, or acceptance criteria to make tests pass.
Scope is frozen by the applicable FunctionSpec/TaskCard/FilesToChange. Each completed slice must carry implementation, applicable tests, replay/golden evidence, benchmark evidence, and documentation/audit updates.
When an issue requires user/environment confirmation, record it as an external gate and continue with all non-blocked work.
