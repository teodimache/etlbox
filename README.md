# ETLBox

It's all in the box! Run all your ETL jobs with this awesome C# class library.

## What is ETLBox

ETLBox is a comprehensive C# class library that is able to manage your whole ETL or ELT. You can use it to run some simple (or complex) sql against your database. You can easily manage your database using some simple and easy-to-understand objects. You can even create your own dataflow, where data is send from some source to a target and transformed on it's way. 

## Why ETLBox

Perhaps you are looking for an alternative to Sql Server Integrations Services (SSIS). Or you are searching for framework to create an ETL with C# code. The goal of ETLBox is to provide an easy-to-use but still powerful library to create complex ETL routines and sophisticated data flows.

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

With .net core and .net standard, etlbox will very likely become a self-deploying toolbol – usable where .net core runs. (Work in progress, currently ETLBox is tested only with .NET 4.6.1)

## Overview of functionalities

ETLBox is split into two main components: Control Flow task and Data Flow Tasks.

### Control Flow tasks
Control Flow Tasks gives you control over your database: They allow you to create or delete databases, tables, procedures, schema, ... or other objects in your database. They also can truncate your tables, count rows or execute *any* sql you like. Anything you can script in sql, can be done here - but mostly with only one line of c# code. This improves the readability of your code a lot. 
But Control Flow tasks are not restricted to databases only: you even can process a cube or run an xmla on your Analysis Service.

### Control Flow tasks examples

The easiest way to connect all your tasks to a database is to store the connection string in the Control Flow object
```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("...connection string...")); 
```

Afer this, you can basically you all tasks, e.g.: 

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

### Data flow tasks

Dataflow tasks gives you the ability to create your own pipeline where you can send your data through. Dataflows consists of one or more source elements (like CSV-Files or data derived from a table), some transformations and optionally one or more targets. To create your own data flow , there are three steps necessary: First you defined your dataflow components. After that, you link these components together (each source has an output, each transformation at least one input and one output and each destination has an input). 
After the linking of the components you just tell your source to start retrieving the data. The source will start reading and post its data into the components connected to its output. As soon as connected compoment retrieve any data in their input, they will start theirselves with processing the data and then send it further down the flow to their connected components. The dataflow will finish when all data from the source(s) are read. 
Of course, all data is processed asynchronously. Each compoment has is own set of buffers, so while the source is still reading data the transformations can process it and the destinations can already write the processed chunks into the target. 

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

By default, ETLBox uses NLog. NLog already comes with different log targets that be configured either via your app.config or programatically. Please see the NLog-documentation for a full reference.

ETLBox already comes with NLog as dependency. So you don't have to install or download anything in order to use the logging funtionality. All you have to do is to set up a nlog configuration section in your app.config, and create a target and a logger rule.

This could look like
```xml
<nlog>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="debugger" />
  </rules>
  <targets>
    <target name="debugger" xsi:type="Debugger" />     
  </targets>
</nlog>
```

After adding this section, you will already get some logging output. 

But there is more. If you want to have logging in your database, you can use the CreateLogTables - Task. This task will create two tables: etl.LoadProcess and etl.Log

The etl.LoadProcess contains information about the etl processes that you started programatically with the StartLoadProcessTask. To end or abort a process, you can use the EndLoadProcessTask or AbortLoadProcessTask.

The etl.Log table will store all log message generated from any control flow or data flow task. You can even use your own LogTask to create your own log message in there.

This is an example for using the logging
```C#
StartLoadProcessTask.Start("Process 1 started");

SqlTask.ExecuteNonQuery("some sql", "Select 1 as test");
Sequence.Execute("some custom code", () => { });
LogTask.Warn("Some warning!");

EndLoadProcessTask.End("Process 1 ended successfully");
```

After running this code, you will one line with process information in your etl.LoadProcess Table and several lines of log information (for each task like SqlTask, Sequence or LogTask) in your etl.Log table.

Attention: Before running this code, you must configure a nlog target for your database. Please see the wiki for the complete example. 

### but there is more
a quick overview of available task:
 - CalculateDatabaseHashTask, CleanUpSchemaTask, CreateIndexTask, GetDatabaseListTask, RestoreDatabaseTask, XmlaTask, DropCubeTask, ProcessCubeTask, ConnectionManager (Sql, SMO, AdoMD, AS, File), ControlFlow, Package, Sequence, CustomTask, ...  

## Getting Started

### Prerequisites

We recommend that you have Visual Studio 2017 installed (including the Github extension)

### Using ETLBox

#### Nuget 

ETLBox is available on nuget.

#### Download the sources

Clone the repository
```
git clone https://github.com/roadrunnerlenny/etlbox.git
```

Then, open the downloaded solution file ETLBox.sln with Visual Studio 2015 or higher.
Now you can build the solution, and use it as a reference in your other projects. 

## Going further

To dig deeper into it, have a look at the ETLBox tests within the solution. There is a test for everything that you can do with ETLToolbox.

See the wiki for detailed documentation of every task or component, including examples.
