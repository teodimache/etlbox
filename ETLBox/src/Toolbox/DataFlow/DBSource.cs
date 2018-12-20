using ALE.ETLBox.ControlFlow;
using ALE.ETLBox.Helper;
using System;
using System.Threading.Tasks.Dataflow;

namespace ALE.ETLBox.DataFlow {
    /// <summary>
    /// A database source defines either a table or sql query that returns data from a database. While reading the result set or the table, data is asnychronously posted
    /// into the targets.
    /// </summary>    
    /// <typeparam name="TOutput">Type of data output.</typeparam>
    /// <example>
    /// <code>
    /// DBSource<MyRow> source = new DBSource<MyRow>("dbo.table");
    /// source.LinkTo(dest); //Transformation or Destination
    /// source.Execute(); //Start the data flow
    /// </code>
    /// </example>
    public class DBSource<TOutput> : GenericTask, ITask, IDataFlowSource<TOutput> where TOutput : new() {
        /* ITask Interface */
        public override string TaskType { get; set; } = "DF_DBSOURCE";
        public override string TaskName => $"Dataflow: Read DB data from {SourceDescription}";
        public override void Execute() => ExecuteAsync();

        /* Public Properties */       
        public TableDefinition SourceTableDefinition { get; set; }        
        public string Sql { get; set; }
        public string SqlForRead => String.IsNullOrWhiteSpace(Sql) ? $"select {SourceTableDefinition.Columns.AsString()} from " + SourceTableDefinition.Name : Sql;
        public string SourceDescription => String.IsNullOrWhiteSpace(Sql) ? "table " + SourceTableDefinition.Name : "custom sql";
        public ISourceBlock<TOutput> SourceBlock => this.Buffer;

        /* Private stuff */
        internal BufferBlock<TOutput> Buffer { get; set; }
        NLog.Logger NLogger { get; set; }

        public DBSource() {
            NLogger = NLog.LogManager.GetLogger("ETL");
            Buffer = new BufferBlock<TOutput>();
        }

        public DBSource(TableDefinition sourceTableDefinition) : this() {
            SourceTableDefinition = sourceTableDefinition;
        }
        public DBSource(string sql) : this() {
            Sql = sql;
        }

        public void ExecuteAsync() {
            NLogStart();
            ReadAll();
            Buffer.Complete();
            NLogFinish();
        }

        public void ReadAll() {
            new SqlTask() {
                DisableLogging = true,
                DisableExtension = true,
                Sql = SqlForRead,
            }.Query<TOutput>(row => Buffer.Post(row));
        }
         
        public void LinkTo(IDataFlowLinkTarget<TOutput> target) {
            Buffer.LinkTo(target.TargetBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            NLogger.Debug(TaskName + " was linked to Target!", TaskType, "LOG", TaskHash, ControlFlow.ControlFlow.STAGE, ControlFlow.ControlFlow.CurrentLoadProcess?.LoadProcessKey);
        }

        public void LinkTo(IDataFlowLinkTarget<TOutput> target, Predicate<TOutput> predicate) {
            Buffer.LinkTo(target.TargetBlock, new DataflowLinkOptions() { PropagateCompletion = true }, predicate);
            NLogger.Debug(TaskName + " was linked to Target!", TaskType, "LOG", TaskHash, ControlFlow.ControlFlow.STAGE, ControlFlow.ControlFlow.CurrentLoadProcess?.LoadProcessKey);
        }

        void NLogStart() {
            if (!DisableLogging)
                NLogger.Info(TaskName, TaskType, "START", TaskHash, ControlFlow.ControlFlow.STAGE, ControlFlow.ControlFlow.CurrentLoadProcess?.LoadProcessKey);
        }

        void NLogFinish() {
            if (!DisableLogging)
                NLogger.Info(TaskName, TaskType, "END", TaskHash, ControlFlow.ControlFlow.STAGE, ControlFlow.ControlFlow.CurrentLoadProcess?.LoadProcessKey);
        }


    }

}
