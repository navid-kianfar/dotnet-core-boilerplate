using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Fixtures;

namespace MyApplication.Abstraction.Implementation;

internal class ConsoleLogger : ILoggerService
{
    public Task Log(string message, string section)
    {
        return HandleError(ConsoleColor.DarkYellow, message, section);
    }

    public Task Info(string message, string section)
    {
        return HandleError(ConsoleColor.Blue, message, section);
    }

    public Task Debug(string message, string section)
    {
        return HandleError(ConsoleColor.Magenta, message, section);
    }

    public Task Error(string message, string section, Exception? ex = null)
    {
        return HandleError(ConsoleColor.Red, ex?.Message ?? message, section);
    }

    private Task HandleError(ConsoleColor color, string message, string section)
    {
        Console.ResetColor();
        Console.WriteLine(GeneralConstants.LogLineSeparator);
        Console.ForegroundColor = color;
        Console.WriteLine(section);
        Console.ResetColor();
        Console.WriteLine(message);
        Console.WriteLine(GeneralConstants.LogLineSeparator);
        return Task.CompletedTask;
    }
}