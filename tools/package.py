from pathlib import Path
import zipfile,xml.etree.ElementTree as E
p=Path(__file__).resolve().parents[1]
a=E.parse(p/'About/About.xml').getroot()
name=a.findtext('packageId')
output=p/(name+'-1.7.0-beta.1.zip')
folders=['About','1.7','Textures','Languages','Races17','Kurin','Revia','Moyo','Dragonian','DragonianUnofficial','Silkiera','Ratkin','Kiiro','Rabbie','Glassy','Mofy','Heruan','Nyaron']
with zipfile.ZipFile(output,'w',zipfile.ZIP_DEFLATED) as z:
 for folder in folders:
  d=p/folder
  if not d.exists():continue
  for f in d.rglob('*'):
   if f.is_file() and f.name!='PublishedFileId.txt' and f.suffix.lower() not in ['.pdb']:
    z.write(f,Path(name)/f.relative_to(p))
 for name in ['LICENSE','README.md','CHANGELOG.md','RACE-COVERAGE.md','loadFolders.xml','LoadFolders.xml','WORKSHOP-PREPARATION.md']:
  f=p/name
  if f.exists():z.write(f,Path(a.findtext('packageId'))/f.relative_to(p))
print(output)
