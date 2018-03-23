
#sets the version in the VersionInfo.cs file and the .version file

import sys
import json

VersionInfo = "./Kerbal_Construction_Time/Properties/VersionInfo.cs"
VersionFile = "./GameData/KerbalConstructionTime/.version"


with open(VersionFile, 'r') as v:
    fileJSON = json.load(v)

build = sys.argv[1]

major = fileJSON["VERSION"]["MAJOR"]
minor = fileJSON["VERSION"]["MINOR"]
patch = fileJSON["VERSION"]["PATCH"]

version = '{0}.{1}.{2}.{3}'.format(major, minor, patch, build)

print('Version is '+version)

#update the VersionInfo.cs file
print("Setting .dll version")
with open(VersionInfo, 'w') as v:
    #v.write('[assembly: System.Reflection.AssemblyVersion("' + version + '")] //Added by build\n')
    v.write('[assembly: System.Reflection.AssemblyFileVersion("' + version + '")] //Added by build\n')
    v.write('[assembly: System.Reflection.AssemblyInformationalVersion("' + version + '")] //Added by build\n')
  
#Update the .version file
#don't erase everything, just the version stuff
#it's json, so lets do it that way
print("Editing .version file")

fileJSON["VERSION"]["BUILD"] = build

with open(VersionFile, 'w') as v:
    json.dump(fileJSON, v, sort_keys=True, indent=4)

print("Writing version.txt")
with open('version.txt', 'w') as f:
    f.write(version+'\n')

print("Done!")