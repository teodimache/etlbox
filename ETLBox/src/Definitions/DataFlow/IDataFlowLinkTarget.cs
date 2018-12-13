using System.Threading.Tasks.Dataflow;

namespace ALE.ETLBox {
    public interface IDataFlowLinkTarget<TInput>  {
        ITargetBlock<TInput> TargetBlock { get; }        
    }
}
