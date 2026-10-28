from pathlib import Path
import zipfile,xml.etree.ElementTree as E
p=Path(__file__).resolve().parents[1]
a=E.parse(p/'About/About.xml').getroot()
packageId=a.findtext('packageId')
version=a.findtext('modVersion') or 'dev'
output=p/(f'{packageId}-{version}.zip')
folders=['About','1.6','1.7','Textures','Languages','Races']
with zipfile.ZipFile(output,'w',zipfile.ZIP_DEFLATED) as z:
 for folder in folders:
  d=p/folder
  if not d.exists():continue
  for f in d.rglob('*'):
   if f.is_file() and f.name!='PublishedFileId.txt' and f.suffix.lower() not in ['.pdb']:
    z.write(f,Path(packageId)/f.relative_to(p))
 for extra in ['LICENSE-Tails','README.md','CHANGELOG.md','RACE-COVERAGE.md','loadFolders.xml','LoadFolders.xml','WORKSHOP-PREPARATION.md']:
  f=p/extra
  if f.exists():z.write(f,Path(packageId)/f.relative_to(p))
print(output)
