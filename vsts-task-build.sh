
# build 

#run tests

#copy dlls for the comperator
cp ./schema_comparator_cli/bin/Release/*  ./vsts_task/GraphqlSchemaComperator/comperator
#update version numbers
bumped release patch
#build the extension 
tfx extension create --root ./vsts_task --manifest-globs vss-extension.json
#tfx extension publish --root ./vsts_task --manifest-globs vss-extension.json 

