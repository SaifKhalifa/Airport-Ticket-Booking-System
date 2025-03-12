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

        public void BookFlight(Passenger passenger, Flight flight, string flightClass)
        {
            decimal price = flightClass switch
            {
                "Economy" => flight.EconomyPrice,
                "Business" => flight.BusinessPrice,
                "FirstClass" => flight.FirstClassPrice,
                _ => throw new ArgumentException("Invalid class type")
            };

            var booking = new Booking
            {
                BookingId = Guid.NewGuid().ToString(),
                PassengerName = passenger.Name,
                PassportNumber = passenger.PassportNumber,
                FlightNumber = flight.FlightNumber,
                Class = flightClass,
                Price = price
            };

            bookings.Add(booking);
            SaveBookings();
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
    }
}
