
# Overview Control flow tasks

To get aquainted with ETLBox, you should start with the Control Flow tasks. 

## Idea behind

Control Flow task can be split in "General" task - it is a comprehensive set of task to manage, alter or query a database. 
Tasks are a simple to manage or query or database. The idea behind them is that you don't have to write over and over again the same code again for opening up a connection and doing
some things on database like counting the rows in a table. For instance, the code for establishing a connection and doing a simple row count on a table would look like this:

```C#
string connectionString = "Data Source=.; Database=Sample; Integrated Security=SSPI";
using (SqlConnection con = new SqlConnection(connectionString))
{
   SqlCommand cmd = new SqlCommand("select count(*) from dbo.tbl", con);
   con.Open();
   int numrows = (int)cmd.ExecuteScalar();   
}
```

## Set up a connection

With Control Flow Task you only have to set up your database connection string once, like this:
```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("Data Source=.; Database=Sample; Integrated Security=SSPI""));
```
*The connection will be stored in a static property and used by all subsequent tasks if no other connection is passed when a tasks is executed.*

## Using the RowCountTask

Now you can use a `RowCountTask` to query the number of rows within a table with only one line.

```C#
int count = RowCountTask.Count("dbo.tbl");
```
Internally, an ADO.NET connection is opened up (the default ADO.NET connection pooling is used) and a `select count(*) from dbo.tbl` is executed on the database. 
The result is return from the RowCountTask. 

## Using the instances
[!NOTE]
<For every Control Flow Tasks, there are static accessors to simplify the use of the tasks. In order to have access to all functionalities of a task, sometime you 
have to create a new instance of the task object.>

If you want to do a rowcount with an instance instead of the static accessor, it would look like this:
```C#
RowCountTask task = new RowCountTask("dbo.tbl");
int count = task.Count().Rows;
```

So whenever you don't find a static accessor satisfing your needs, try to create an instance and check the properties and methods of the object.

## Confgure a tasks 

But there is more. Let's assume you want to count the rows on a pretty big table, a "normal" row count perhaps would take some time. So RowCount has a property called
`QuickQueryMode`. If set to true, a sql statement that queries the partition tables is then executed. 

```C#
RowCountTask task = new RowCountTask("dbo.tbl") 
	{ QuickQueryMode = true };
int count = task.Count().Rows;
```

This would give you the same result, but insteal of doing a `select count(*) from dbo.tbl` the following sql is fired on the database
```sql
select cast(sum([rows]) as int) from sys.partitions where [object_id] = object_id(N'dbo.tbl') and index_id in (0,1)
```
