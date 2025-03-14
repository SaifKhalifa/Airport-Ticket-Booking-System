using Airport_Ticket_Booking_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport_Ticket_Booking_System.Services
{
    class BookingService
    {
        private List<Booking> bookings = new List<Booking>();

        public BookingService()
        {
            LoadBookings();
        }

        private void LoadBookings()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "bookings.csv");

            if (File.Exists(filePath))               
            {
                var lines = File.ReadAllLines("Data/bookings.csv").Skip(1); // Skip header if it exists
                foreach (var line in lines)
                {
                    var data = line.Split(',');
                    if (data.Length < 6) continue; // Ensure data is complete

                    bookings.Add(new Booking
                    {
                        BookingId = data[0],
                        PassengerName = data[1],
                        PassportNumber = data[2],
                        FlightNumber = data[3],
                        Class = data[4],
                        Price = decimal.Parse(data[5])
                    });
                }
            }
            else
            {
                Console.WriteLine("\aBookings data file not found.");
            }
        }

        public void BookFlight(Passenger passenger, FlightService flightService, int flightClassNumber, string flightNumber)
        {
            // Find the flight by flight number
            var flight = flightService.flights.FirstOrDefault(f => f.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

            if (flight == null)
            {
                Console.WriteLine("\aFlight not found. Please check the flight number and try again.");
                return;
            }

            // Determine the price based on the flight class number
            decimal price;
            string flightClass;
            switch (flightClassNumber)
            {
                case 1:
                    flightClass = "Economy";
                    price = flight.EconomyPrice;
                    break;
                case 2:
                    flightClass = "Business";
                    price = flight.BusinessPrice;
                    break;
                case 3:
                    flightClass = "FirstClass";
                    price = flight.FirstClassPrice;
                    break;
                default:
                    Console.WriteLine("Invalid class type. Please choose 1 for Economy, 2 for Business, or 3 for First Class.");
                    return;
            }

            // Create a new booking
            var booking = new Booking
            {
                BookingId = Guid.NewGuid().ToString(),
                PassengerName = passenger.Name,
                PassportNumber = passenger.PassportNumber,
                FlightNumber = flight.FlightNumber,
                Class = flightClass,
                Price = price
            };

            // Add the booking to the list and save
            bookings.Add(booking);
            SaveBookings();

            Console.WriteLine($"Booking successful! Booking ID: {booking.BookingId}");
        }

        private void SaveBookings()
        {
            File.WriteAllLines("Data/bookings.csv", bookings.Select(b =>
                $"{b.BookingId},{b.PassengerName},{b.PassportNumber},{b.FlightNumber},{b.Class},{b.Price}"
            ));
        }

        public void PrintAllBookings()
        {
            string horizontalLine = new string('-', 80);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found.");
                return;
            }

            Console.WriteLine("All Bookings:");
            Console.WriteLine(horizontalLine);
            Console.WriteLine("| Booking ID | Passenger Name | Passport Number | Flight Number | Class      | Price   |");
            Console.WriteLine(horizontalLine);

            foreach (var booking in bookings)
            {
                Console.WriteLine($"| {booking.BookingId,-10} | {booking.PassengerName,-14} | {booking.PassportNumber,-15} | {booking.FlightNumber,-12} | {booking.Class,-10} | {booking.Price,7:C} |");
            }

            Console.WriteLine(horizontalLine);
        }

        public void PrintBookingsForPassenger(Passenger passenger)
        {
            string horizontalLine = new string('-', 80);

            // Filter bookings for the provided passenger
            var passengerBookings = bookings
                .Where(b => b.PassportNumber.Equals(passenger.PassportNumber, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (passengerBookings.Count == 0)
            {
                Console.WriteLine($"No bookings found for passenger: {passenger.Name} (Passport: {passenger.PassportNumber}).");
                return;
            }

            Console.WriteLine($"Bookings for Passenger: {passenger.Name} (Passport: {passenger.PassportNumber})");
            Console.WriteLine(horizontalLine);
            Console.WriteLine("| Booking ID | Passenger Name | Passport Number | Flight Number | Class      | Price   |");
            Console.WriteLine(horizontalLine);

            foreach (var booking in passengerBookings)
            {
                Console.WriteLine($"| {booking.BookingId} | {booking.PassengerName} | {booking.PassportNumber} | {booking.FlightNumber} | {booking.Class} | {booking.Price:C} |");
            }

            Console.WriteLine(horizontalLine);
        }
    }
}
