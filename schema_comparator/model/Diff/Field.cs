using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace schema_comparator.model.Diff
{
    internal class Field
    {
        private ObjectGraphType oldType;
        private ObjectGraphType newType;
        private FieldType oldField;
        private FieldType newField;
        private QueryArguments oldArguments;
        private QueryArguments newArguments;

        public Field(ObjectGraphType oldType, ObjectGraphType newType, FieldType oldField, FieldType newField)
        {
            this.oldType = oldType;
            this.newType = newType;
            this.oldField = oldField;
            this.newField = newField;

            this.oldArguments = oldField.Arguments;
            this.newArguments = newField.Arguments;
        }

        internal IEnumerable<Change> Diff()
        {
            List<Change> changes = new List<Change>();

            //TODO:
            /*
            if old_field.description != new_field.description
            changes << Changes::FieldDescriptionChanged.new(new_type, old_field, new_field)
            end*/

            if (oldField.DeprecationReason != newField.DeprecationReason)
            {
                changes.Add(new FieldDeprecationChanged(newType, oldField, newField));
            }

            if (oldField.ResolvedType.Name != newField.ResolvedType.Name)
            {
                changes.Add(new FieldTypeChanged(oldType, oldField, newField));
            }

            changes.AddRange(DiffTools.RemovedElements(oldArguments, newArguments,  a => new FieldArgumentRemoved(newType, oldField, a)));
            changes.AddRange(DiffTools.AddedElements(oldArguments, newArguments, a => new FieldArgumentAdded(newType, newField, a)));
            
            foreach (var newArg in this.newArguments)
            {
                QueryArgument oldArg = oldArguments.SingleOrDefault(t => t.Name.Equals(newArg.Name));
                if (oldArg != null)
                {
                    changes.AddRange(new Diff.Argument(newType, newField, oldArg, newArg).Diff());
                }
            }
   
            return changes;

        }
    }
}