using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace schema_comparator.model.Changes
{
    class SafeTypeChanges
    {

        /*
        internal static IsSafeChangeForField(old_type, new_type)
        {
            if (!old_type.kind.wraps ? && !new_type.kind.wraps) {
                old_type == new_type
            }
            else if (new_type.kind.non_null)
                of_type = old_type.kind.non_null ? ? old_type.of_type : old_type
            safe_change_for_field ? (of_type, new_type.of_type)
            else if (old_type.kind.list 
                ? new_type.kind.list ? && safe_change_for_field
                ? (old_type.of_type, new_type.of_type) ||
                  new_type.kind.non_null ? && safe_change_for_field ? (old_type, new_type.of_type)
            else

            false
        }

        end
        }
        */
        /*
        def safe_change_for_input_value?(old_type, new_type)
        if !old_type.kind.wraps? && !new_type.kind.wraps?
            old_type == new_type
            elsif old_type.kind.list? && new_type.kind.list?
            safe_change_for_input_value?(old_type.of_type, new_type.of_type)
            elsif old_type.kind.non_null?
            of_type = new_type.kind.non_null ? ? new_type.of_type : new_type
            safe_change_for_input_value ? (old_type.of_type, of_type)
        else
        false
        end
            end
        end
        */
    }
}
