namespace Macabresoft.AvaloniaEx;

using System;

/// <summary>
/// Interface for a logging service.
/// </summary>
public interface ILoggingService {
    /// <summary>
    /// Logs an exception.
    /// </summary>
    /// <param name="e">The exception.</param>
    void LogException(Exception e);

    /// <summary>
    /// Logs an exception with a specific message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="e">The exception.</param>
    void LogException(string message, Exception e);

    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="message">The message.</param>
    void LogMessage(string message);
}

/// <summary>
/// A service which logs to configured output. Usually a text file and console output.
/// </summary>
public class LoggingService : ILoggingService {
    /// <inheritdoc />
    public void LogException(Exception e) {
        //TODO: replace
        Console.WriteLine($"An exception has occured: {e.Message}");
    }

    /// <inheritdoc />
    public void LogException(string message, Exception e) {
        //TODO: replace
        Console.WriteLine($"{message}: {e.Message}");
    }

    /// <inheritdoc />
    public void LogMessage(string message) {
        //TODO: replace
        Console.WriteLine(message);
    }
}