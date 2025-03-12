using Airport_Ticket_Booking_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (File.Exists("Data/flights.csv"))
            {
                var lines = File.ReadAllLines("Data/flights.csv");
                foreach (var line in lines)
                {
                    var data = line.Split(',');
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
        }

        public List<Flight> SearchFlights(string departureCountry, string destinationCountry, DateTime? departureDate = null)
        {
            return flights.Where(f =>
                (string.IsNullOrEmpty(departureCountry) || f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(destinationCountry) || f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                (!departureDate.HasValue || f.DepartureDate.Date == departureDate.Value.Date)
            ).ToList();
        }
    }
}
