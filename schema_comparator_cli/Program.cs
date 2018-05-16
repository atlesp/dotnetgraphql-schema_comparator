using CommandLine;
using Newtonsoft.Json;
using schema_comparator;
using System;
using System.Collections.Generic;
using System.IO;

namespace schema_comparator_cli
{
    class Program
    {

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));
          
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Failed to run");
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {

            System.Console.WriteLine($"Comparing {opts.NewSchemaFilePath} with {opts.OldSchemaFilePath}");

            var comperator = new SchemaComparator();
            Result result = null;
            try
            {
                result = comperator.Compare(File.ReadAllText(opts.OldSchemaFilePath), File.ReadAllText(opts.NewSchemaFilePath));
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Failed to compare the schemas: " + ex.Message);
                if (!opts.Silent)
                {
                    System.Console.WriteLine("Press a key to continue");
                    System.Console.ReadKey();
                }

            }
            if(result != null)
            {
                foreach (var change in result.changes)
                {
                    System.Console.WriteLine($"{change.Message}");
                }

                string json = JsonConvert.SerializeObject(result, Formatting.Indented);

                File.WriteAllText(opts.SchemaDIffResultFilePath, json);
                if (!opts.Silent)
                {
                    System.Console.WriteLine("Press a key to continue");
                    System.Console.ReadKey();
                }

            }

            
        }
    }
}
