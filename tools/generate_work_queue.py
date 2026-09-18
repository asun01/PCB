from __future__ import annotations
from pathlib import Path
import re
import json
ROOT=Path(__file__).resolve().parents[1]
DOCS=ROOT/'docs'
FS_DIR=DOCS/'12-function-specs'
def line_value(text,key):
    for line in text.splitlines():
        if line.strip().startswith('|') and key.lower() in line.lower():
            parts=[p.strip() for p in line.strip('|').split('|')]
            if len(parts)>=2 and parts[0].lower()==key.lower(): return parts[1]
    return 'UNVERIFIED'
def parse_fs(path):
    text=path.read_text(encoding='utf-8')
    did='UNVERIFIED'; status='UNVERIFIED'
    for line in text.splitlines():
        if '文档 ID：' in line: did=line.split('`')[1] if '`' in line else 'UNVERIFIED'
        if '- 状态：' in line: status=line.split('`')[1] if '`' in line else 'UNVERIFIED'
    return {'id':did,'name':path.stem,'status':status,'project':line_value(text,'Project'),'port':line_value(text,'Canonical port'),'page':line_value(text,'UI PageContract'),'work_package':line_value(text,'Primary Work Package'),'algorithm':line_value(text,'Algorithm baseline'),'performance':line_value(text,'Performance gate'),'source':path.relative_to(ROOT).as_posix()}
def main():
    items=[parse_fs(p) for p in sorted(FS_DIR.glob('FS-*.md'))]
    missing=[x for x in items if x['id']=='UNVERIFIED']
    print(json.dumps({'repository':'asun01/PCB','function_spec_count':len(items),'items':items,'missing_identity':missing,'policy':'Document-derived reporting only; no contracts, APIs, owners, thresholds or acceptance values are invented.'},ensure_ascii=False,indent=2))
    return 1 if missing else 0
if __name__=='__main__': raise SystemExit(main())
