using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport_Ticket_Booking_System.Models
{
    class Booking
    {
        public string BookingId { get; set; }
        public string PassengerName { get; set; }
        public string PassportNumber { get; set; }
        public string FlightNumber { get; set; }
        public string Class { get; set; } // Economy, Business, FirstClass
        public decimal Price { get; set; }
    }
}
