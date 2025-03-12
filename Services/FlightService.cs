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

        public List<Flight> SearchFlights(string departureCountry, string destinationCountry, DateTime? departureDate = null)
        {
            return flights.Where(f =>
                (string.IsNullOrEmpty(departureCountry) || f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(destinationCountry) || f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                (!departureDate.HasValue || f.DepartureDate.Date == departureDate.Value.Date)
            ).ToList();
        }

        // New Method: Print all available flights
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