# Repository Tools

Automation validates repository structure and documentation integrity without changing authoritative semantics.

Current deterministic checks include solution/project layout, FunctionSpec/PageContract/ContractCandidate bindings, planning cross-links across DEV-PLAN-008/011/012/014, project dependency boundaries, open-gate reporting, and repository-authority path resolution/fingerprinting.

`validate_repository_authorities.py` reads `docs/00-baseline/REQUIRED-REPOSITORY-AUTHORITIES.md` as its sole registry source. Missing authorities remain governance gates; the tool reports them without inventing substitutes or changing gate state.

`generate_task_admission.py` derives per-FunctionSpec admission from the recorded open gates. It does not close gates or invent semantics; it explicitly distinguishes preparation-allowed work from production-blocked work.
