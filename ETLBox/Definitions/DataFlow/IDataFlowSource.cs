namespace ALE.ETLBox {
    public interface IDataFlowSource<TOutput> : IDataFlowLinkSource<TOutput> {        
        void ExecuteAsync();
        void LinkTo(IDataFlowLinkTarget<TOutput> target);
    }
}
