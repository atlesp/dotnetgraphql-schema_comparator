using GraphQL.Types;
using System;


namespace schema_comparator
{
    public class SchemaComparator
    {

        public Result Compare(String oldSchema, String newSchema)
        {
            
            Schema o = (Schema) Schema.For(oldSchema);
            Schema n = (Schema) Schema.For(newSchema);
            
            return Compare(o, n);
        }
        
        public Result Compare(Schema oldSchema, Schema newSchema)
        {
            System.Collections.Generic.List<Change> changes = new Schemas(oldSchema, newSchema).Diff();
            return new Result(changes);
        }
                        
    }
}
