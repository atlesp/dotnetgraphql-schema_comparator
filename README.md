# DotnetGraphQL::SchemaComparator

The idea and structure of this project taken from the Ruby procject [graphql-schema_comparator](https://github.com/xuorig/graphql-schema_comparator). T

`DotnetGraphQL::SchemaComparator` is a GraphQL Schema comparator.  `DotnetGraphQL::SchemaComparator` takes two GraphQL schemas and outputs a list of changes between versions. This is useful for many things:

  - Breaking Change detection
  - Applying custom rules to schema changes
  - Building automated tools like linters

  
https://travis-ci.com/atlesp/dotnetgraphql-schema_comparator   
  
## Installation

```


## CLI

`DotnetGraphQL::SchemaComparator` comes with a handy CLI to help compare two schemas using the command line.

??how to install??

```
Commands:

```

Where OLD_SCHEMA and NEW_SCHEMA can be a string containing a schema IDL or a filename where that IDL is located.

### Example

```
schema_comparator compare "type Query { a: A } type A { a: String } enum B { A_VALUE }" "type Query { a: A } type A { b: String } enum B { A_VALUE ANOTHER_VALUE }"
⏳  Checking for changes...
🎉  Done! Result:

Detected the following changes between schemas:

🛑  Field `a` was removed from object type `A`
⚠️  Enum value `ANOTHER_VALUE` was added to enum `B`
✅  Field `b` was added to object type `A`
```

## Usage

`DotnetGraphQL::SchemaComparator`, provides a simple api for .Net applications to use.


### GraphQL::SchemaComparator.compare

The compare method takes two arguments, `old_schema` and `new_schema`, the two schemas to compare.

You may provide schema IDL as strings, or provide instances of `GraphQL::Schema`.

The result of `compare` returns a `SchemaComparator::Result` object, from which you can
access information on the changes between the two schemas.

 - `result.IsBreaking()` returns true if any breaking changes were found between the two schemas
 - `result.IsIdentical()` returns true if the two schemas were identical
 - `result.GetBreakingChanges()` returns the list of breaking changes found between schemas.
 - `result.GetNonBreakingChanges` returns the list of non-breaking changes found between schemas.
 - `result.GetDangerousChanges` returns the list of dangerous changes found between schemas.
- `result.changes` returns the full list of change objects.

### Change Objects

`DotnetGraphQL::SchemaComparator` returns a list of change objects. These change objects
all inherit from `Change`

Possible changes are all found in [Changes.cs](model/Changes/changes.cs).

### Change Criticality

Each change object has a `#criticality` method which returns a `Changes::Criticality` object.
This objects defines how dangerous a change is to a schema.




The library is available as open source under the terms of the [MIT License](http://opensource.org/licenses/MIT).
