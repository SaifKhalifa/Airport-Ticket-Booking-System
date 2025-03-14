using Airport_Ticket_Booking_System.Models;
using Airport_Ticket_Booking_System.Services;

namespace Airport_Ticket_Booking_System
{
    internal class Program
    {
        static FlightService flightService = new FlightService();
        static BookingService bookingService = new BookingService();
        static Passenger passenger;
        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("============================");
                Console.WriteLine("   AIRPORT TICKET BOOKING   ");
                Console.WriteLine("============================");
                Console.WriteLine("[1] Login as Passenger");
                Console.WriteLine("[2] Login as Manager");
                Console.WriteLine("[3] Exit");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": PassengerLogin(); break;
                    case "2": ManagerLogin(); break;
                    case "3": Environment.Exit(0); break;

                    default: Console.WriteLine("Invalid choice! Press Enter to try again."); Console.ReadLine(); break;
                }
            }
        }

        static void PassengerLogin()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            Console.Write("Enter your passport number: ");
            string passport = Console.ReadLine();

            passenger = new Passenger { Name = name, PassportNumber = passport };
            PassengerMenu();
        }

        static void PassengerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("======= Passenger Menu =======");
                Console.WriteLine("[1] Search Flights");
                Console.WriteLine("[2] Book a Flight");
                Console.WriteLine("[3] View Bookings");
                Console.WriteLine("[4] Cancel Booking");
                Console.WriteLine("[5] Modify Booking Class");
                Console.WriteLine("[6] Logout");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        {
                            flightService.SearchFlightsCli(flightService);

                            WaitForKey();
                            break;
                        }
                    case "2":
                        {
                            Console.WriteLine("\nFLights available:");
                            flightService.PrintAllFlights();

                            Console.Write("\nEnter flight number to book: ");
                            string flightNumber = Console.ReadLine();

                            Console.Write("\nClasses Available" 
                                            + "\n1. Economy."
                                            + "\n2. Bussiness."
                                            + "\n3. First Class.");

                            Console.WriteLine("\nEnter class number to book:");
                            int flightClassNumber = int.Parse(Console.ReadLine());

                            bookingService.BookFlight(passenger, flightService, flightClassNumber, flightNumber);
                            WaitForKey();
                            break;
                        }
                    case "3":
                        {
                            bookingService.PrintBookingsForPassenger(passenger);

                            WaitForKey();
                            break;
                        }
                    case "4":
                        {
                            Console.WriteLine("Here's your booking:");
                            bookingService.PrintBookingsForPassenger(passenger);

                            Console.WriteLine("Enter booking ID to edit: ");
                            string bookingId = Console.ReadLine();

                            bookingService.CancelBooking(bookingId);
                            WaitForKey();
                            break;
                        }
                    case "5":
                        {
                            Console.WriteLine("Here's your booking:");
                            bookingService.PrintBookingsForPassenger(passenger);

                            Console.WriteLine("Enter booking ID to edit: ");
                            string bookingId = Console.ReadLine();

                            Console.Write("\nClasses Number"
                                            + "\n1. Economy."
                                            + "\n2. Bussiness."
                                            + "\n3. First Class.");

                            Console.WriteLine("\nEnter class number to book:");
                            int flightClassNumber = int.Parse(Console.ReadLine());

                            bookingService.EditBooking(bookingId, flightService, flightClassNumber);
                            WaitForKey();
                            break;
                        }
                    case "6": return;
                    default:
                        {
                            Console.WriteLine("Invalid choice! Press Enter to try again.");
                            Console.ReadLine();
                            break;
                        }
                }
            }
        }

        static void ManagerLogin()
        {
            Console.Write("Enter Manager Password: ");
            string password = Console.ReadLine();
            if (password == "1234")
            {
                ManagerMenu();
            }
            else
            {
                Console.WriteLine("\aIncorrect Password! Press Enter to return to the main menu.");
                Console.ReadLine();
            }
        }

        static void ManagerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("======= Manager Menu =======");
                Console.WriteLine("[1] View All Bookings");
                Console.WriteLine("[2] Import Flights (CSV)");
                Console.WriteLine("[3] Logout");
                Console.Write("Select an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        {
                            bookingService.PrintAllBookings();
                            WaitForKey();
                            break;
                        }
                    case "2":
                        {
                            flightService.ImportFlightsFromCsv();

                            WaitForKey();
                            break;
                        }
                    case "3": return;
                    default:
                        {
                            Console.WriteLine("Invalid choice! Press Enter to try again.");
                            Console.ReadLine();
                            break;
                        }
                }
            }
        }

        static void WaitForKey()
        {
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
