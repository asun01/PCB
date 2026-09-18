# Open External / Contract Gates

| Gate | State | Impact | Handling |
|---|---|---|---|
| GATE-001 repository authority gate | UNVERIFIED | Required for production-authority implementation | Record and proceed with non-authoritative scaffold/tooling. |
| C-01..C-08 exact approved Schema artifacts | CONTRACT_CANDIDATE | Affects exact DTO/persistence fields | Do not invent production schema; keep scaffold isolated. |
| DevExpress 25.2.3 local assembly/package resolution | ENVIRONMENT_GATE | Affects actual DevExpress references | UI shell remains vendor-neutral until verified. |
| HALCON 25.11 installed build/operator verification | ENVIRONMENT_GATE | Affects vision implementation | No guessed operators. |
| Hardware SDK contracts | DEFERRED | Concrete adapters | Keep ports and simulation boundaries only. |
| Automated test framework | UNVERIFIED | Package/test selection | Do not invent framework before frozen authority. |
