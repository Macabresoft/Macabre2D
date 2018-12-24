namespace Macabre2D.UI.Services {

    using log4net;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Text;

    public sealed class LoggingService : ILoggingService {
        private static readonly string _headerDivider = $"#{new string('-', 78)}#";
        private readonly ILog _log;

        public LoggingService(ILog log) {
            this._log = log;
        }

        public void LogDebug(string debugStatement) {
            var logEntry = LoggingService.GenerateLogEntry(debugStatement);
            this._log.Info(logEntry);
        }

        public void LogError(string error) {
            var logEntry = LoggingService.GenerateLogEntry(error);
            this._log.Info(logEntry);
        }

        public void LogInfo(string header, string info) {
            var logEntry = LoggingService.GenerateLogEntry(header, info);
            this._log.Info(logEntry);
        }

        public void LogInfo(string info) {
            var logEntry = LoggingService.GenerateLogEntry(info);
            this._log.Info(logEntry);
        }

        public void LogWarning(string warning) {
            var logEntry = LoggingService.GenerateLogEntry(warning);
            this._log.Info(logEntry);
        }

        private static string GenerateLogEntry(string header, string body) {
            var builder = new StringBuilder();
            builder.AppendLine();

            builder.AppendLine(_headerDivider);
            if (!string.IsNullOrEmpty(header)) {
                builder.AppendLine(header);
                builder.AppendLine(_headerDivider);
            }

            builder.AppendLine(body);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string GenerateLogEntry(string body) {
            return LoggingService.GenerateLogEntry(null, body);
        }
    }
}