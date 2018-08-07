namespace ALE.ETLBox {
    public interface IDataFlowDestination<TInput> : IDataFlowLinkTarget<TInput> {        
        void Wait();
    }
}
