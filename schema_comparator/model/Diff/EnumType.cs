using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace schema_comparator.model.Diff
{
    class EnumType
    {
        private readonly EnumerationGraphType oldEnum;
        private readonly EnumerationGraphType newEnum;
        private readonly EnumValues oldValues;
        private readonly EnumValues newValues;

        internal EnumType(EnumerationGraphType oldEnum, EnumerationGraphType newEnum)
        {
            this.oldEnum = oldEnum;
            this.newEnum = newEnum;

            this.oldValues = oldEnum.Values;
            this.newValues = newEnum.Values;

        }

        internal List<Change> Diff()
        {
            List<Change> changes = new List<Change>();

            changes.AddRange(RemovedValues()); 
            changes.AddRange(AddedValues());
    
            //TODO
            /*
    each_common_value do |old_value, new_value|
                # TODO: Add Directive Stuff

                if old_value.description != new_value.description
                  changes << Changes::EnumValueDescriptionChanged.new(new_enum, old_value, new_value)
                end

                if old_value.deprecation_reason != new_value.deprecation_reason
                  changes << Changes::EnumValueDeprecated.new(new_enum, old_value, new_value)
                end
              end

              changes
            end

            private

            attr_reader :old_enum, :new_enum, :old_values, :new_values

            def each_common_value(&block)
              intersection = old_values.keys & new_values.keys
              intersection.each do |common_value|
                old_value = old_enum.values[common_value]
                new_value = new_enum.values[common_value]

                block.call(old_value, new_value)
              end
            end

        end
      end
    end
    */

            return changes;
        }

        private IEnumerable<Change> AddedValues()
        {
            return this.newValues.Where(v => !oldValues.Any(ov => ov.Name.Equals(v.Name))).Select(v => new EnumValueAdded(this.newEnum, v));
        }

        private IEnumerable<Change> RemovedValues()
        {
            return this.oldValues.Where(v => !newValues.Any(nv => nv.Name.Equals(v.Name))).Select(v => new EnumValueRemoved(this.oldEnum,v));
        }
    }
}