using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace schema_comparator.model.Diff
{
    internal class InputObjectType
    {
        private InputObjectGraphType oldType;
        private InputObjectGraphType newType;
        private readonly IEnumerable<FieldType> oldFields;
        private readonly IEnumerable<FieldType> newFields;

        public InputObjectType(InputObjectGraphType oldType, InputObjectGraphType newType)
        {
            this.oldType = oldType;
            this.newType = newType;


            oldFields = oldType.Fields;
            newFields = newType.Fields;
        }

        internal IEnumerable<Change> Diff()
        {
            List<Change> changes = new List<Change>();
            changes.AddRange(DiffTools.RemovedElements(oldFields, newFields, delegate (IFieldType t) { return new InputFieldRemoved(this.oldType, t); }));
            changes.AddRange(DiffTools.AddedElements(oldFields, newFields, delegate (IFieldType t) { return new InputFieldAdded(this.newType, t); }));


            foreach (var newField in this.newFields)
            {
                var oldField = this.oldFields.Where(t => t.Name.Equals(newField.Name)).SingleOrDefault();
                if (oldField != null)
                {
                    changes.AddRange(new Diff.InputField(oldType, newType, oldField, newField).Diff());
                }
            }


            return changes;

        }





    }
}