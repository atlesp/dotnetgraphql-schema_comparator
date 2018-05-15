using System;
using System.Collections.Generic;
using GraphQL.Types;

namespace schema_comparator
{
    internal class Directive
    {
        private DirectiveGraphType oldDirective;
        private DirectiveGraphType newDirective;
        private readonly QueryArguments oldArguments;
        private readonly QueryArguments newArguments;

        internal Directive(DirectiveGraphType oldDirective, DirectiveGraphType newDirective)
        {
            this.oldDirective = oldDirective;
            this.newDirective = newDirective;
            this.oldArguments = oldDirective.Arguments;
            this.newArguments = newDirective.Arguments;

        }

        internal List<Change> Diff()
        {
            List<Change> changes = new List<Change>();
            //TODO
            /*
            if old_directive.description != new_directive.description
            changes << Changes::DirectiveDescriptionChanged.new(old_directive, new_directive)
          end

          changes += removed_locations.map { | location | Changes::DirectiveLocationRemoved.new(new_directive, location) }
            changes += added_locations.map { | location | Changes::DirectiveLocationAdded.new(new_directive, location) }
            changes += added_arguments.map { | argument | Changes::DirectiveArgumentAdded.new(new_directive, argument) }
            changes += removed_arguments.map { | argument | Changes::DirectiveArgumentRemoved.new(new_directive, argument) }

            each_common_argument do | old_argument, new_argument |
               changes += Diff::DirectiveArgument.new(new_directive, old_argument, new_argument).diff
          end
          */

            return changes;
        }
    }
}