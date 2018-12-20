using CsvHelper;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ALE.ETLBox.DataFlow {
    /// <summary>
    /// Reads data from a csv source. While reading the data from the file, data is also asnychronously posted into the targets.
    /// </summary>
    /// <example>
    /// <code>
    /// CSVSource source = new CSVSource("demodata.csv");
    /// source.LinkTo(dest); //Link to transformation or destination
    /// source.Execute(); //Start the dataflow
    /// </code>
    /// </example>
    public class CSVSource : GenericTask, ITask, IDataFlowSource<string[]> {
        /* ITask Interface */
        public override string TaskType { get; set; } = "DF_CSVSOURCE";
        public override string TaskName => $"Dataflow: Read CSV Source data from file: {FileName}";
        public override void Execute() => ExecuteAsync();

        /* Public properties */        
        public int SourceCommentRows { get; set; } = 0;
        public bool TrimFields { get; set; } = true;
        public bool TrimHeaders { get; set; } = true;
        public string Delimiter { get; set; } = ",";
        public char Quote { get; set; } = '"';
        public bool AllowComments { get; set; } = true;
        public char Comment { get; set; } = '/';
        public bool SkipEmptyRecords { get; set; } = true;
        public bool IgnoreBlankLines { get; set; } = true;
        string FileName { get; set; }
        public string[] FieldHeaders { get; private set; }
        
        public bool IsHeaderRead => FieldHeaders != null;
        public ISourceBlock<string[]> SourceBlock => this.Buffer;

        /* Private stuff */
        CsvReader CsvReader { get; set; }
        StreamReader StreamReader { get; set; }     
        BufferBlock<string[]> Buffer { get; set; }
        NLog.Logger NLogger { get; set; }

        public CSVSource() {
            NLogger = NLog.LogManager.GetLogger("ETL");
        }

        public CSVSource(string fileName) : this(){
            FileName = fileName;
            Buffer = new BufferBlock<string[]>();
        }

        public void ExecuteAsync() {
            NLogStart();
            Open();
            ReadAll().Wait();
            Buffer.Complete();
            Close();
            NLogFinish();
        }

        private void Open() {
            StreamReader = new StreamReader(FileName, Encoding.UTF8);
            SkipSourceCommentRows();
            CsvReader = new CsvReader(StreamReader);
            ConfigureCSVReader();
        }
        private void SkipSourceCommentRows() {
            for (int i = 0; i < SourceCommentRows; i++)
                StreamReader.ReadLine();
        }

        private async Task ReadAll() {
            CsvReader.Read();
            CsvReader.ReadHeader();
            FieldHeaders = CsvReader.Context.HeaderRecord;
            while (CsvReader.Read()) {
                string[] line = CsvReader.Context.Record;
                await Buffer.SendAsync(line);
            }
        }

        private void ConfigureCSVReader() {
            CsvReader.Configuration.Delimiter = Delimiter;
            CsvReader.Configuration.Quote = Quote;
            CsvReader.Configuration.AllowComments = AllowComments;
            CsvReader.Configuration.Comment = Comment;            
            CsvReader.Configuration.IgnoreBlankLines = IgnoreBlankLines;            
            CsvReader.Configuration.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
            CsvReader.Configuration.Encoding = Encoding.UTF8;            
        }

        private void Close() {
            CsvReader?.Dispose();
            CsvReader = null;
            StreamReader?.Dispose();
            StreamReader = null;
        }

        public void LinkTo(IDataFlowLinkTarget<string[]> target) {
            Buffer.LinkTo(target.TargetBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            NLogger.Debug(TaskName + " was linked to Target!", TaskType, "LOG", TaskHash, ControlFlow.ControlFlow.STAGE, ControlFlow.ControlFlow.CurrentLoadProcess?.LoadProcessKey);
        }

        public void LinkTo(IDataFlowLinkTarget<string[]> target, Predicate<string[]> predicate) {
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
