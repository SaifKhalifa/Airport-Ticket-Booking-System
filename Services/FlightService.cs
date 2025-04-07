using Airport_Ticket_Booking_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Airport_Ticket_Booking_System.Services;
internal class FlightService
{
    public List<Flight> flights = new List<Flight>();

    public FlightService()
    {
        Task.Run(() => LoadFlights()).Wait(); // Enforced sync call due to naming restriction
    }

    private async Task LoadFlights()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "flights.csv");

        if (File.Exists(filePath))
        {
            Console.WriteLine("flights data file was found!, Loading flights data...");

            var lines = (await File.ReadAllLinesAsync(filePath)).Skip(1); // Skip header
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length < 9) continue;

                flights.Add(new Flight
                {
                    FlightNumber = data[0],
                    DepartureCountry = data[1],
                    DestinationCountry = data[2],
                    DepartureAirport = data[3],
                    ArrivalAirport = data[4],
                    DepartureDate = DateTime.Parse(data[5]),
                    EconomyPrice = decimal.Parse(data[6]),
                    BusinessPrice = decimal.Parse(data[7]),
                    FirstClassPrice = decimal.Parse(data[8])
                });
            }
        }
        else
        {
            Console.WriteLine("\aFlights data file not found.");
        }
    }

    public List<Flight> SearchFlights(
        string departureCountry = null,
        string destinationCountry = null,
        DateTime? departureDate = null,
        string departureAirport = null,
        string arrivalAirport = null,
        string flightClass = null,
        decimal? maxPrice = null)
    {
        return flights.Where(f =>
            (string.IsNullOrEmpty(departureCountry) || f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(destinationCountry) || f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
            (!departureDate.HasValue || f.DepartureDate.Date == departureDate.Value.Date) &&
            (string.IsNullOrEmpty(departureAirport) || f.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(arrivalAirport) || f.ArrivalAirport.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(flightClass) ||
                (flightClass.ToLower() == "economy") ||
                (flightClass.ToLower() == "business") ||
                (flightClass.ToLower() == "first class")) &&
            (!maxPrice.HasValue || (flightClass.ToLower() == "economy" && f.EconomyPrice <= maxPrice) ||
                                    (flightClass.ToLower() == "business" && f.BusinessPrice <= maxPrice) ||
                                    (flightClass.ToLower() == "first class" && f.FirstClassPrice <= maxPrice))
        ).ToList();
    }

    public void SearchFlightsCli(FlightService flightService)
    {
        Console.Clear();
        Console.WriteLine("Search for Available Flights");
        Console.WriteLine("Enter search parameters (leave blank to skip):");

        Console.Write("Departure Country: ");
        string departureCountry = Console.ReadLine();

        Console.Write("Destination Country: ");
        string destinationCountry = Console.ReadLine();

        Console.Write("Departure Date (yyyy-MM-dd): ");
        DateTime? departureDate = null;
        if (DateTime.TryParse(Console.ReadLine(), out DateTime parsedDate))
        {
            departureDate = parsedDate;
        }

        Console.Write("Departure Airport: ");
        string departureAirport = Console.ReadLine();

        Console.Write("Arrival Airport: ");
        string arrivalAirport = Console.ReadLine();

        Console.Write("Class (Economy, Business, First Class): ");
        string flightClass = Console.ReadLine();

        Console.Write("Maximum Price: ");
        decimal? maxPrice = null;
        if (decimal.TryParse(Console.ReadLine(), out decimal parsedPrice))
        {
            maxPrice = parsedPrice;
        }

        var results = flightService.SearchFlights(
            departureCountry,
            destinationCountry,
            departureDate,
            departureAirport,
            arrivalAirport,
            flightClass,
            maxPrice
        );

        if (results.Any())
        {
            Console.WriteLine("Search Results:");
            foreach (var flight in results)
            {
                Console.WriteLine($"Flight Number: {flight.FlightNumber}");
                Console.WriteLine($"Departure: {flight.DepartureCountry} ({flight.DepartureAirport})");
                Console.WriteLine($"Destination: {flight.DestinationCountry} ({flight.ArrivalAirport})");
                Console.WriteLine($"Departure Date: {flight.DepartureDate:yyyy-MM-dd}");
                Console.WriteLine($"Economy Price: {flight.EconomyPrice:C}, Business Price: {flight.BusinessPrice:C}, First Class Price: {flight.FirstClassPrice:C}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No flights found matching your criteria.");
        }
    }

    public void PrintAllFlights()
    {
        string horizontalLine = new string('-', 80);
        if (flights.Count == 0)
        {
            Console.WriteLine("No flights available.");
            return;
        }

        Console.WriteLine("\nAvailable Flights:");
        Console.WriteLine(horizontalLine);
        Console.WriteLine("FlightNumber | Departure -> Arrival | Date       | Economy | Business | FirstClass");
        Console.WriteLine(horizontalLine);

        foreach (var flight in flights)
        {
            Console.WriteLine(
                $"{flight.FlightNumber,-12} | " +
                $"{flight.DepartureAirport} -> {flight.ArrivalAirport,-10} | " +
                $"{flight.DepartureDate.ToShortDateString()} | " +
                $"${flight.EconomyPrice,-6} | ${flight.BusinessPrice,-8} | ${flight.FirstClassPrice,-9}");
        }

        Console.WriteLine(horizontalLine);
    }

    public async void ImportFlightsFromCsv()
    {
        Console.WriteLine("Enter the path to the CSV file to import flights:");
        string importFilePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(importFilePath) || !File.Exists(importFilePath))
        {
            Console.WriteLine("\aInvalid file path or file does not exist.");
            return;
        }

        try
        {
            var lines = (await File.ReadAllLinesAsync(importFilePath)).Skip(1);
            var newFlights = new List<Flight>();

            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length < 9) continue;

                newFlights.Add(new Flight
                {
                    FlightNumber = data[0],
                    DepartureCountry = data[1],
                    DestinationCountry = data[2],
                    DepartureAirport = data[3],
                    ArrivalAirport = data[4],
                    DepartureDate = DateTime.Parse(data[5]),
                    EconomyPrice = decimal.Parse(data[6]),
                    BusinessPrice = decimal.Parse(data[7]),
                    FirstClassPrice = decimal.Parse(data[8])
                });
            }

            if (newFlights.Count == 0)
            {
                Console.WriteLine("No valid flights found in the file.");
                return;
            }

            flights.AddRange(newFlights);

            await SaveFlightsToCsv();

            Console.WriteLine($"{newFlights.Count} flights imported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\aAn error occurred while importing flights: {ex.Message}");
        }
    }

    private async Task SaveFlightsToCsv()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "flights.csv");

        try
        {
            var lines = new List<string>
            {
                "FlightNumber,DepartureCountry,DestinationCountry,DepartureAirport,ArrivalAirport,DepartureDate,EconomyPrice,BusinessPrice,FirstClassPrice"
            };

            lines.AddRange(flights.Select(f =>
                $"{f.FlightNumber},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureAirport},{f.ArrivalAirport},{f.DepartureDate:yyyy-MM-dd},{f.EconomyPrice},{f.BusinessPrice},{f.FirstClassPrice}"
            ));

            await File.WriteAllLinesAsync(filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\aAn error occurred while saving flights: {ex.Message}");
        }
    }
}
