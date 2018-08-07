namespace ALE.ETLBox {
    public interface IDataFlowTransformation<TInput,TOutput> : IDataFlowLinkSource<TOutput>, IDataFlowLinkTarget<TInput> {
    }
}
