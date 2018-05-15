using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace schema_comparator.Tests
{
    [TestClass()]
    public class SchemaComparatorTests
    {

        SchemaComparator comperator = new SchemaComparator();

        [TestMethod()]
        public void empty_schemas_has_no_changes()
        {
            Result result = comperator.Compare("", "");
            Assert.AreEqual(0, result.changes.Count);
        }


        [TestMethod()]
        public void type_is_removed()
        {
            Result result = comperator.Compare("type A { a: String } ", " ");
            VerfiyChanges(result, typeof(TypeRemovedChange), "A", Criticality.Breaking);
        }

        [TestMethod()]
        public void type_is_added()
        {

            Result result = comperator.Compare("", "type A { a: String } ");
            VerfiyChanges(result, typeof(TypeAddedChange), "A", Criticality.NonBreaking);
            
        }

        [TestMethod()]
        public void type_has_changed_type()
        {
            Result result = comperator.Compare("type Query { a: A } type A { a: String } ", "type Query { a: A }  enum A { A_VALUE ANOTHER_VALUE } ");
            VerfiyChanges(result, typeof(TypeKindChanged), "A", Criticality.Breaking);

        }


        [TestMethod(), Ignore]
        public void type_description_has_changed()
        {
            Result result = comperator.Compare("#desc " + System.Environment.NewLine + "type A { a: String } ", "#newdesc" + System.Environment.NewLine + "type A { a: String } ");
            VerfiyChanges(result, typeof(TypeDescriptionChanged), "a", Criticality.NonBreaking);
            
        }


        [TestMethod()]
        public void enum_value_added()
        {
            Result result = comperator.Compare("enum A { A_VALUE }  ", "enum A { A_VALUE, A_VALUE ANOTHER_VALUE } ");

            VerfiyChanges(result, typeof(EnumValueAdded), "A.ANOTHER_VALUE", Criticality.Dangerous);

        }


        [TestMethod()]
        public void enum_value_removed()
        {
            Result result = comperator.Compare("enum A { A_VALUE,  A_VALUE ANOTHER_VALUE }  ", "enum A { A_VALUE } ");
            VerfiyChanges(result, typeof(EnumValueRemoved), "A.ANOTHER_VALUE", Criticality.Breaking);

        }


        [TestMethod()]
        public void object_type_field_removed()
        {
            Result result = comperator.Compare("type A { a: String, b:String } ", "type A { a: String }");
            VerfiyChanges(result, typeof(FieldRemoved), "A.b", Criticality.Breaking);

        }


        [TestMethod()]
        public void object_type_field_added()
        {
            Result result = comperator.Compare("type A { a: String } ", "type A { a: String, b:String }");
            VerfiyChanges(result, typeof(FieldAdded), "A.b", Criticality.NonBreaking);

        }


        [TestMethod(), Ignore]
        public void object_type_interface_removed()
        {
            Result result = comperator.Compare("interface I {a: String} type A implements I { a: String } ", "interface I {a: String} type A { a: String }");
            VerfiyChanges(result, typeof(ObjectTypeInterfaceRemoved), "A.b", Criticality.Breaking);

        }


        [TestMethod(), Ignore]
        public void object_type_interface_added()
        {
            Result result = comperator.Compare("type A { a: String }", "interface I {a: String} type A implements I { a: String } ");
            VerfiyChanges(result, typeof(ObjectTypeInterfaceAdded), "A.b", Criticality.Breaking);
            
        }


        [TestMethod()]
        public void directive_added()
        {
            Result result = comperator.Compare("", "directive @A ( a: String ) on FIELD_DEFINITION");
            VerfiyChanges(result, typeof(DirectiveAdded), "A", Criticality.NonBreaking);
            Assert.AreEqual(1, result.changes.Count);

        }

        [TestMethod()]
        public void directive_removed()
        {
            Result result = comperator.Compare("directive @A ( a: String ) on FIELD_DEFINITION", "");
            VerfiyChanges(result, typeof(DirectiveRemoved), "A", Criticality.Breaking);

        }


        [TestMethod()]
        public void schema_query_changed()
        {
            Result result = comperator.Compare("schema { query: MyQuery} type MyQuery {} type MyQueryNext {}", "schema { query: MyQueryNext} type MyQuery {}  type MyQueryNext {}");
            VerfiyChanges(result, typeof(SchemaQueryTypeChanged), "MyQuery", Criticality.Breaking);

        }

        [TestMethod()]
        public void schema_mutation_changed()
        {
            Result result = comperator.Compare("schema { mutation: MyMutation} type MyMutation {} type MyMutationNext {}", 
                                                "schema { mutation: MyMutationNext} type MyMutation {}  type MyMutationNext {}");
            VerfiyChanges(result, typeof(SchemaMutationTypeChanged), "MyMutation", Criticality.Breaking);

        }

        [TestMethod()]
        public void schema_subscription_changed()
        {
            Result result = comperator.Compare("schema { subscription: MySubscription} type MySubscription {} type MySubscriptionNext {}", 
                                                "schema { subscription: MySubscriptionNext} type MySubscription {}  type MySubscriptionNext {}");
            VerfiyChanges(result, typeof(SchemaSubscriptionTypeChanged), "MySubscription", Criticality.Breaking);

        }


        [TestMethod()]
        public void input_object_field_added()
        {
            Result result = comperator.Compare("input MessageInput { content: String}",
                                                "input MessageInput { content: String, author: String}");
            VerfiyChanges(result, typeof(InputFieldAdded), "MessageInput.author", Criticality.NonBreaking);
            
        }

        [TestMethod()]
        public void input_object_field_removed()
        {
            Result result = comperator.Compare("input MessageInput { content: String, author: String}",
                                                "input MessageInput { content: String}");
            VerfiyChanges(result, typeof(InputFieldRemoved), "MessageInput.author", Criticality.Breaking);
            
        }

        [TestMethod()]
        public void input_object_field_type_changed()
        {
            Result result = comperator.Compare("input MessageInput { content: String}",
                                                "input MessageInput { content: Boolean}");

            VerfiyChanges(result, typeof(InputFieldTypeChanged), "MessageInput.content", Criticality.Breaking);

        }


        [TestMethod()]
        public void input_object_field_defaultvalue_changed()
        {
            Result result = comperator.Compare("input MessageInput { content: String = \"default\" }",
                                                "input MessageInput { content: String = \"newdefault\" }");

            VerfiyChanges(result, typeof(InputFieldDefaultChanged), "MessageInput.content", Criticality.Dangerous);
            
        }


        [TestMethod()]
        public void type_field_deprecated()
        {
            Result result = comperator.Compare("type A { b: String }",
                                                "type A { b: String @deprecated}");


            VerfiyChanges(result, typeof(FieldDeprecationChanged), "A.b", Criticality.NonBreaking);
        }

        private static void VerfiyChanges(Result result, Type typeOfChange, String path, Criticality criticality)
        {
            Assert.AreEqual(1, result.changes.Count);
            Change change = result.changes[0];
            Assert.AreEqual(path, change.Path, "Path is not as expected");
            Assert.AreEqual(typeOfChange, result.changes[0].GetType(), "The change type is not as expected");
            Assert.AreEqual(criticality == Criticality.Breaking, change.IsBreaking, "Isbreaking was not as expected");
            Assert.AreEqual(criticality == Criticality.Dangerous, change.IsDangerous, "IsDangerous was not as expected");
            Assert.AreEqual(criticality == Criticality.NonBreaking, change.IsNonBreaking, "IsNoneBreaking was not as expected");
        }
    }
}