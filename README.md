# Filter Query Langauage to LINQ converter.
Version 9.1.0 supporting .NET 6, 7, 8, 9 and .NET Standard 2.1

Provides helper method to convert Filter Query Language object to a LINQ expression. This expression can then be passed to an IQueryable.Where() 
to provide filtering of the object. The expression tree is built in a way to be type safe based upon the object Type passed into the convert logic.

**New in v9.1.0:** Optional sorting and pagination support!

## Example for web api

### Basic Filtering (Backward Compatible)
``` c#
[HttpPost]
public IEnumerable<User> Post([FromBody] FilterQueryLanguage fql)
{
    var expression = FilterHelper.Convert<User>(fql);
    var results = GetUsers().Where(expression).ToList();

    return results;
}
```

### With Sorting and Pagination (New!)
``` c#
[HttpPost]
public IEnumerable<User> Post([FromBody] FilterQueryLanguage fql)
{
    // ApplyFql automatically handles filtering, sorting, and pagination
    var results = GetUsers().ApplyFql(fql).ToList();

    return results;
}
```

## Understanding Filter Query Language (FQL)
FQL is designed to be a normalized definition of filters that can be applied to a dataset in a serializable format.  The format
allows for flexible filter configuration that still includes order of filter application and nested properties. This result is an
object that can represent the WHERE clause of a SQL while keeping the information needed to display the filters.

### Basic Filtering

``` javascript
  FQL = { // The base FQL wrapping object.
    logic: 'AND', // AND | OR - Used to represent how multiple filters are grouped together. (Default: AND)
    filterQueries: [ 
      logic: 'OR', // Logic used to join filter values on a property together and multiple filters.
      field: 'name', // The property or field to be filtered on. Can be array of fields or nested fields. ex ['user.firstName', 'user.lastName']
      filterItems: [{
        operation: 'EQ', // Logic used in the comparison operation.
        value: 'Jim' // The value to check against.
      }]
    ]
  }
```
As SQL
``` sql
SELECT * FROM USER WHERE [name] = 'Jim';
```

### Filtering with IN-style Logic

``` javascript
// The base FQL wrapping object.
const fql = {
    logic: 'AND',
    filterQueries: [
        {
            logic: 'OR',
            field: 'comment',
            filterItems: [{
                operation: 'CONTAINS',
                value: 'Test'
            }]
        },
        {
            logic: 'OR',
            field: 'color',
            filterItems: [{
                operation: 'EQ',
                value: 'red'
            }, {
                operation: 'EQ',
                value: 'blue'
            }]
        }
    ]
}
```
As SQL
``` sql
SELECT * FROM USER WHERE [comment] LIKE '%Test%' AND ([color] = 'red' OR [color] = 'blue');
-- Equivalent to:
SELECT * FROM USER WHERE [comment] LIKE '%Test%' AND ([color] IN ('red', 'blue'));
```

### Sorting (v9.1.0+)

``` javascript
const fql = {
    logic: 'AND',
    filterQueries: [/* filters */],
    sorting: [
        { field: 'name', direction: 'Ascending' },
        { field: 'age', direction: 'Descending' }
    ]
}
```

**Supported Sort Directions:**
- `Ascending` (or `0`) - A-Z, 0-9
- `Descending` (or `1`) - Z-A, 9-0

**Features:**
- Multiple field sorting (ThenBy support)
- Nested property sorting (e.g., `"user.address.city"`)
- Type-safe sorting

### Pagination (v9.1.0+)

``` javascript
const fql = {
    logic: 'AND',
    filterQueries: [/* filters */],
    sorting: [/* optional sorting */],
    pagination: {
        page: 1,        // 1-based page number
        pageSize: 20    // Number of items per page
    }
}
```

### Complete Example

``` javascript
const fql = {
    logic: 'AND',
    filterQueries: [
        {
            field: 'status',
            filterItems: [{ operation: 'EQ', value: 'Active' }]
        },
        {
            logic: 'OR',
            field: 'name',
            filterItems: [
                { operation: 'CONTAINS', value: 'John' },
                { operation: 'CONTAINS', value: 'Jane' }
            ]
        }
    ],
    sorting: [
        { field: 'name', direction: 'Ascending' }
    ],
    pagination: {
        page: 1,
        pageSize: 10
    }
}
```

As SQL:
``` sql
SELECT * FROM USER 
WHERE [status] = 'Active' 
  AND ([name] LIKE '%John%' OR [name] LIKE '%Jane%')
ORDER BY [name] ASC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
```

## Supported Operations

- `EQ` - Equals
- `NEQ` - Not Equals
- `GT` - Greater Than
- `LT` - Less Than
- `GTE` - Greater Than or Equal
- `LTE` - Less Than or Equal
- `CONTAINS` - String contains (case-sensitive)
- `STARTS` - String starts with
- `ENDS` - String ends with

## Backward Compatibility

All existing code continues to work. The sorting and pagination features are **completely optional**:

``` c#
// Old way - still works
var expression = FilterHelper.Convert<User>(fql);
var results = GetUsers().Where(expression).ToList();

// New way - with optional sorting/pagination
var results = GetUsers().ApplyFql(fql).ToList();
```
