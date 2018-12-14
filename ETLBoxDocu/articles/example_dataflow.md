
# Prerequisites

If you want to run this example, you need a working copy of the latests ETLBox - dll as a reference in your project.
Alternatively, you can use the nuget package manager to get the latest version of ETLBox.

# Example Control Flow

Control Flow tasks are a great way to alter a database and run some sql against it. 

This example will give you a quick example how to create, alter and query a database.

## Namespace

The namespace for all objects in ETLBox is ```ALE.ETLBox```

## Connection String to database
First of all, you will need a connection string tied to your code. To avoid handing over this string again and again, you can preserve it in a static object - the ControlFlow object. It's purpose is to store parameters that are used in every ControlFlow Task. 

```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("Data Source=.;Integrated Security=SSPI;"));
```

## Creating a database ###

You can now simply create a database with

```C#
CreateDatabaseTask.Create("DemoDB");
```

If you want to drop it again, simple add ```C# DropDatabaseTask.Delete("DemoDB");``` before you create. Drop database won't give you an exception, even if the database does not exists. 

Now you need to change the connection string to the database you just created:

```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("Data Source=.;Integrated Security=SSPI;Initial Catalog=DemoDB;"));
```

## Altering the database 

Let's now create a new schema and a table on the database.

```C#
CreateSchemaTask.Create("demo");
CreateTableTask.Create("demo.table1", new List<TableColumn>() {
   new TableColumn(name:"key",dataType:"int",allowNulls:false,isPrimaryKey:true, isIdentity:true),
   new TableColumn(name:"value", dataType:"nvarchar(100)",allowNulls:true)
});
```
There is a ```TableColumn``` object that helps you to define columns in the table. 

## Running sql

The most powerful and comman task that you want to run against a database is sql. It will use the underlying ADO.NET connection manager, which allows you to do almost everything on the database, without the "overhead" and boilerplate code that ADO.NET brings with it. 

```C#
SqlTask.ExecuteNonQuery("Insert data",
   $@"insert into demo.table1 (value) select * from (values ('Ein Text'), ('Noch mehr Text')) as data(v)");
```

## Further task 

There are quite a lot of different control flow tasks that ETLBox comes with. 
A simple example is the RowCountTask, which suprisingly count the rows in a table.

```C#
int count = RowCountTask.Count("demo.table1").Value;
Debug.WriteLine($"Found {count} entries in demo table!");
```
