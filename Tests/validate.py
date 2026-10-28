from pathlib import Path
import xml.etree.ElementTree as E
import json
p=Path(__file__).resolve().parents[1]
a=E.parse(p/'About/About.xml').getroot()
assert a.findtext('packageId') in ['cupears.harvestalltails17','cupears.harvesteverything17']
assert len(a.findall('modVersion'))==1
assert [x.text for x in a.findall('supportedVersions/li')]==['1.7']
assert not (p/'About/PublishedFileId.txt').exists()
for f in p.rglob('*.xml'):
 if not any(x in f.parts for x in ['.git','obj','bin']):E.parse(f)
fixtures=json.loads((p/'Tests/race-fixtures.json').read_text())
assert len(fixtures)==46
if (p/'Races17').exists():
 doc=E.parse(p/'Races17/Patches/AdditionalHumanoidTails.xml')
 names=[x.findtext('defName') for x in doc.findall('.//RecipeDef')]
 assert len(names)==len(set(names))==18
 assert all(n.startswith('HATT17_') for n in names)
print('PASS: metadata, XML, publishing identity and 46 race fixtures')
