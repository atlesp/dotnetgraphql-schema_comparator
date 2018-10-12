using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;

namespace schema_comparator.model.Diff
{
    internal class Argument
    {

        private ObjectGraphType newType;
        private FieldType newField;
        private QueryArgument oldArgument;
        private QueryArgument newArgument;


        public Argument(ObjectGraphType newType, FieldType newField, QueryArgument oldArg, QueryArgument newArg)
        {
            this.newType = newType;
            this.newField = newField;
            this.oldArgument = oldArg;
            this.newArgument = newArg;
        }



        internal IEnumerable<Change> Diff()
        {
            List<Change> changes = new List<Change>();
            /*

            def diff
            changes = []

        if old_arg.description != new_arg.description
            changes << Changes::FieldArgumentDescriptionChanged.new(type, field, old_arg, new_arg)
            end

        if old_arg.default_value != new_arg.default_value
            changes << Changes::FieldArgumentDefaultChanged.new(type, field, old_arg, new_arg)
            end
            */
       if (oldArgument.ResolvedType.Name != newArgument.ResolvedType.Name) { 
            changes.Add(new FieldArgumentTypeChanged(newType, newField, oldArgument, newArgument));
       }
                /*
// TODO directives

        changes
            end

        private

            attr_reader(
                :type,
        :field,
        :new_arg,
        :old_arg
        )
        end
            end
        end
            end*/
            return changes;
        }
    }
}
