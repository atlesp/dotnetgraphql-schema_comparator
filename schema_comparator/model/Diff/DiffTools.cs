using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;

namespace schema_comparator.model.Diff
{
    internal static class DiffTools
    {
        internal delegate Change CreateChangeGraphType(INamedType t);

        internal static IEnumerable<Change> AddedElements(IEnumerable<INamedType> oldElements, IEnumerable<INamedType> newElements, CreateChangeGraphType creator)
        {
            return newElements
                .Where(t => !oldElements.Any(ot => ot.Name.Equals(t.Name)))
                .Select(d => creator(d));
        }

        internal static IEnumerable<Change> RemovedElements(IEnumerable<INamedType> oldElements, IEnumerable<INamedType> newElements, CreateChangeGraphType creator)
        {
            return oldElements.Where(t => !newElements.Any(nt => nt.Name.Equals(t.Name))).Select(d => creator(d));
        }


        internal delegate Change CreateChangeFieldType(IFieldType t);

        internal static IEnumerable<Change> AddedElements(IEnumerable<IFieldType> oldElements, IEnumerable<IFieldType> newElements, CreateChangeFieldType creator)
        {
            return newElements
                .Where(t => !oldElements.Any(ot => ot.Name.Equals(t.Name)))
                .Select(d => creator(d));
        }

        internal static IEnumerable<Change> RemovedElements(IEnumerable<IFieldType> oldElements, IEnumerable<IFieldType> newElements, CreateChangeFieldType creator)
        {
            return oldElements.Where(t => !newElements.Any(nt => nt.Name.Equals(t.Name))).Select(d => creator(d));
        }


    }
}
