using Airport_Ticket_Booking_System.Models;
using Airport_Ticket_Booking_System.Services;

namespace Airport_Ticket_Booking_System;
internal static class Program
{
    static FlightService flightService;
    static BookingService bookingService;
    static IConsoleService console = new ConsoleService();
    static Passenger passenger;

    static async Task Main()
    {
        flightService = new FlightService();
        bookingService = await BookingService.CreateAsync();

        while (true)
        {
            console.Clear();
            console.WriteLine("============================");
            console.WriteLine("   AIRPORT TICKET BOOKING   ");
            console.WriteLine("============================");
            console.WriteLine("[1] Login as Passenger");
            console.WriteLine("[2] Login as Manager");
            console.WriteLine("[3] Exit");
            console.Write("Select an option: ");

            string? choice = console.ReadLine();
            switch (choice)
            {
                case "1": PassengerLogin(); break;
                case "2": ManagerLogin(); break;
                case "3": Environment.Exit(0); break;
                default:
                    console.WriteLine("Invalid choice! Press Enter to try again.");
                    console.ReadLine();
                    break;
            }
        }
    }

    static void PassengerLogin()
    {
        console.Write("Enter your name: ");
        string? name = console.ReadLine()?.Trim();

        console.Write("Enter your passport number: ");
        string? passport = console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(passport))
        {
            console.WriteLine("Name and Passport Number cannot be empty.");
            console.WaitForKey();
            return;
        }

        passenger = new Passenger { Name = name, PassportNumber = passport };
        PassengerMenu();
    }

    static async void PassengerMenu()
    {
        while (true)
        {
            console.Clear();
            console.WriteLine("======= Passenger Menu =======");
            console.WriteLine("[1] Search Flights");
            console.WriteLine("[2] Book a Flight");
            console.WriteLine("[3] View Bookings");
            console.WriteLine("[4] Cancel Booking");
            console.WriteLine("[5] Modify Booking Class");
            console.WriteLine("[6] Logout");
            console.Write("Select an option: ");

            string choice = console.ReadLine();
            switch (choice)
            {
                case "1":
                    flightService.SearchFlightsCli(flightService);
                    console.WaitForKey();
                    break;

                case "2":
                    console.WriteLine("\nFlights available:");
                    flightService.PrintAllFlights();

                    console.Write("\nEnter flight number to book: ");
                    string flightNumber = console.ReadLine();

                    console.WriteLine("\nClasses Available\n1. Economy\n2. Business\n3. First Class");
                    console.Write("Enter class number to book: ");
                    if (int.TryParse(console.ReadLine(), out int classNum))
                    {
                        await bookingService.BookFlight(passenger, flightService, classNum, flightNumber);
                    }
                    else
                    {
                        console.WriteLine("Invalid class input.");
                    }
                    console.WaitForKey();
                    break;

                case "3":
                    bookingService.PrintBookingsForPassenger(passenger);
                    console.WaitForKey();
                    break;

                case "4":
                    console.WriteLine("Here's your booking:");
                    bookingService.PrintBookingsForPassenger(passenger);

                    console.Write("Enter booking ID to cancel: ");
                    string cancelId = console.ReadLine();
                    await bookingService.CancelBooking(cancelId);
                    console.WaitForKey();
                    break;

                case "5":
                    console.WriteLine("Here's your booking:");
                    bookingService.PrintBookingsForPassenger(passenger);

                    console.Write("Enter booking ID to edit: ");
                    string editId = console.ReadLine();

                    console.WriteLine("\nClass Options\n1. Economy\n2. Business\n3. First Class");
                    console.Write("Enter class number: ");
                    if (int.TryParse(console.ReadLine(), out int newClass))
                    {
                        await bookingService.EditBooking(editId, flightService, newClass);
                    }
                    else
                    {
                        console.WriteLine("Invalid class input.");
                    }
                    console.WaitForKey();
                    break;

                case "6": return;

                default:
                    console.WriteLine("Invalid choice! Press Enter to try again.");
                    console.ReadLine();
                    break;
            }
        }
    }

    static void ManagerLogin()
    {
        console.Write("Enter Manager Password: ");
        string password = console.ReadLine();
        if (password == "1234")
        {
            ManagerMenu();
        }
        else
        {
            console.WriteLine("\aIncorrect Password! Press Enter to return to the main menu.");
            console.ReadLine();
        }
    }

    static void ManagerMenu()
    {
        while (true)
        {
            console.Clear();
            console.WriteLine("======= Manager Menu =======");
            console.WriteLine("[1] View All Bookings");
            console.WriteLine("[2] Import Flights (CSV)");
            console.WriteLine("[3] Logout");
            console.Write("Select an option: ");

            string choice = console.ReadLine();
            switch (choice)
            {
                case "1":
                    bookingService.PrintAllBookings();
                    console.WaitForKey();
                    break;
                case "2":
                    flightService.ImportFlightsFromCsv();
                    console.WaitForKey();
                    break;
                case "3": return;
                default:
                    console.WriteLine("Invalid choice! Press Enter to try again.");
                    console.ReadLine();
                    break;
            }
        }
    }
}