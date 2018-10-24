using GraphQL.Types;
using System;
using GraphQL;

namespace schema_comparator
{
    public abstract class Change
    {
        private Criticality criticality;

        internal Change(Criticality criticality)
        {
            this.criticality = criticality;
        }

        public String Name
        {
            get { return this.GetType().Name; }
        }


        public Boolean IsBreaking
        {
            get
            {
                return this.criticality == Criticality.Breaking;
            }
        }

        public Boolean IsDangerous
        {
            get
            {
                return this.criticality == Criticality.Dangerous;
            }
        }

        public Boolean IsNonBreaking
        {
            get
            {
                return this.criticality == Criticality.NonBreaking;
            }
        }

        public String Message
        {
            get { return GetMessage(); }
        }

        public String Path
        {
            get { return GetPath(); }
        }

        protected abstract String GetMessage();

        protected abstract String GetPath();

    }

    public class TypeRemovedChange : Change
    {
        private readonly IGraphType removedType;
        internal TypeRemovedChange(IGraphType removedType) : base(Criticality.Breaking)
        {
            this.removedType = removedType;

        }

        protected override string GetMessage()
        {
            return "Type `" + this.removedType.Name + "` was removed";
        }

        protected override string GetPath()
        {
            return this.removedType.Name;
        }
    }

    public class TypeAddedChange : Change
    {
        private readonly IGraphType addedType;
        internal TypeAddedChange(IGraphType addedType) : base(Criticality.NonBreaking)
        {
            this.addedType = addedType;

        }

        protected override string GetMessage()
        {
            return "Type `" + this.addedType.Name + "` was added";
        }

        protected override string GetPath()
        {
            return this.addedType.Name;
        }

    }


    public class TypeKindChanged : Change
    {

        private readonly IGraphType oldType;
        private readonly IGraphType newType;
        internal TypeKindChanged(IGraphType oldType, IGraphType newType) : base(Criticality.Breaking)
        {
            this.oldType = oldType;
            this.newType = newType;
        }

        protected override string GetMessage()
        {
            return $"`{oldType.Name}` kind changed from `{oldType.GetType().Name}` to `{newType.GetType().Name}`";
        }

        protected override string GetPath()
        {
            return this.oldType.Name;
        }

    }

    public class TypeDescriptionChanged : Change
    {
        private readonly IGraphType oldType;
        private readonly IGraphType newType;
        internal TypeDescriptionChanged(IGraphType oldType, IGraphType newType) : base(Criticality.NonBreaking)
        {
            this.oldType = oldType;
            this.newType = newType;
        }

        protected override string GetMessage()
        {
            return $"Description `{oldType.Description}` on type `{oldType.Name}` has changed to `{newType.Description}`";
        }

        protected override string GetPath()
        {
            return this.oldType.Name;
        }


    }



    public class EnumValueAdded : Change
    {

        private readonly EnumerationGraphType enumType;
        private readonly EnumValueDefinition enumValue;
        internal EnumValueAdded(EnumerationGraphType enumType, EnumValueDefinition enumValue) : base(Criticality.Dangerous)
        {
            this.enumType = enumType;
            this.enumValue = enumValue;
            //TODO: Reason
            /*      reason: "Adding an enum value may break existing clients that were not " \
            "programming defensively against an added case when querying an enum." */
        }

        protected override string GetMessage()
        {
            return $"Enum value `{enumValue.Name}` was added to enum `{enumType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{enumType.Name}.{enumValue.Name}";
        }


    }

    public class EnumValueRemoved : Change
    {

        private readonly EnumerationGraphType enumType;
        private readonly EnumValueDefinition enumValue;
        internal EnumValueRemoved(EnumerationGraphType enumType, EnumValueDefinition enumValue) : base(Criticality.Breaking)
        {
            this.enumType = enumType;
            this.enumValue = enumValue;
        }

        protected override string GetMessage()
        {
            return $"Enum value `{enumValue.Name}` was removed from enum `{enumType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{enumType.Name}.{enumValue.Name}";
        }

    }


    public class ObjectTypeInterfaceAdded : Change
    {
        private Type interfaceType;
        private ObjectGraphType objectType;

        public ObjectTypeInterfaceAdded(Type interfaceType, ObjectGraphType objectType) : base(Criticality.Dangerous)
        {
            this.interfaceType = interfaceType;
            this.objectType = objectType;

            //TODO:
            /*
                 reason: "Adding an interface to an object type may break existing clients " \
      "that were not programming defensively against a new possible type."

             */

        }

        protected override string GetMessage()
        {
            return $"`{objectType.Name}` object implements `{interfaceType.Name}` interface";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}";
        }

    }

    public class ObjectTypeInterfaceRemoved : Change
    {
        private Type interfaceType;
        private ObjectGraphType objectType;

        public ObjectTypeInterfaceRemoved(Type interfaceType, ObjectGraphType objectType) : base(Criticality.Breaking)
        {
            this.interfaceType = interfaceType;
            this.objectType = objectType;
        }
        protected override string GetMessage()
        {
            return $"`{objectType.Name}` object type no longer implements `{interfaceType.Name}` interface";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}";
        }

    }

    public class FieldAdded : Change
    {
        private IFieldType field;
        private ObjectGraphType objectType;

        public FieldAdded(ObjectGraphType objectType, IFieldType field) : base(Criticality.NonBreaking)
        {
            this.field = field;
            this.objectType = objectType;
        }
        protected override string GetMessage()
        {
            return $"Field `{field.Name}` was added to object type `{objectType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{field.Name}";
        }

    }

    public class FieldRemoved : Change
    {
        private IFieldType field;
        private ObjectGraphType objectType;

        public FieldRemoved(ObjectGraphType objectType, IFieldType field) : base(Criticality.Breaking)
        {
            this.field = field;
            this.objectType = objectType;
        }
        protected override string GetMessage()
        {
            return $"Field `{field.Name}` was removed from object type `{objectType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{field.Name}";
        }

    }

    public class DirectiveAdded : Change
    {
        private DirectiveGraphType directive;


        public DirectiveAdded(DirectiveGraphType directive) : base(Criticality.NonBreaking)
        {
            this.directive = directive;

        }
        protected override string GetMessage()
        {
            return $"Directive `{directive.Name}` was added";
        }

        protected override string GetPath()
        {
            return $"{directive.Name}";
        }

    }

    public class DirectiveRemoved : Change
    {
        private DirectiveGraphType directive;

        public DirectiveRemoved(DirectiveGraphType directive) : base(Criticality.Breaking)
        {
            this.directive = directive;
        }
        protected override string GetMessage()
        {
            return $"Directive `{directive.Name}` was removed";
        }

        protected override string GetPath()
        {
            return $"{directive.Name}";
        }

    }

    public class SchemaQueryTypeChanged : Change
    {
        private readonly Schema oldSchema;
        private readonly Schema newSchema;

        public SchemaQueryTypeChanged(Schema oldSchema, Schema newSchema) : base(Criticality.Breaking)
        {
            this.oldSchema = oldSchema;
            this.newSchema = newSchema;
        }
        protected override string GetMessage()
        {
            return $"Schema query root has changed from `{oldSchema.Query.Name}` to `{newSchema.Query.Name}`";
        }

        protected override string GetPath()
        {
            return $"{oldSchema.Query.Name}";
        }

    }

    public class SchemaMutationTypeChanged : Change
    {
        private readonly Schema oldSchema;
        private readonly Schema newSchema;

        public SchemaMutationTypeChanged(Schema oldSchema, Schema newSchema) : base(Criticality.Breaking)
        {
            this.oldSchema = oldSchema;
            this.newSchema = newSchema;
        }
        protected override string GetMessage()
        {
            return $"Schema mutation root has changed from `{oldSchema.Mutation.Name}` to `{newSchema.Mutation.Name}`";
        }

        protected override string GetPath()
        {
            return $"{oldSchema.Mutation.Name}";
        }

    }

    public class SchemaSubscriptionTypeChanged : Change
    {
        private readonly Schema oldSchema;
        private readonly Schema newSchema;

        public SchemaSubscriptionTypeChanged(Schema oldSchema, Schema newSchema) : base(Criticality.Breaking)
        {
            this.oldSchema = oldSchema;
            this.newSchema = newSchema;
        }
        protected override string GetMessage()
        {
            return $"Schema subscription type has changed from `{oldSchema.Subscription.Name}` to `{newSchema.Subscription.Name}`";
        }

        protected override string GetPath()
        {
            return $"{oldSchema.Subscription.Name}";
        }

    }

    public class InputFieldAdded : Change
    {
        private readonly IGraphType objectType;
        private readonly IFieldType field;

        public InputFieldAdded(IGraphType objectType, IFieldType field) : base(Criticality.NonBreaking)
        {
            this.objectType = objectType;
            this.field = field;

        }
        protected override string GetMessage()
        {
            return $"Input field `{field.Name}` was added to input object type `{objectType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{field.Name}";
        }

    }

    public class InputFieldRemoved : Change
    {
        private readonly IGraphType objectType;
        private readonly IFieldType field;

        public InputFieldRemoved(IGraphType objectType, IFieldType field) : base(Criticality.Breaking)
        {
            this.objectType = objectType;
            this.field = field;

        }
        protected override string GetMessage()
        {
            return $"Input field `{field.Name}` was removed from input object type `{objectType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{field.Name}";
        }

    }
    public class FieldDeprecationChanged : Change
    {
        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly IFieldType newField;

        public FieldDeprecationChanged(IGraphType objectType, IFieldType oldField, IFieldType newField) : base(Criticality.NonBreaking)
        {
            this.objectType = objectType;
            this.oldField = oldField;
            this.newField = newField;
        }
        protected override string GetMessage()
        {
            return $"Deprecation reason on field `{objectType.Name}.{newField.Name}` has changed " +
                    $"from `{oldField.DeprecationReason}` to `{newField.DeprecationReason}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}";
        }

    }

    public class FieldArgumentRemoved : Change
    {

        private readonly IGraphType objectType;
        private readonly IFieldType field;
        private readonly QueryArgument argument;


        public FieldArgumentRemoved(IGraphType objectType, IFieldType field, QueryArgument argument) : base(Criticality.Breaking)
        {
            this.objectType = objectType;
            this.field = field;
            this.argument = argument;

        }

        protected override string GetMessage()
        {
            return $"Argument `{ argument.Name}:{argument.ResolvedType.Name}` was removed from field `{ objectType.Name}.{ field.Name}`";
        }

        protected override string GetPath()
        {
            //[ object_type.name, field.name, argument.name].join('.')
            return $"{objectType.Name}.{field.Name}.{argument.Name}";
        }

    }


    public class FieldArgumentAdded : Change
    {

        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly QueryArgument argument;


        public FieldArgumentAdded(IGraphType objectType, IFieldType oldField, QueryArgument argument)
                : base(argument.ResolvedType is NonNullGraphType ? Criticality.Breaking : Criticality.NonBreaking)
        {

            this.objectType = objectType;
            this.oldField = oldField;
            this.argument = argument;

        }

        protected override string GetMessage()
        {
            var typename = argument.ResolvedType is NonNullGraphType
                ? ((NonNullGraphType)argument.ResolvedType).ResolvedType.Name
                : argument.ResolvedType.Name;
            return $"Argument `{argument.Name}:{typename}` added to field `{objectType.Name}.{oldField.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}.{argument.Name}";
        }

    }


    public class FieldArgumentDefaultChanged : Change
    {

        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly QueryArgument oldArgument;
        private readonly QueryArgument newArgument;


        public FieldArgumentDefaultChanged(IGraphType objectType, IFieldType field,
                                         QueryArgument oldArgument, QueryArgument newArgument)
            : base(Criticality.Dangerous)
        {
            this.objectType = objectType;
            this.oldField = field;
            this.newArgument = newArgument;
            this.oldArgument = oldArgument;
            // reason: "Changing the default value for an argument may change the runtime " \
            //"behaviour of a field if it was never provided."

        }


        protected override string GetMessage()
        {
            if (oldArgument.DefaultValue == null)
            {
                return
                    $"Default value `{newArgument.DefaultValue}` was added to argument `{newArgument.Name}` on field `{objectType.Name}.{oldField.Name}`";
            }
            else
            {
                return $"Default value for argument `{newArgument.Name}` on field `{objectType.Name}.{oldField.Name}` changed"
                       + $" from `{oldArgument.DefaultValue}` to `{newArgument.DefaultValue}`";
            }
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}.{oldArgument.Name}";
        }


    }


    public class InputFieldTypeChanged : Change
    {
        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly IFieldType newField;

        public InputFieldTypeChanged(IGraphType objectType, IFieldType oldField, IFieldType newField) : base(Criticality.Breaking)
        {
            //TODO:  reason: "Changing an input field from non-null to null is considered non-breaking"
            this.objectType = objectType;
            this.oldField = oldField;
            this.newField = newField;
        }
        protected override string GetMessage()
        {
            return $"Input field `{objectType}.{oldField.Name}` changed type from `{oldField.ResolvedType.Name}` to `{newField.ResolvedType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}";
        }

    }


    public class InputFieldDefaultChanged : Change
    {
        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly IFieldType newField;

        public InputFieldDefaultChanged(IGraphType objectType, IFieldType oldField, IFieldType newField) : base(Criticality.Dangerous)
        {
            //reason: "Changing the default value for an argument may change the runtime " \
            //"behaviour of a field if it was never provided."
            this.objectType = objectType;
            this.oldField = oldField;
            this.newField = newField;
        }
        protected override string GetMessage()
        {
            return $"Input field `{objectType.Name}.{oldField.Name}` default changed"
                    + $" from `{oldField.DefaultValue}` to `{newField.DefaultValue}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}";
        }

    }


    public class FieldArgumentTypeChanged : Change
    {
        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly QueryArgument oldArgument;
        private readonly QueryArgument newArgument;


        private static Criticality getCriticalityLevel(QueryArgument oldArgument, QueryArgument newArgument)
        {
            if ((oldArgument.ResolvedType is NonNullGraphType && !(newArgument.ResolvedType is NonNullGraphType))
                && ((NonNullGraphType)oldArgument.ResolvedType).ResolvedType.Name == newArgument.ResolvedType.Name)
            {
                //"Changing an input field from non-null to null is considered non-breaking"
                return Criticality.NonBreaking;
            }

            return Criticality.Breaking;
        }

        public FieldArgumentTypeChanged(IGraphType objectType, IFieldType field, QueryArgument oldArgument, QueryArgument newArgument)
                        : base(getCriticalityLevel(oldArgument, newArgument))
        {

            this.objectType = objectType;
            this.oldField = field;
            this.newArgument = newArgument;
            this.oldArgument = oldArgument;
        }
        protected override string GetMessage()
        {

            return $"Type for argument `{ newArgument.Name}` on field `{objectType.Name}.{oldField.Name}` changed" +
                        $" from `{oldArgument.ResolvedType.Name}` to `{newArgument.ResolvedType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}.{oldArgument.Name}";
        }


    }


    public class FieldTypeChanged : Change
    {
        // include SafeTypeChange


        private readonly IGraphType objectType;
        private readonly IFieldType oldField;
        private readonly IFieldType newField;

        public FieldTypeChanged(IGraphType objectType, IFieldType oldField, IFieldType newField) : base(Criticality.Breaking)
        {
            /*
            if safe_change_for_field?(old_field.type, new_field.type)
                Changes::Criticality.non_breaking
            else
                Changes::Criticality.breaking
             */
            this.objectType = objectType;
            this.oldField = oldField;
            this.newField = newField;
        }


        protected override string GetMessage()
        {
            return $"Field `{objectType}.{oldField.Name}` changed type from `{oldField.ResolvedType.Name}` to `{newField.ResolvedType.Name}`";
        }

        protected override string GetPath()
        {
            return $"{objectType.Name}.{oldField.Name}";
        }

    }

    /*
    class InputFieldDefaultChanged < AbstractChange
attr_reader :input_type, :old_field, :new_field, :criticality

def initialize(input_type, old_field, new_field)
@criticality = Changes::Criticality.dangerous(
reason: "Changing the default value for an argument may change the runtime " \
"behaviour of a field if it was never provided."
)
@input_type = input_type
@old_field = old_field
@new_field = new_field
end

def message
"Input field `{input_type.name}.{old_field.name}` default changed"\
" from `{old_field.default_value}` to `{new_field.default_value}`"
end

def path
[input_type.name, old_field.name].join(".")
end
end


 
  
 */

}


/*

class UnionMemberRemoved < AbstractChange
attr_reader :union_type, :union_member, :criticality

def initialize(union_type, union_member)
@union_member = union_member
@union_type = union_type
@criticality = Changes::Criticality.breaking
end

def message
"Union member `{union_member.name}` was removed from Union type `{union_type.name}`"
end

def path
union_type.name
end
end


class DirectiveArgumentRemoved < AbstractChange
attr_reader :directive, :argument, :criticality

def initialize(directive, argument)
@directive = directive
@argument = argument
@criticality = Changes::Criticality.breaking
end

def message
"Argument `{argument.name}` was removed from directive `{directive.name}`"
end

def path
["@{directive.name}", argument.name].join('.')
end
end


class DirectiveLocationRemoved < AbstractChange
attr_reader :directive, :location, :criticality

def initialize(directive, location)
@directive = directive
@location = location
@criticality = Changes::Criticality.breaking
end

def message
"Location `{location}` was removed from directive `{directive.name}`"
end

def path
"@{directive.name}"
end
end





class DirectiveArgumentTypeChanged < AbstractChange
include SafeTypeChange

attr_reader :directive, :old_argument, :new_argument, :criticality

def initialize(directive, old_argument, new_argument)
if safe_change_for_input_value?(old_argument.type, new_argument.type)
@criticality = Changes::Criticality.non_breaking(
reason: "Changing an input field from non-null to null is considered non-breaking"
)
else
@criticality = Changes::Criticality.breaking
end

@directive = directive
@old_argument = old_argument
@new_argument = new_argument
end

def message
"Type for argument `{new_argument.name}` on directive `{directive.name}` changed"\
" from `{old_argument.type}` to `{new_argument.type}`"
end

def path
["@{directive.name}", old_argument.name].join('.')
end
end


# Dangerous Changes


class DirectiveArgumentDefaultChanged < AbstractChange
attr_reader :directive, :old_argument, :new_argument, :criticality

def initialize(directive, old_argument, new_argument)
@criticality = Changes::Criticality.dangerous(
reason: "Changing the default value for an argument may change the runtime " \
"behaviour of a field if it was never provided."
)
@directive = directive
@old_argument = old_argument
@new_argument = new_argument
end

def message
"Default value for argument `{new_argument.name}` on directive `{directive.name}` changed"\
" from `{old_argument.default_value}` to `{new_argument.default_value}`"
end

def path
["@{directive.name}", new_argument.name].join(".")
end
end

class UnionMemberAdded < AbstractChange
attr_reader :union_type, :union_member, :criticality

def initialize(union_type, union_member)
@union_member = union_member
@union_type = union_type
@criticality = Changes::Criticality.dangerous(
reason: "Adding a possible type to Unions may break existing clients " \
"that were not programming defensively against a new possible type."
)
end

def message
"Union member `{union_member.name}` was added to Union type `{union_type.name}`"
end

def path
union_type.name
end
end

# Mostly Non-Breaking Changes




class EnumValueDescriptionChanged < AbstractChange
attr_reader :enum, :old_enum_value, :new_enum_value, :criticality

def initialize(enum, old_enum_value, new_enum_value)
@enum = enum
@old_enum_value = old_enum_value
@new_enum_value = new_enum_value
@criticality = Changes::Criticality.non_breaking
end

def message
"Description for enum value `{enum.name}.{new_enum_value.name}` changed from " \
"`{old_enum_value.description}` to `{new_enum_value.description}`"
end

def path
[enum.name, old_enum_value.name].join(".")
end
end

class EnumValueDeprecated < AbstractChange
attr_reader :enum, :old_enum_value, :new_enum_value, :criticality

def initialize(enum, old_enum_value, new_enum_value)
@criticality = Changes::Criticality.non_breaking
@enum = enum
@old_enum_value = old_enum_value
@new_enum_value = new_enum_value
end

def message
if old_enum_value.deprecation_reason
"Enum value `{enum.name}.{new_enum_value.name}` deprecation reason changed " \
"from `{old_enum_value.deprecation_reason}` to `{new_enum_value.deprecation_reason}`"
else
"Enum value `{enum.name}.{new_enum_value.name}` was deprecated with reason" \
" `{new_enum_value.deprecation_reason}`"
end
end

def path
[enum.name, old_enum_value.name].join(".")
end
end

class InputFieldDescriptionChanged < AbstractChange
attr_reader :input_type, :old_field, :new_field, :criticality

def initialize(input_type, old_field, new_field)
@criticality = Changes::Criticality.non_breaking
@input_type = input_type
@old_field = old_field
@new_field = new_field
end

def message
"Input field `{input_type.name}.{old_field.name}` description changed"\
" from `{old_field.description}` to `{new_field.description}`"
end

def path
[input_type.name, old_field.name].join(".")
end
end

class DirectiveDescriptionChanged < AbstractChange
attr_reader :old_directive, :new_directive, :criticality

def initialize(old_directive, new_directive)
@criticality = Changes::Criticality.non_breaking
@old_directive = old_directive
@new_directive = new_directive
end

def message
"Directive `{new_directive.name}` description changed"\
" from `{old_directive.description}` to `{new_directive.description}`"
end

def path
"@{old_directive.name}"
end
end

class FieldDescriptionChanged < AbstractChange
attr_reader :type, :old_field, :new_field, :criticality

def initialize(type, old_field, new_field)
@criticality = Changes::Criticality.non_breaking
@type = type
@old_field = old_field
@new_field = new_field
end

def message
"Field `{type.name}.{old_field.name}` description changed"\
" from `{old_field.description}` to `{new_field.description}`"
end

def path
[type.name, old_field.name].join(".")
end
end

class FieldArgumentDescriptionChanged < AbstractChange
attr_reader :type, :field, :old_argument, :new_argument, :criticality

def initialize(type, field, old_argument, new_argument)
@criticality = Changes::Criticality.non_breaking
@type = type
@field = field
@old_argument = old_argument
@new_argument = new_argument
end

def message
"Description for argument `{new_argument.name}` on field `{type.name}.{field.name}` changed"\
" from `{old_argument.description}` to `{new_argument.description}`"
end

def path
[type.name, field.name, old_argument.name].join(".")
end
end

class DirectiveArgumentDescriptionChanged < AbstractChange
attr_reader :directive, :old_argument, :new_argument, :criticality

def initialize(directive, old_argument, new_argument)
@criticality = Changes::Criticality.non_breaking
@directive = directive
@old_argument = old_argument
@new_argument = new_argument
end

def message
"Description for argument `{new_argument.name}` on directive `{directive.name}` changed"\
" from `{old_argument.description}` to `{new_argument.description}`"
end

def path
["@{directive.name}", old_argument.name].join(".")
end
end




class DirectiveLocationAdded < AbstractChange
attr_reader :directive, :location, :criticality

def initialize(directive, location)
@criticality = Changes::Criticality.non_breaking
@directive = directive
@location = location
end

def message
"Location `{location}` was added to directive `{directive.name}`"
end

def path
"@{directive.name}"
end
end

# TODO
class FieldAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class FieldAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class EnumValueAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class EnumValueAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class InputFieldAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class InputFieldAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class DirectiveArgumentAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class DirectiveArgumentAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class FieldArgumentAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class FieldArgumentAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class ObjectTypeAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class ObjectTypeAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class InterfaceTypeAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class InterfaceTypeAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class UnionTypeAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class UnionTypeAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class EnumTypeAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class EnumTypeAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class ScalarTypeAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class ScalarTypeAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class InputObjectTypeAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class InputObjectTypeAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

# TODO
class SchemaAstDirectiveAdded < AbstractChange
def initialize(*)
end
end

# TODO
class SchemaAstDirectiveRemoved < AbstractChange
def initialize(*)
end
end

class DirectiveArgumentAdded < AbstractChange
attr_reader :directive, :argument, :criticality

def initialize(directive, argument)
@criticality = if argument.type.non_null?
Changes::Criticality.breaking
else
Changes::Criticality.non_breaking
end
@directive = directive
@argument = argument
end

def message
"Argument `{argument.name}` was added to directive `{directive.name}`"
end
end



*/
