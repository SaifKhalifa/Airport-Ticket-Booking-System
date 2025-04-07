namespace Airport_Ticket_Booking_System.Services;
public class ConsoleService : IConsoleService
{
    public void Write(string message) => Console.Write(message);
    public void WriteLine(string message) => Console.WriteLine(message);
    public string ReadLine() => Console.ReadLine();
    public void Clear() => Console.Clear();
    public void WaitForKey()
    {
        Console.Write("\nPress any key to continue...");
        Console.ReadKey();
    }
}
