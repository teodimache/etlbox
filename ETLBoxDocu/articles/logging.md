## Logging Control Flow Tasks


By default, ETLBox uses NLog. NLog already comes with different log targets that be configured either via your app.config or programatically. Please see the NLog-documentation for a full reference: (https://nlog-project.org/)[https://nlog-project.org/]

ETLBox already comes with NLog as dependency. So the needed packages will be retrieved from nuget. But in order to use it, you have to set up a nlog configuration section in your app.config, and create a target and a logger rule.

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


### Create log tables 

```C#
CreateLogTablesTask.CreateLog();

CleanUpLogTask.Clean();
RemoveLogTablesTask
ReadLodProcessTableTask
ReadLogTableTask
GetLogAsJSONTask
```

### Start / End load processes

```C#
StartLoadProcessTask.Start("Process 1");
TransferCompletedForLoadProcessTask.Complete();
EndLoadProcessTask.End("Everything successful");
AbortLoadProcessTask.Abort()
```

### Custom log message

```C#
LogTask.Trace("Some text!");
LogTask.Debug("Some text!");
LogTask.Info("Some text!");
LogTask.Warn("Some text!");
LogTask.Error("Some text!");
LogTask.Fatal("Some text!");
```
