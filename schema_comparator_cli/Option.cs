using CommandLine;

namespace schema_comparator_cli
{
    class Options
    {
        [Option('n', "newschema", Required = true, HelpText = "Path to file with the new grapql schema defintion")]
        public string NewSchemaFilePath { get; set; }

        [Option('o', "oldschema", Required = true, HelpText = "Path to file with the old grapql schema defintion")]
        public string OldSchemaFilePath { get; set; }


        [Option('d', "schemadiff", Required = true, HelpText = "Path to file that should contain the schema diff result.")]
        public string SchemaDIffResultFilePath { get; set; }


        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }


        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option('s',"silent", Default = false, HelpText = "")]
        public bool Silent { get; set; }


        /*[Option("stdin", Default = false, HelpText = "Read from stdin")]
        public bool stdin { get; set; }
        */

    }

}
