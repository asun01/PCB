from pathlib import Path
import xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[1]
SOLUTION=ROOT/'AsunVision.slnx'
def main():
    root=ET.parse(SOLUTION).getroot()
    projects=[p.attrib.get('Path','') for p in root.findall('Project')]
    missing=[p for p in projects if not (ROOT/p).exists()]
    print(f'projects={len(projects)} missing={len(missing)}')
    for p in missing: print('MISSING:',p)
    return 1 if missing else 0
if __name__=='__main__':
    raise SystemExit(main())
