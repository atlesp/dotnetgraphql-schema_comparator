using System;
using System.Collections.Generic;
using GraphQL.Types;

namespace schema_comparator.model.Diff
{
    internal class UnionObjectType
    {
        private UnionGraphType oldType;
        private UnionGraphType newType;

        public UnionObjectType(UnionGraphType oldType, UnionGraphType newType)
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