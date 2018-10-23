
using System;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace schema_comparator.Tests
{
    [TestFixture()]
    public class SchemaComparatorTests
    {
        readonly SchemaComparator _comperator = new SchemaComparator();

        [Test]
        public void empty_schemas_has_no_changes()
        {
            Result result = _comperator.Compare("", "");
            Assert.AreEqual(0, result.changes.Count);
        }


        [Test]
        public void type_is_removed()
        {
            Result result = _comperator.Compare("type A { a: String } ", " ");
            VerfiyChanges(result, typeof(TypeRemovedChange), "A", Criticality.Breaking, "Type `A` was removed");
        }

        [Test]
        public void type_is_added()
        {

            Result result = _comperator.Compare("", "type A { a: String } ");
            VerfiyChanges(result, typeof(TypeAddedChange), "A", Criticality.NonBreaking, "Type `A` was added");
            
        }

        [Test]
        public void type_has_changed_type()
        {
            Result result = _comperator.Compare("type Query { a: A } type A { a: String } ", "type Query { a: A }  enum A { A_VALUE ANOTHER_VALUE } ");
            VerfiyChanges(result, typeof(TypeKindChanged), "A", Criticality.Breaking, "`A` kind changed from `ObjectGraphType` to `EnumerationGraphType`");

        }


        [Test()]
        [Ignore("Not implemented")]
        public void type_description_has_changed()
        {
            Result result = _comperator.Compare("#desc " + System.Environment.NewLine + "type A { a: String } ", "#newdesc" + System.Environment.NewLine + "type A { a: String } ");
            VerfiyChanges(result, typeof(TypeDescriptionChanged), "a", Criticality.NonBreaking, "");
            
        }


        [Test]
        public void enum_value_added()
        {
            Result result = _comperator.Compare("enum A { A_VALUE }  ", "enum A { A_VALUE, A_VALUE ANOTHER_VALUE } ");

            VerfiyChanges(result, typeof(EnumValueAdded), "A.ANOTHER_VALUE", Criticality.Dangerous, "Enum value `ANOTHER_VALUE` was added to enum `A`");

        }


        [Test]
        public void enum_value_removed()
        {
            Result result = _comperator.Compare("enum A { A_VALUE,  A_VALUE ANOTHER_VALUE }  ", "enum A { A_VALUE } ");
            VerfiyChanges(result, typeof(EnumValueRemoved), "A.ANOTHER_VALUE", Criticality.Breaking, "Enum value `ANOTHER_VALUE` was removed from enum `A`");
        }


        [Test]
        public void object_type_field_removed()
        {
            Result result = _comperator.Compare("type A { a: String, b:String } ", "type A { a: String }");
            VerfiyChanges(result, typeof(FieldRemoved), "A.b", Criticality.Breaking, "Field `b` was removed from object type `A`");

        }


        [Test]
        public void object_type_field_added()
        {
            Result result = _comperator.Compare("type A { a: String } ", "type A { a: String, b:String }");
            VerfiyChanges(result, typeof(FieldAdded), "A.b", Criticality.NonBreaking, "Field `b` was added to object type `A`");

        }


        [Test]
        [Ignore("Not implemented")]
        public void object_type_interface_removed()
        {
            Result result = _comperator.Compare("interface I {a: String} type A implements I { a: String } ", "interface I {a: String} type A { a: String }");
            VerfiyChanges(result, typeof(ObjectTypeInterfaceRemoved), "A.b", Criticality.Breaking, "");

        }


        [Test]
        [Ignore("Not implemented")]
        public void object_type_interface_added()
        {
            Result result = _comperator.Compare("type A { a: String }", "interface I {a: String} type A implements I { a: String } ");
            VerfiyChanges(result, typeof(ObjectTypeInterfaceAdded), "A.b", Criticality.Breaking, "");
            
        }


        [Test]
        public void directive_added()
        {
            Result result = _comperator.Compare("", "directive @A ( a: String ) on FIELD_DEFINITION");
            VerfiyChanges(result, typeof(DirectiveAdded), "A", Criticality.NonBreaking, "Directive `A` was added");
            Assert.AreEqual(1, result.changes.Count);

        }

        [Test]
        public void directive_removed()
        {
            Result result = _comperator.Compare("directive @A ( a: String ) on FIELD_DEFINITION", "");
            VerfiyChanges(result, typeof(DirectiveRemoved), "A", Criticality.Breaking, "Directive `A` was removed");

        }


        [Test]
        public void schema_query_changed()
        {
            Result result = _comperator.Compare("schema { query: MyQuery} type MyQuery {} type MyQueryNext {}", "schema { query: MyQueryNext} type MyQuery {}  type MyQueryNext {}");
            VerfiyChanges(result, typeof(SchemaQueryTypeChanged), "MyQuery", Criticality.Breaking, "Schema query root has changed from `MyQuery` to `MyQueryNext`");

        }

        [Test]
        public void schema_mutation_changed()
        {
            Result result = _comperator.Compare("schema { mutation: MyMutation} type MyMutation {} type MyMutationNext {}", 
                                                "schema { mutation: MyMutationNext} type MyMutation {}  type MyMutationNext {}");
            VerfiyChanges(result, typeof(SchemaMutationTypeChanged), "MyMutation", Criticality.Breaking, "Schema mutation root has changed from `MyMutation` to `MyMutationNext`");

        }

        [Test]
        public void schema_subscription_changed()
        {
            Result result = _comperator.Compare("schema { subscription: MySubscription} type MySubscription {} type MySubscriptionNext {}", 
                                                "schema { subscription: MySubscriptionNext} type MySubscription {}  type MySubscriptionNext {}");
            VerfiyChanges(result, typeof(SchemaSubscriptionTypeChanged), "MySubscription",
                Criticality.Breaking, "Schema subscription type has changed from `MySubscription` to `MySubscriptionNext`");

        }


        [Test]
        public void input_object_field_added()
        {
            Result result = _comperator.Compare("input MessageInput { content: String}",
                                                "input MessageInput { content: String, author: String}");
            VerfiyChanges(result, typeof(InputFieldAdded), "MessageInput.author", Criticality.NonBreaking, "Input field `author` was added to input object type `MessageInput`");
            
        }

        [Test]
        public void input_object_field_removed()
        {
            Result result = _comperator.Compare("input MessageInput { content: String, author: String}",
                                                "input MessageInput { content: String}");
            VerfiyChanges(result, typeof(InputFieldRemoved), "MessageInput.author", Criticality.Breaking, "Input field `author` was removed from input object type `MessageInput`");
            
        }

        [Test]
        public void input_object_field_type_changed()
        {
            Result result = _comperator.Compare("input MessageInput { content: String}",
                                                "input MessageInput { content: Boolean}");

            VerfiyChanges(result, typeof(InputFieldTypeChanged), "MessageInput.content", Criticality.Breaking, "Input field `GraphQL.Types.InputObjectGraphType.content` changed type from `String` to `Boolean`");

        }


        [Test]
        public void input_object_field_defaultvalue_changed()
        {
            Result result = _comperator.Compare("input MessageInput { content: String = \"default\" }",
                                                "input MessageInput { content: String = \"newdefault\" }");

            VerfiyChanges(result, typeof(InputFieldDefaultChanged), "MessageInput.content", 
                Criticality.Dangerous, "Input field `MessageInput.content` default changed from `default` to `newdefault`");
            
        }


        [Test]
        public void type_field_deprecated()
        {
            Result result = _comperator.Compare("type A { b: String }",
                                                "type A { b: String @deprecated}");
            
            VerfiyChanges(result, typeof(FieldDeprecationChanged), "A.b", Criticality.NonBreaking, "Deprecation reason on field `A.b` has changed from `` to `No longer supported`");
        }


        [Test]
        [Ignore("seems like the comparator it ignore lower/upper case differences")]
        public void field_argument_name_casing_changed()
        {
            
            Result result = _comperator.Compare("type Starship {length(Unit: String): Float}",
                "type Starship {Length(Unit: String): Float}");


            VerfiyChanges(result, typeof(FieldDeprecationChanged), "A.b", Criticality.Breaking, "");
        }


        [Test]
        public void field_argument_removed()
        {
            Result result = _comperator.Compare("type Starship { field (a: String, b: String): Float}",
                "type Starship { field(a: String ): Float }");


            VerfiyChanges(result, typeof(FieldArgumentRemoved), "Starship.field.b", Criticality.Breaking, "Argument `b: ` was removed from field `Starship.field`");
        }


        [Test]
        public void field_argument_added()
        {
            Result result = _comperator.Compare("type Starship { field (a: String): Float}",
                "type Starship { field(a: String, b: String ): Float }");

            VerfiyChanges(result, typeof(FieldArgumentAdded), "Starship.field.b", Criticality.Breaking, "Argument `b: String` added to field `Starship.field`");
            
        }


        [Test]
        public void field_nullable_argument_added()
        {
            Result result = _comperator.Compare("type Starship { field (a: String): Float}",
                "type Starship { field(a: String, b: String! ): Float }");

            VerfiyChanges(result, typeof(FieldArgumentAdded), "Starship.field.b", Criticality.NonBreaking, "Argument `b: ` added to field `Starship.field`");
        }


        [Test]
        public void field_argument_type_changed()
        {
            Result result = _comperator.Compare("type Starship { field (a: String): Float}",
                "type Starship { field(a: Float ): Float }");
            
            VerfiyChanges(result, typeof(FieldArgumentTypeChanged), "Starship.field.a", 
                Criticality.Breaking, "Type for argument `a` on field `Starship.field` changed from `String` to `Float`");
            
        }

        [Test]
        public void field_argument_default_value_changed()
        {
            Result result = _comperator.Compare("type Starship { field (a: Int = 10): Float }",
                "type Starship { field(a: Int = 9 ): Float }");
            VerfiyChanges(result, typeof(FieldArgumentDefaultChanged), "Starship.field.a", 
                Criticality.Dangerous, "Default value for argument `a` on field `Starship.field` changed from `10` to `9`");
            
        }

        [Test]
        public void field_argument_default_value_added()
        {
          
           var result = _comperator.Compare("type Starship { field (a: Int ): Float}",
                "type Starship { field(a: Int = 9 ): Float }");

            VerfiyChanges(result, typeof(FieldArgumentDefaultChanged), "Starship.field.a", 
                    Criticality.Dangerous, "Default value `9` was added to argument `a` on field `Starship.field`");
            
        }

        [Test]
        public void field_argument_default_value_removed()
        {

            var result = _comperator.Compare("type Starship { field (a: Int = 9): Float}",
                "type Starship { field(a: Int ): Float }");

            VerfiyChanges(result, typeof(FieldArgumentDefaultChanged), "Starship.field.a",
                Criticality.Dangerous, "Default value for argument `a` on field `Starship.field` changed from `9` to ``");
            
        }




        [Test]
        public void field_argument_type_changed_to_nullable()
        {
            Result result = _comperator.Compare("type Starship { field (a: String!): Float}",
                "type Starship { field(a: String ): Float }");


            VerfiyChanges(result, typeof(FieldArgumentTypeChanged), "Starship.field.a", Criticality.NonBreaking, "Type for argument `a` on field `Starship.field` changed from `` to `String`");
            
        }

        [Test]
        public void field_argument_type_changed_to_nullable_and_type()
        {
            Result result = _comperator.Compare("type Starship { field (a: String!): Float}",
                "type Starship { field(a: Int ): Float }");


            VerfiyChanges(result, typeof(FieldArgumentTypeChanged), "Starship.field.a", Criticality.Breaking, "Type for argument `a` on field `Starship.field` changed from `` to `Int`");
            
        }


        [Test]
        public void field_type_changed()
        {
            Result result = _comperator.Compare("type Starship { field : Float}",
                "type Starship { field : String }");

            VerfiyChanges(result, typeof(FieldTypeChanged), "Starship.field", Criticality.Breaking, "Field `GraphQL.Types.ObjectGraphType.field` changed type from `Float` to `String`");
            
        }

        [Test]
        [Ignore("Not implemented")]
        public void field_type_changed_to_nullable()
        {
            Result result = _comperator.Compare("type Starship { field : Float!}",
                "type Starship { field : Float }");
            
            VerfiyChanges(result, typeof(FieldTypeChanged), "Starship.field", Criticality.NonBreaking, "");
        }
        

        private static void VerfiyChanges(Result result, Type typeOfChange, string path, Criticality criticality, string message)
        {
            Assert.AreEqual(1, result.changes.Count);
            Change change = result.changes[0];
            Assert.AreEqual(path, change.Path, "Path is not as expected");
            Assert.AreEqual(typeOfChange, result.changes[0].GetType(), "The change type is not as expected");
            Assert.AreEqual(criticality == Criticality.Breaking, change.IsBreaking, "Isbreaking was not as expected");
            Assert.AreEqual(criticality == Criticality.Dangerous, change.IsDangerous, "IsDangerous was not as expected");
            Assert.AreEqual(criticality == Criticality.NonBreaking, change.IsNonBreaking, "IsNoneBreaking was not as expected");
            Assert.AreEqual(message, change.Message);
            
        }
    }
}