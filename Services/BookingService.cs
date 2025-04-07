using Airport_Ticket_Booking_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Airport_Ticket_Booking_System.Services;
internal class BookingService
{
    private List<Booking> _bookings = new List<Booking>();

    public BookingService() { }

    // Factory method to create instance with async file loading
    public static async Task<BookingService> CreateAsync()
    {
        var service = new BookingService();
        await service.LoadBookings();
        return service;
    }

    private async Task LoadBookings()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "bookings.csv");

        if (File.Exists(filePath))
        {
            var lines = (await File.ReadAllLinesAsync(filePath)).Skip(1);
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length < 6) continue;

                _bookings.Add(new Booking
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

    public async Task BookFlight(Passenger passenger, FlightService flightService, int flightClassNumber, string flightNumber)
    {
        var flight = flightService.flights.FirstOrDefault(f => f.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));

        if (flight == null)
        {
            Console.WriteLine("\aFlight not found. Please check the flight number and try again.");
            return;
        }

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

        var booking = new Booking
        {
            BookingId = Guid.NewGuid().ToString(),
            PassengerName = passenger.Name,
            PassportNumber = passenger.PassportNumber,
            FlightNumber = flight.FlightNumber,
            Class = flightClass,
            Price = price
        };

        _bookings.Add(booking);
        await SaveBookings();

        Console.WriteLine($"Booking successful! Booking ID: {booking.BookingId}");
    }

    private async Task SaveBookings()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "bookings.csv");

        try
        {
            var lines = new List<string>
            {
                "BookingId,PassengerName,PassportNumber,FlightNumber,Class,Price"
            };

            lines.AddRange(_bookings.Select(b =>
                $"{b.BookingId},{b.PassengerName},{b.PassportNumber},{b.FlightNumber},{b.Class},{b.Price}"
            ));

            await File.WriteAllLinesAsync(filePath, lines);

            Console.WriteLine("Bookings saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\aAn error occurred while saving bookings: {ex.Message}");
        }
    }

    public void PrintAllBookings()
    {
        string horizontalLine = new string('-', 80);

        if (_bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }

        Console.WriteLine("All Bookings:");
        Console.WriteLine(horizontalLine);
        Console.WriteLine("| Booking ID | Passenger Name | Passport Number | Flight Number | Class      | Price   |");
        Console.WriteLine(horizontalLine);

        foreach (var booking in _bookings)
        {
            Console.WriteLine($"| {booking.BookingId,-10} | {booking.PassengerName,-14} | {booking.PassportNumber,-15} | {booking.FlightNumber,-12} | {booking.Class,-10} | {booking.Price,7:C} |");
        }

        Console.WriteLine(horizontalLine);
    }

    public void PrintBookingsForPassenger(Passenger passenger)
    {
        string horizontalLine = new string('-', 80);

        var passengerBookings = _bookings
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

    public async Task EditBooking(string bookingId, FlightService flightService, int newFlightClassNumber)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase));

        if (booking == null)
        {
            Console.WriteLine("\aPlease check the booking ID and try again.");
            return;
        }

        var flight = flightService.flights
            .FirstOrDefault(f => f.FlightNumber.Equals(booking.FlightNumber, StringComparison.OrdinalIgnoreCase));

        if (flight == null)
        {
            Console.WriteLine("\aFlight associated with this booking no longer exists.");
            return;
        }

        decimal newPrice;
        string newFlightClass;
        switch (newFlightClassNumber)
        {
            case 1:
                newFlightClass = "Economy";
                newPrice = flight.EconomyPrice;
                break;
            case 2:
                newFlightClass = "Business";
                newPrice = flight.BusinessPrice;
                break;
            case 3:
                newFlightClass = "FirstClass";
                newPrice = flight.FirstClassPrice;
                break;
            default:
                Console.WriteLine("Invalid class type. Please choose 1 for Economy, 2 for Business, or 3 for First Class.");
                return;
        }

        booking.Class = newFlightClass;
        booking.Price = newPrice;

        await SaveBookings();

        Console.WriteLine($"Booking updated successfully! New class: {newFlightClass}, New price: {newPrice:C}");
    }

    public async Task CancelBooking(string bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId.Equals(bookingId, StringComparison.OrdinalIgnoreCase));

        if (booking == null)
        {
            Console.WriteLine("\aBooking not found. Please check the booking ID and try again.");
            return;
        }

        _bookings.Remove(booking);

        await SaveBookings();

        Console.WriteLine($"Booking with ID '{bookingId}', canceled successfully!");
    }
}
