#copy dlls for the comperator
cp ./schema_comparator_cli/bin/Release/*  ./vsts_task/GraphqlSchemaComperator/comperator
#update version numbers

#build the extension 
tfx extension create --manifest-globs vss-extension.json