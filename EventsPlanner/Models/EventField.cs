using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsPlanner.Models
{
    public class EventField
    {
        public int id { get; set; }
        public int eventId { get; set; }
        public string name { get; set; }
        public string value { get; set; }

        public Event CurrentEvent { get; set; }

    }
}