using System.Threading.Tasks.Dataflow;

namespace ALE.ETLBox {
    public interface IDataFlowLinkSource<TOutput>  {
        ISourceBlock<TOutput> SourceBlock { get; }        
    }
}
