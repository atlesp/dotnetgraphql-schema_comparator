using GraphQL.Types;
using schema_comparator.model.Diff;
using System.Collections.Generic;
using System.Linq;

namespace schema_comparator
{
    class Schemas
    {
        private readonly Schema oldSchema;
        private readonly Schema newSchema;

        public readonly IEnumerable<IGraphType> oldTypes;

        private readonly IEnumerable<IGraphType> newTypes;
        private readonly IEnumerable<DirectiveGraphType> oldDirectives;
        private readonly IEnumerable<DirectiveGraphType> newDirectives;

        internal Schemas(Schema oldSchema, Schema newSchema)
        {
            this.oldSchema = oldSchema;
            this.newSchema = newSchema;

            this.oldTypes = oldSchema.AllTypes;
            this.newTypes = newSchema.AllTypes;

            oldDirectives = oldSchema.Directives;
            newDirectives = newSchema.Directives;

        }

        internal List<Change> Diff()
        {
            List<Change> changes = new List<Change>();

            //Removed and Added Types
            changes.AddRange(DiffTools.AddedElements(oldTypes, newTypes, t => new TypeAddedChange((IGraphType)t)));
            changes.AddRange(DiffTools.RemovedElements(oldTypes, newTypes, t => new TypeRemovedChange((IGraphType)t)));

      
            //Type Diff for common types
            foreach (var newType in this.newTypes)
            {

                var oldType = this.oldTypes.Where(t => t.Name.Equals(newType.Name)).SingleOrDefault();
                if (oldType != null)
                {
                    changes.AddRange(ChangesInType(oldType, newType));
                }
            }

            //# Diff Schemas
            changes.AddRange(ChangesInSchema());

            // Diff Directives
            changes.AddRange(ChangesInDirectives());

            return changes;
        }

        private IEnumerable<Change> ChangesInSchema()
        {

            List<Change> changes = new List<Change>();

            if (!IsSchemaQueriesEqual())
            {
                changes.Add(new SchemaQueryTypeChanged(oldSchema, newSchema));
            }


            if (!IsMutationEqual())
            {
                changes.Add(new SchemaMutationTypeChanged(oldSchema, newSchema));
            }


            if (!IsSubscriptionEqual())
            {
                changes.Add(new SchemaSubscriptionTypeChanged(oldSchema, newSchema));
            }


            return changes;
        }

        private bool IsSubscriptionEqual()
        {
            if (oldSchema.Subscription == null && newSchema.Subscription == null) return true;
            if (oldSchema.Subscription == null || newSchema.Subscription == null) return false;
            return oldSchema.Subscription.Name.Equals(newSchema.Subscription.Name);
            
        }

        private bool IsMutationEqual()
        {
            if (oldSchema.Mutation == null && newSchema.Mutation == null) return true;
            if (oldSchema.Mutation == null || newSchema.Mutation == null) return false;
            return oldSchema.Mutation.Name.Equals(newSchema.Mutation.Name);
            
        }

        private bool IsSchemaQueriesEqual()
        {
            if( oldSchema.Query == null && newSchema.Query == null) return true;
            if (oldSchema.Query == null || newSchema.Query == null) return false;
            return oldSchema.Query.Name.Equals(newSchema.Query.Name);
        }

        private IEnumerable<Change> ChangesInType(IGraphType oldType, IGraphType newType)
        {
            List<Change> changes = new List<Change>();

            if (oldType.GetType() != newType.GetType())
            {
                changes.Add(new TypeKindChanged(oldType, newType));
            }
            else
            {

                if (oldType.GetType() == typeof(EnumerationGraphType))
                {
                    changes.AddRange(new model.Diff.EnumType((EnumerationGraphType)oldType, (EnumerationGraphType)newType).Diff());
                }
                else if (oldType.GetType() == typeof(ObjectGraphType))
                {
                    changes.AddRange(new model.Diff.ObjectType((ObjectGraphType)oldType, (ObjectGraphType)newType).Diff());
                }
                else if (oldType.GetType() == typeof(InputObjectGraphType))
                {
                    changes.AddRange(new model.Diff.InputObjectType((InputObjectGraphType)oldType, (InputObjectGraphType)newType).Diff());
                }
                else if (oldType.GetType() == typeof(UnionGraphType))
                {
                    changes.AddRange(new model.Diff.UnionObjectType((UnionGraphType)oldType, (UnionGraphType)newType).Diff());
                }
                else if (oldType.GetType() == typeof(InterfaceGraphType))
                {
                    changes.AddRange(new model.Diff.InterfacesType((InterfaceGraphType)oldType, (InterfaceGraphType)newType).Diff());
                }

            }


            if (!IsTypeDescriptionEqual(oldType, newType))
            {
                changes.Add(new TypeDescriptionChanged(oldType, newType));
            }

            return changes;

        }

        private static bool IsTypeDescriptionEqual(IGraphType oldType, IGraphType newType)
        {
            if (oldType.Description == null && newType.Description == null) return true;
            if (oldType.Description == null || newType.Description == null) return false;

            return oldType.Description.Equals(newType.Description);
        }

        private IEnumerable<Change> ChangesInDirectives()
        {
            List<Change> changes = new List<Change>();
            changes.AddRange(DiffTools.AddedElements(oldDirectives, newDirectives, d => new DirectiveAdded((DirectiveGraphType)d)));
            changes.AddRange(DiffTools.RemovedElements(oldDirectives, newDirectives, d => new DirectiveRemoved((DirectiveGraphType)d)));

            foreach (var newDirective in this.newDirectives)
            {

                var oldDirective = oldDirectives.Where(t => t.Name.Equals(newDirective.Name)).SingleOrDefault();
                if (oldDirective != null)
                {
                    changes.AddRange(new Directive(oldDirective, newDirective).Diff());
                    
                }
            }
            
            return changes;
        }
        
    }
}
