using System;

namespace ALE.ETLBox.ConnectionManager {
    public interface IConnectionManager : IDisposable {
        ConnectionString ConnectionString { get; }
        void Open();
        void Close();       

    }
}
