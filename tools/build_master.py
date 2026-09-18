from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
DOCS=ROOT/'docs'
OUT=DOCS/'MASTER_ALL_DOCUMENTS.md'
EXCLUDE={'README.md','MASTER_ALL_DOCUMENTS.md','REPOSITORY-TECHNICAL-BASELINE.md','OPEN-GATES.md','TASK-FS-001-BOOTSTRAP.md','PHASE1_BOOTSTRAP.md','PHASE1_PROGRESS.md'}
def docs():
    return sorted((p for p in DOCS.rglob('*.md') if p.name not in EXCLUDE), key=lambda p:p.relative_to(DOCS).as_posix())
def main():
    ps=['# Asun Vision Platform — MASTER ALL DOCUMENTS v2.1.5\n','> Convenience aggregation only. Individual documents are authoritative.\n']
    for p in docs():
        rel=p.relative_to(DOCS).as_posix()
        ps += [f'# ===== BEGIN {rel} =====\n', p.read_text(encoding='utf-8'), f'\n# ===== END {rel} =====\n']
    OUT.write_text('\n'.join(ps),encoding='utf-8')
    print(f'generated {len(docs())} documents')
if __name__=='__main__': main()
