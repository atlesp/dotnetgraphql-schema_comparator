using System;
using System.Collections.Generic;
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


            /*

            if old_field.type != new_field.type
              changes << Changes::FieldTypeChanged.new(new_type, old_field, new_field)
            end

            changes += arg_removals

            changes += arg_additions

            each_common_argument do | old_arg, new_arg |
               changes += Diff::Argument.new(new_type, new_field, old_arg, new_arg).diff
            end
            */
            return changes;

        }
    }
}