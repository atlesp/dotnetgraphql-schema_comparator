using System;
using System.Collections.Generic;
using GraphQL.Types;

namespace schema_comparator.model.Diff
{
    internal class InputField
    {
        private InputObjectGraphType oldType;
        private InputObjectGraphType newType;
        private FieldType oldField;
        private FieldType newField;

        public InputField(InputObjectGraphType oldType, InputObjectGraphType newType, FieldType oldField, FieldType newField)
        {
            this.oldType = oldType;
            this.newType = newType;
            this.oldField = oldField;
            this.newField = newField;
        }

        internal IEnumerable<Change> Diff()
        {
            List<Change> changes = new List<Change>();

            /*
             //TODO
            if( oldField.description != newField.description)
              changes << Changes::InputFieldDescriptionChanged.new(old_type, old_field, new_field)
          end*/

            if (oldField.DefaultValue != newField.DefaultValue) {
                changes.Add(new InputFieldDefaultChanged(oldType, oldField, newField));
            }
                 
            if (oldField.ResolvedType.Name != newField.ResolvedType.Name)
            {
                changes.Add(new InputFieldTypeChanged(oldType, oldField, newField));
            }

            return changes;

            //# TODO: directives
        }
    }
}