# ETLBox overview

## ControlFlow Tasks

### General 

#### Set overall connection to database

```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("Data Source=.;"));
```

#### Create/drop database

```C#
CreateDatabaseTask.Create("DemoDB");
DropDatabaseTask.Delete("DemoDB");
```

#### Create or clean schema

```C#
CreateSchemaTask.Create("demo");
CleanUpSchemaTask.CleanUp("demo");
```

#### Create table

```C#
CreateTableTask.Create("demo.table1", new List<TableColumn>() {
                new TableColumn(name:"key",dataType:"int",allowNulls:false,isPrimaryKey:true, isIdentity:true),
                new TableColumn(name:"value", dataType:"nvarchar(100)",allowNulls:true)
            });
```

#### Custom Sql

```C#
SqlTask.ExecuteNonQuery("Description","insert into demo.table1 select * from demo.table2");
```

#### Row count

```C#
int count = RowCountTask.Count("demo.table1").Value;
```

#### Connection Managers

Sql Connection Manager (ADO.NET)
```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("...connection string..."));
```

SMO Connection Managers (Sql Server Managed Objects)
```C#
ControlFlow.CurrentDbConnection = new SMOConnectionManager(new ConnectionString("...connection string..."));
SqlTask.ExecuteNonQuery("sql with go keyword", @"insert into demo.table1 (value) select '####'; go 2");
```

Adomd Connection Manager
```C#
ControlFlow.CurrentAdomdConnection = new AdomdConnectionManager(new ConnectionString("...connection string...");
```

AS Connection Manager 
```C#
ControlFlow.CurrentASConnection = new ASConnectionManager(new ConnectionString("...connection string..."));
```


#### Truncate table

```C#
TruncateTableTask.Truncate("demo.table1");
```

#### Add Filegroup

```C#
AddFileGroupTask.AddFileGroup("FGName", "DemoDB", "200MB", "10MB", isDefaultFileGroup: true);
```

#### Create / Update procedure

```C#
CRUDProcedureTask.CreateOrAlter("demo.proc1", "select 1 as test");
```

### Logging 

#### Create log tables 

```C#
CreateLogTablesTask.CreateLog();

CleanUpLogTask.Clean();
RemoveLogTablesTask
ReadLodProcessTableTask
ReadLogTableTask
GetLogAsJSONTask
```

#### Start / End load processes

```C#
StartLoadProcessTask.Start("Process 1");
TransferCompletedForLoadProcessTask.Complete();
EndLoadProcessTask.End("Everything successful");
AbortLoadProcessTask.Abort()
```

#### Custom log message

```C#
LogTask.Trace("Some text!");
LogTask.Debug("Some text!");
LogTask.Info("Some text!");
LogTask.Warn("Some text!");
LogTask.Error("Some text!");
LogTask.Fatal("Some text!");
```

#### Calculate database hash

```C#
CalculateDatabaseHashTask.Calculate(new List<string>() { "demo", "dbo" });
```

#### Create index

```C#
CreateIndexTask.Create("indexname","tablename", indexColumns)
```

#### List all databases

```C#
GetDatabaseListTask.List();
```

#### Restore database

```C#
RestoreDatabaseTask
```

#### XMLA

```C#
XmlaTask.ExecuteNonQuery("some xmla","xmla goes here...")
```

#### Cube management 

```C#
DropCubeTask.Execute("Cube");
ProcessCubeTask.Process("Cube");
```

#### 

```C#

```





