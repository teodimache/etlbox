# Overview Data Flow

A big part of ETLBox is the Data Flow library. 

## What is a data flow?

You have some data somewhere - stored in some files, a table or somehow in memory. 
Now you want to define a pipeline which takes this data, transforms it "on the fly" and writes it somewhere else 
(again in memory, a database or file or so). On an abstract level this can be seen as an ETL process (ETL = Extract, Transform, Load).
This artice shows how ETLBox can help you with this.

## Namespace
All Data Flow taks reside in the 'ALE.ETLBox.DataFlow' namespace.

## Source components

All dataflow pipeline will need at least one or more sources. Source are basically everything that can read data from someplace 
(e.g. CSV file or a database table) and then post this data into the pipeline. All sources should be able to read data asynchronously. 
That means, while the component reads data from the source, it simultanously sends the already processed data to components that are connected to source.
There are currently two build-in data sources: `CSVSource` and `DBSource`. If you are in need of another source component, you can either extend the 
`CustomSource` or you open an issue in github describing your needs. 

Once a source starts reading data, it will start sending data to its connected components. These could be either a Transoformation or Destination.

## Transformations

Transformations always have at least one input and one output. Inputs can be connected either to other transformation or sources, and the output also to other transformations
or to destinations. 
The purpose of a transformation component is to take the data from its input(s) and post the transformed data to its outputs. This is done on a row-by-row basis.
As soon as there is any data in the input, the transformation will start and post the result to output. 

### Buffering

Every transformation will come with an input. If the components connected to the input post data faster than the transformation
can process it, the buffer will hold this data until the transformation can continue with the next item. This wi

### Non-Blocking and Blocking transformations

Transformation can be either blocking or non-blocking. 

Non-Blocking transformations will start to process data as soon as it finds something
in its input buffer. In the moment where it finds data there, it will  start to transform it and send the data to registered output components. 

Blocking transformation will stop the data processing for the whole pipe - the input buffer will wait until all data has reached the input. This means it wil wait until
all sources in the pipe connected to the transformation have read all data from their source, and all transformations before have processed the incoming data. 
When all data was read from the connected sources and transformations further down the pipe, the blocking transformation will start it transformation. In a transformation
of a blocking transformation, you will therefore have access to all data within the memory. E.g. the sort component is a blocking transformation. It will wait until all
data reached the transformation block - then it will sort it and post the sorted data to its output. 

## Destination components 

Destination components will have normally only one input. They define a target for you data, e.g. a database table or CSV file. Currently, there is `DBDestination` 
and `CSVDestination` implemented. If you are in need of another destination component, you can either extend the `CustomDestination` or you open an 
issue in github.

Every Destination comes with an input buffer. 

While a Destination for csv target will open a file stream where data is written into it as soon as arrives, a DB target will do this batch-by-batch - therefore, 
it will wait until the input buffer reaches the batch size (or the data is the last batch) and then insert it into the database using a bulk insert. 

## A simple dataflow

Let's look at a simple dataflow like this:

CSV File (Source) --> Row transformation --> DB destination.

### Setting up the connection

As the Data Flow Tasks are based on the same foundament like the Control Flow Tasks, you first should set up a connection like you do for
a Control Flow Task.

```C#
ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("Data Source=.;Integrated Security=SSPI;"));
```

### Creating the source 

Now we need to create a source, in this example it could contain order data. This will look like this:


```C#
CSVSource sourceOrderData = new CSVSource("demodata.csv");
```

### Creating the row transformation


We now add a row transformation. The default output format of a `CSVSource` is an string array. In this example, we will convert the csv string array into an `Order` object.

```C#
RowTransformation<string[], Order> rowTrans = new RowTransformation<string[], Order>(
  row => new Order(row)
);    
```

### Creating the destination 

Now we need to create a destination. Notice that the destination is typed with the `Order` object.

```C#
DBDestination<Order> dest = new DBDestination<Order>("dbo.OrderTable");
```

### Linking all together

Until now we have only created the components, but we didn't define the Data Flow pipe. Let's do this now:

```C#
sourceOrderData.LinkTo(rowTrans);
rowTrans.LinkTo(dest);
```

This will create a data  flow pipe CSVSource -> RowTransformation -> DBDestination

### Starting the dataflow

Now we will give the source the command to start reading data. 

```C#
  source.Execute();
``` 

This code will execute as an asynchronous task. If you want to wait for the Data Flow pipeline to finish, add this line to your code

```C#
dest.Wait();
```

When ``dest.Wait()` returns, all data was read from the source and written into the database table. 





