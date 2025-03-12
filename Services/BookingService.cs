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
            if (File.Exists("Data/bookings.csv"))
            {
                var lines = File.ReadAllLines("Data/bookings.csv");
                foreach (var line in lines)
                {
                    var data = line.Split(',');
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
    }
}
