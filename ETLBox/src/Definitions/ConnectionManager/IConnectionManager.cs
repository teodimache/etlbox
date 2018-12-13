using System;

namespace ALE.ETLBox {
    public interface IConnectionManager : IDisposable {
        ConnectionString ConnectionString { get; }
        void Open();
        void Close();       

    }
}
