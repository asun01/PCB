from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
OPEN_GATES=ROOT/'docs/00-baseline/OPEN-GATES.md'
def main():
    text=OPEN_GATES.read_text(encoding='utf-8')
    print('Recorded open gates:')
    for line in text.splitlines():
        if line.startswith('| ') and 'Gate' not in line[:20] and '---' not in line:
            print(line)
    return 0
if __name__=='__main__': raise SystemExit(main())
