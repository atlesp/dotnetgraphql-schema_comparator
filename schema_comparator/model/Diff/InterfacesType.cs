using System;
using System.Collections.Generic;
using GraphQL.Types;

namespace schema_comparator.model.Diff
{
    internal class InterfacesType
    {
        private InterfaceGraphType oldType;
        private InterfaceGraphType newType;

        public InterfacesType(InterfaceGraphType oldType, InterfaceGraphType newType)
        {
            this.oldType = oldType;
            this.newType = newType;
        }

        internal IEnumerable<Change> Diff()
        {
            throw new NotImplementedException();
        }
    }
}