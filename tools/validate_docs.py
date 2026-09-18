from pathlib import Path
ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / 'docs'
AUX = {'README.md','MASTER_ALL_DOCUMENTS.md','REPOSITORY-TECHNICAL-BASELINE.md','OPEN-GATES.md','TASK-FS-001-BOOTSTRAP.md','PHASE1_BOOTSTRAP.md','PHASE1_PROGRESS.md'}

def main():
    docs=[p for p in DOCS.rglob('*.md') if p.name not in AUX]
    ids={}; errors=[]
    for p in docs:
        doc_id=None
        for line in p.read_text(encoding='utf-8').splitlines():
            if '文档 ID：' in line or 'Document ID:' in line:
                if '`' in line:
                    doc_id=line.split('`',2)[1]
                break
        if not doc_id:
            errors.append(f'missing Document ID: {p.relative_to(ROOT)}')
        elif doc_id in ids:
            errors.append(f'duplicate Document ID: {doc_id}')
        else:
            ids[doc_id]=p
    if len(docs)!=220: errors.append(f'expected 220 baseline docs, found {len(docs)}')
    if len(ids)!=220: errors.append(f'expected 220 unique Document IDs, found {len(ids)}')
    for e in errors: print('ERROR:',e)
    if errors: return 1
    print(f'OK: {len(docs)} baseline documents / {len(ids)} unique Document IDs')
    return 0

if __name__=='__main__': raise SystemExit(main())
