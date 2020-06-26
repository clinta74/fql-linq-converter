# Filter Query Langauage to LINQ converter.

Provides helper meathod to convert Filter Query Langauge object to a LINQ expression.  This expression can then be passed to a IQuerable.Where() 
to provide filtering of the object.  The expression tree is built in a way to be type safe based upon the object Type passed into the convert logic.

## Example for web api
``` c#
[HttpPost]
public IEnumerable<User> Post([FromBody] FilterQueryLanguage fql)
{
    var expression = FilterHelper.Convert<User>(fql);
    var results = GetUsers().Where(expression).ToList();

    return results;
}
```

## Understanding Filter Query Language (FQL)
FQL is designed to be a normalized definition of filters that can be applied to a dataset in a serializable format.  The format
allows for flexable filter configuration that still includes order of filter application and nested properties. This result is an
object that can represent the WHERE clause of a SQL while keeping the information needed to display the filters.

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
SELECT * FROM USER WHERE [comment] LIKE '%Test%' AND ([color] IN ('red', 'blue'));
```
