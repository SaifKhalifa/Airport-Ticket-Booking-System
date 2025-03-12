using Airport_Ticket_Booking_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport_Ticket_Booking_System.Manager
{
    class ManagerService
    {
        private FlightService flightService = new FlightService();

        public void ImportFlights(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            List<string> errors = new List<string>();

            foreach (var line in lines)
            {
                var data = line.Split(',');

                if (data.Length != 9 || !decimal.TryParse(data[6], out _) || !DateTime.TryParse(data[5], out _))
                {
                    errors.Add($"Invalid data in line: {line}");
                }
            }

            if (errors.Count > 0)
            {
                Console.WriteLine("Errors found in file:");
                errors.ForEach(Console.WriteLine);
            }
            else
            {
                File.Copy(filePath, "Data/flights.csv", true);
                Console.WriteLine("Flights imported successfully.");
            }
        }
    }
}
