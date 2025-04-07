using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport_Ticket_Booking_System.Models;
internal class Booking
{
    public string BookingId { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Class { get; set; } = "Economy"; // Default to Economy
    public decimal Price { get; set; }
}
