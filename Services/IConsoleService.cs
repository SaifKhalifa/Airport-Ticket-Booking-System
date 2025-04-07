namespace Airport_Ticket_Booking_System.Services;
public interface IConsoleService
{
    void Write(string message);
    void WriteLine(string message);
    string ReadLine();
    void Clear();
    void WaitForKey();
}
