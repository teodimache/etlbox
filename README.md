# ETLBox

It's all in the box! Run all your ETL jobs with this awesome C# class library.

## What is ETLBox

ETLBox is a comprehensive C# class library that is able to manage your whole ETL or ELT. You can use it to run some simple (or complex) sql against your database. You can easily manage your database using some simple and easy-to-understand objects. You can even create your own dataflow, where data is send from some source to a target and transformed on it's way. 

### Examples
Execute some sql on the DB
```
SqlTask.ExecuteNonQuery("Do some sql",$@"EXEC dbo.myProc");
```

Create or change a Stored Procedure
```
CRUDProcedureTask.CreateOrAlter("demo.proc1", "select 1 as test");
```

Create a schema and a table
```
CreateSchemaTask.Create("demo");
CreateTableTask.Create("demo.table1", new List<TableColumn>() {
    new TableColumn(name:"key",dataType:"int",allowNulls:false,isPrimaryKey:true, isIdentity:true),
    new TableColumn(name:"value", dataType:"nvarchar(100)",allowNulls:true)
});
```

Logging is as easy as this
```
CreateLogTablesTask.CreateLog();
StartLoadProcessTask.Start("Process 1");
ControlFlow.STAGE = "Staging";
SqlTask.ExecuteNonQuery("some sql", "Select 1 as test");
ControlFlow.STAGE = "DataVault";
Sequence.Execute("some custom code", () => { });
LogTask.Warn("Some warning!");
EndLoadProcessTask.End("Everything successful");
```

a quick overview of available task:
 - CalculateDatabaseHashTask
 - CleanUpSchemaTask
 - CreateIndexTask
 - GetDatabaseListTask
 - RestoreDatabaseTask
 - XmlaTask
 - DropCubeTask
 - ProcessCubeTask
 - ConnectionManager (Sql, SMO, AdoMD, AS, File)
 - ControlFlow
 - Package
 - Sequnce
 - CustomTask
 - ...  

## Getting Started

### Prerequisites

We recommend that you have Visual Studio 2017 installed (including the Github extension)

### Installing

Clone the repository
```
git clone https://github.com/roadrunnerlenny/etlbox.git
```

Open the download solution file ETLBox.sln with Visual Studio.
Build the solution. Add it as a reference to other project.

If you want to see a demo, have a look at the ETLBoxDemo project.

## Going further

To dig deeper into it, have a look at the ETLBox tests within the solution. There is a test for everything that you can do with ETLToolbox.

See the wiki for detailed documentation of every task or component.
