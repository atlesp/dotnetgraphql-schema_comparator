using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace schema_comparator.model.Diff
{
    internal class ObjectType
    {
        private ObjectGraphType oldType;
        private ObjectGraphType newType;
        private IEnumerable<FieldType> oldfields;
        private IEnumerable<FieldType> newfields;
        private IEnumerable<Type> oldInterfaces;
        private IEnumerable<Type> newInterfaces;

        public ObjectType(ObjectGraphType oldType, ObjectGraphType newType)
        {
            this.oldType = oldType;
            this.newType = newType;

            this.oldfields = oldType.Fields;
            this.newfields = newType.Fields;

            this.oldInterfaces = oldType.Interfaces;
            this.newInterfaces = newType.Interfaces;

        }


        internal List<Change> Diff()
        {
            List<Change> changes = new List<Change>();

            //TODO struggling with the types here
            //changes.AddRange(InterfaceAdditions());
            //changes.AddRange(InterfaceRemovals());

            changes.AddRange(DiffTools.RemovedElements(oldfields, newfields, delegate (IFieldType t) { return new FieldRemoved(oldType, t); }));
            changes.AddRange(DiffTools.AddedElements(oldfields, newfields, delegate (IFieldType t) { return new FieldAdded(newType, t); }));

            foreach (var newField in this.newfields)
            {

                var oldField = this.oldfields.Where(t => t.Name.Equals(newField.Name)).SingleOrDefault();
                if (oldField != null)
                {
                    changes.AddRange(new Diff.Field(oldType, newType, oldField, newField).Diff());
                }
            }

            return changes;
        }


        private IEnumerable<Change> InterfaceAdditions()
        {
            return this.newInterfaces
                        .Where(t => !this.oldInterfaces.Any(ot => ot.Name.Equals(t.Name)))
                        .Select(t => new ObjectTypeInterfaceAdded(t, this.newType));
        }


        private IEnumerable<Change> InterfaceRemovals()
        {
            return this.oldInterfaces
                        .Where(t => !newInterfaces.Any(nt => nt.Name.Equals(t.Name)))
                        .Select(t => new ObjectTypeInterfaceRemoved(t, this.oldType));
        }

    }


}