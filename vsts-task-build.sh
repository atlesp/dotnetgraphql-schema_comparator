#!/usr/bin/env bash
# build 
#"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe" dotnetgraphql-schema_comparator.sln -p:Configuration=Release

#run tests
#./packages/NUnit.ConsoleRunner.3.9.0/tools/nunit3-console.exe ./schema_comparatorTests/bin/Release/schema_comparatorTests.dll

#copy dlls for the comperator
#cp ./schema_comparator_cli/bin/Release/*  ./vsts_task/GraphqlSchemaComperator/comperator

#update version number of the extension
bumped release patch
#set version number of the task to the same as the extension
node > out <<EOF
'use strict';

const fs = require('fs');

var extensionJson = require('./vsts_task/vss-extension.json');
var newVersion = extensionJson.version.split('.');
var taskJson = require('./vsts_task/GraphqlSchemaComperator/task.json');

taskJson.version.Major = newVersion[0];
taskJson.version.Minor = newVersion[1];
taskJson.version.Patch = newVersion[2];

//Output data
fs.writeFile('./vsts_task/GraphqlSchemaComperator/task.json',  JSON.stringify(taskJson, null, 2), (err) => {  
    if (err) throw err;
    console.log('Data written to file');
});

EOF

#sleep to make sure the version number is written
sleep 0.5

#build the extension 
#tfx extension create --root ./vsts_task --manifest-globs vss-extension.json
#tfx extension publish --root ./vsts_task --manifest-globs vss-extension.json 

