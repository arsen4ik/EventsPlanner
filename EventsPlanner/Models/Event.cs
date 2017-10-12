using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventsPlanner.Models
{
    public class Event
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime eventDate { get; set; }
        public DateTime createdDate { get; set; }

        public int maxSubscribedUsers { get; set; }


        public virtual ICollection<ApplicationUser> ManageUsers { get; set; }

        public virtual ICollection<ApplicationUser> SubscribedUsers { get; set; }

        public virtual ICollection<EventField> EventFields { get; set; }

        [NotMapped]
        public bool isSubscribed { get; set; }


    }

}