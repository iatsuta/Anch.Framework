namespace SyncWorkflow.States.Output;

public record ConsoleOutput() : DefaultOutput(Console.Out)
{
}

