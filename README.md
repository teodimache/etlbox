# ETLBox

It's all in the box! Run all your ETL jobs with this awesome C# class library.

## What is ETLBox

ETLBox is a comprehensive C# class library that is able to manage your whole ETL or ELT. You can use it to run some simple (or complex) sql against your database. You can easily manage your database using some easy-to-use and easy-to-understand objects. You can even create your own dataflow, where data is send from a source to a target and transformed on its way. 

## Why ETLBox

Perhaps you are looking for an alternative to Sql Server Integrations Services (SSIS). Or you are searching for framework to create ETL with C# code. The goal of ETLBox is to provide an easy-to-use but still powerful library with which you can create complex ETL routines and sophisticated data flows.

## Advantages of using ETLBox

### Build ETL in C#

Code your ETL with a language fittingyour team’s skills and that is coming with a mature toolset

### Run locally

Develop and test your ETL code locally on your desktop using your existing development & debugging tools.

### Process In-Memory

ETLBox comes with dataflow components that allow in-memory processing which is much faster than storing data on disk and processing later. 

### Know your errors

When exceptions are raised you get the exact line of code where your ETL stopped, including a hands-on description of the error.

### Manage Change

Track you changes with git (or other source controls), code review your etl logic, and use your existing CI/CD processes.

### Embedded or standalone (coming soon)

With .net core and .net standard, etlbox will very likely become a self-deploying toolbol – usable where .net core runs. (Work in progress, currently ETLBox is maintained in with .NET 4.6.1, but complies with current .NET Standard)

# Overview of functionalities

ETLBox is split into two main components: Control Flow Tasks and Data Flow Tasks.

## Control Flow Tasks

### Control Flow Tasks overview 

Control Flow Tasks gives you control over your database: They allow you to create or delete databases, tables, procedures, schemas, ... or other objects in your database. With these tasks you also can truncate your tables, count rows or execute *any* sql you like. Anything you can script in sql can be done here - but mostly with only one line of easy-to-read C# code. This improves the readability of your code a lot, and gives you more time to focus on your business logic. 

But Control Flow tasks are not restricted to databases only: you even can process a cube or run an XMLA on your Sql Server Analysis Service.

### Control Flow Tasks examples

The easiest way to connect all your tasks to a database is to store the connection string in the Control Flow object

```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("...connection string...")); 
```

Afer this, you can basically do all tasks, e.g.: 

Execute some sql on the DB
```C#
SqlTask.ExecuteNonQuery("Do some sql",$@"EXEC dbo.myProc");
```

Count rows in a table
```C#
int count = RowCountTask.Count("demo.table1").Value; 
```

Create or change a Stored Procedure
```C#
CRUDProcedureTask.CreateOrAlter("demo.proc1", "select 1 as test");
```

Create a schema and a table
```C#
CreateSchemaTask.Create("demo");
CreateTableTask.Create("demo.table1", new List<TableColumn>() {
    new TableColumn(name:"key",dataType:"int",allowNulls:false,isPrimaryKey:true, isIdentity:true),
    new TableColumn(name:"value", dataType:"nvarchar(100)",allowNulls:true)
});
```

## Data flow Tasks

### Data Flow Tasks overview

Dataflow tasks gives you the ability to create your own pipeline where you can send your data through. Dataflows consists of one or more source element (like CSV files or data derived from a table), some transformations and optionally one or more target. To create your own data flow , you need to accomplish three steps: 
- First you define your dataflow components.
- Then, you link these components together (each source has an output, each transformation at least one input and one output and each destination has an input).
- After the linking of the components you just tell your source to start retrieving the data. 

The source will start reading and post its data into the components connected to its output. As soon as a connected component retrieves any data in its input, the component will start with processing the data and then send it further down the line to its connected components. The dataflow will finish when all data from the source(s) are read and received from the destinations.

Of course, all data is processed asynchronously. Each compoment has its own set of buffers, so while the source is still reading data the transformations already can process it and the destinations can start writing the processed chunks into their target. 

### Data flow tasks examples

It's easy to create your own data flow pipeline. 

Just create a source, some transformation and a destination. 

```C#
DBSource<MySimpleRow> source = new DBSource<MySimpleRow>("select * from dbo.Source");
RowTransformation<MySimpleRow, MySimpleRow> trans = new RowTransformation<MySimpleRow, MySimpleRow>(
    myRow => {  
        myRow.Value2 += 1;
        return myRow;
    });
DBDestination<MySimpleRow> dest = new DBDestination<MySimpleRow>("dbo.Destination");
```

Now link these pipeline elements together. 

```C#
source.LinkTo(trans);
trans.LinkTo(dest);
```

Finally, start the dataflow at the source and wait for your destination to rececive all data (and the completion message from the source).

```C#
source.Execute();
dest.Wait();
```

### Logging 

By default, ETLBox uses NLog. ETLBox already comes with NLog as dependency. So the needed packages will be retrieved from nuget. In order to have the logging activating, you just have to set up a nlog configuration section in your app.config, and create a target and a logger rule. After adding this, you will already get some logging output. 

Also you can create log tables within the database that holds all log information. The task CreateLogTablesTask will do that for you.
Once the tables exists, etlbox will log into them.

Use can then use the [ETLBox LogViewer](https://github.com/roadrunnerlenny/etlboxlogviewer) to easily access and analyze your logs.

## There is more
A quick overview of some of the available tasks:
 - AddFileGroupTask
 - CalculateDatabaseHashTask
 - CleanUpSchemaTask
 - CreateDatabaseTask
 - CreateIndexTask
 - CreateSchemaTask
 - CreateTableTask
 - CRUDProcedureTask
 - CRUDViewTask
 - DropDatabaseTask
 - DropTableTask
 - GetDatabaseListTask
 - RestoreDatabaseTask
 - RowCountTask
 - SqlTask
 - TruncateTableTask
 - XmlaTask
 - DropCubeTask
 - ProcessCubeTask
 - ConnectionManager (Sql, SMO, AdoMD, AS, File)
 - ControlFlow
 - Package
 - Sequence
 - CustomTask
 - BlockTransformation
 - CSVSource, DBSource, CustomSource
 - DBDestination, CustomDestination
 - Lookup
 - MergeJoin
 - Multicast
 - RowTransformation
 - Sort
 - .. and much more

## Getting Started

### Prerequisites

We recommend that you have Visual Studio 2017 installed (including the Github extension)

### Using ETLBox

#### Variant 1: Nuget 

ETLBox is available on nuget. Just add the package to your project via your nuget package manager

#### Variant 2: Download the sources

Clone the repository
```
git clone https://github.com/roadrunnerlenny/etlbox.git
```

Then, open the downloaded solution file ETLBox.sln with Visual Studio 2015 or higher.
Now you can build the solution, and use it as a reference in your other projects. 
Feel free to make changes or to fix bug. Every particiation in this open source project is appreciated.

## Going further

To dig deeper into it, have a look at the ETLBox tests within the solution. There is a test for (almost) everything that you can do with ETLToolbox.

See the [ETLBox Wiki](https://github.com/roadrunnerlenny/etlbox/wiki) for detailed documentation of every task or component, including examples.
