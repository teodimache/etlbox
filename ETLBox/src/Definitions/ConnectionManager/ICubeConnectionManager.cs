namespace ALE.ETLBox {
    public interface ICubeConnectionManager : IConnectionManager {
        void Process();
        void DropIfExists();
        ICubeConnectionManager Clone();
    }
}
