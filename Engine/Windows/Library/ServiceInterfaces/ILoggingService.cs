namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    public interface ILoggingService {

        void LogDebug(string debugStatement);

        void LogError(string error);

        void LogInfo(string header, string info);

        void LogInfo(string info);

        void LogWarning(string warning);
    }
}