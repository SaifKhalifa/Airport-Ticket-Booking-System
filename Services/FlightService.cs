using Airport_Ticket_Booking_System.Models;

namespace Airport_Ticket_Booking_System.Services
{
    class FlightService
    {
        private List<Flight> flights = new List<Flight>();

        public FlightService()
        {
            LoadFlights();
        }

        private void LoadFlights()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "flights.csv");

            if (File.Exists(filePath))
            {
                Console.WriteLine("flights data file was found!, Loading flights data...");

                var lines = File.ReadAllLines("Data/flights.csv").Skip(1); // Skip header if it exists
                foreach (var line in lines)
                {
                    var data = line.Split(',');
                    if (data.Length < 9) continue; // Ensure data is complete

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
                (string.IsNullOrEmpty(departureCountry) || f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) 
                &&
                (string.IsNullOrEmpty(destinationCountry) || f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) 
                &&
                (!departureDate.HasValue || f.DepartureDate.Date == departureDate.Value.Date) 
                &&
                (string.IsNullOrEmpty(departureAirport) || f.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) 
                &&
                (string.IsNullOrEmpty(arrivalAirport) || f.ArrivalAirport.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase)) 
                &&
                (string.IsNullOrEmpty(flightClass)
                || 
                (flightClass.ToLower() == "economy")
                ||
                (flightClass.ToLower() == "business")
                ||
                (flightClass.ToLower() == "first class"))
                &&
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

            // Call the SearchFlights method
            var results = flightService.SearchFlights(
                departureCountry,
                destinationCountry,
                departureDate,
                departureAirport,
                arrivalAirport,
                flightClass,
                maxPrice
            );

            // Display results
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
    }
}