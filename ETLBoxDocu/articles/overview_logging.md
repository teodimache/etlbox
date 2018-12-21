# Logging 

By default, ETLBox uses NLog. NLog already comes with different log targets that be configured either via your app.config or programatically. 
Please see the NLog-documentation for a full reference. [https://nlog-project.org/](https://nlog-project.org/)
ETLBox already comes with NLog as dependency - so the necessary packages will be retrieved from nuget automatically through your package manager. 

## Simple Configuration File

In order to use logging, you have to create a nlog.config file with the exact same name and put it into the root folder of your project. 

A simple nlog.config could look like this

```xml
<?xml version="1.0" encoding="utf-8"?>
<nlog>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="debugger" />
  </rules>
  <targets>
    <target name="debugger" xsi:type="Debugger" />     
  </targets>
</nlog>
```

After adding a file with this configuration, you will already get some logging output to your debugger output. 

## Logging to database

But there is more. If you want to have logging tables in your database, ETLBox comes with some handy stuff that helps you to do this. 

### Extend the nlog.config

As a first step to have nlog log into your database, you must exend your nlog configuration. It should then look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<nlog>
  <targets>
    <target xsi:type="Database" name="database"
       useTransactions="false" keepConnection="true">
      <commandText>
        insert into etl.Log (LogDate, Level, Stage, Message, TaskType, TaskAction, TaskHash, Source, LoadProcessKey)
        select @LogDate
        , @Level
        , cast(@Stage as nvarchar(20))
        , cast(@Message as nvarchar(4000))
        , cast(@Type as nvarchar(40))
        , @Action
        , @Hash
        , cast(@Logger as nvarchar(20))
        , case when @LoadProcessKey=0 then null else @LoadProcessKey end
      </commandText>
      <parameter name="@LogDate" layout="${date:format=yyyy-MM-ddTHH\:mm\:ss.fff}" />
      <parameter name="@Level" layout="${level}" />
      <parameter name="@Stage" layout="${etllog:LogType=Stage}" />
      <parameter name="@Message" layout="${etllog}" />
      <parameter name="@Type" layout="${etllog:LogType=Type}" />
      <parameter name="@Action" layout="${etllog:LogType=Action}" />
      <parameter name="@Hash" layout="${etllog:LogType=Hash}" />
      <parameter name="@LoadProcessKey" layout="${etllog:LogType=LoadProcessKey}" />
      <parameter name="@Logger" layout="${logger}" />
    </target>
  </targets>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="database" />
  </rules>
</nlog>
```
### Copy to output directory

Make sure the config file is copied into the output directory where you build executables are dropped. Your project configuration file .csproj should contain something like this:

```C#
<Itemgroup>
...
  <None Update="nlog.config">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</Itemgroup>
```

### Create database tables

Now you need some tables in the database to store your log information.
You can use the task `CreateLogTables`. This task will create two tables: 
`etl.LoadProcess` and `etl.Log`.
It will also create some stored procedure to access this tables. This can be useful if you want
to log into these table in your sql code or stored procedures.

**Note**: Don't forget the setup the connection for the control flow.

```C#
CreateLogTablesTask.CreateLog();
```

### LoadProcess table

The table etl.LoadProcess contains information about the etl processes that you started programatically with the `StartLoadProcessTask`.
To end or abort a process, you can use the `EndLoadProcessTask` or `AbortLoadProcessTask`. To set the TransferCompletedDate in this table, use
the `TransferCompletedForLoadProcessTask`

This is an example for logging into the load process table.

```C#
StartLoadProcessTask.Start("Process 1 started");
/*..*/
TransferCompletedForLoadProcessTask.Complete();
/*..*/
if (error)
   AbortLoadProcessTask.Abort("This is the abort message");
else 
  EndLoadProcessTask.End("Process 1 ended successfully");
```

### Log Table

The etl.Log table will store all log message generated from any control flow or data flow task. 
You can even use your own LogTask to create your own log message in there.
The following example with create 6 rows in your `etl.Log` table. Everytime a Control Flow Tasks starts, it will create a log entry with an action
'START'. When it's done with its execution, it will create another log entry with action type 'END'

```C#
SqlTask.ExecuteNonQuery("some sql", "Select 1 as test");
Sequence.Execute("some custom code", () => { });
LogTask.Warn("Some warning!");
```

The sql task will produce two log entries - one entry when it started and one entry when it ended its execution.

## Further log tasks

### Clean up or remove log table

You can clean up your log with the CleanUpLogTask. 

```C#
CleanUpLogTask.Clean();
```

Or you can remove the log tables and all its procedure from the database. 

```C#
RemoveLogTablesTask.Remove();
```

### Get log and loadprocess table in JSON

If you want to get the content of the etl.LoadProcess table or etl.Log in JSON-Format, there are two tasks for that:

```
GetLoadProcessAsJSONTask.GetJSON();
GetLogAsJSONTask.GetJSON();
```

### Custom log messages

If you want to create an entry in the etl.Log table (just one entry, no START/END messages) you can do this using the LogTask. 
Also you can define the nlog level. 

```C#
LogTask.Trace("Some text!");
LogTask.Debug("Some text!");
LogTask.Info("Some text!");
LogTask.Warn("Some text!");
LogTask.Error("Some text!");
LogTask.Fatal("Some text!");
```
