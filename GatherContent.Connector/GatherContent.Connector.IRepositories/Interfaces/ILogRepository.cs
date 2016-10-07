namespace GatherContent.Connector.IRepositories.Interfaces
{
    using System;

    public interface ILogRepository
    {
        void Log(string source, string code, Exception ex, string messsage = null);
    }
}