using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace sdcWeb.Models
{
    public class SubscriptionModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string? EmailAddress { get; set; }
        public int AmountOwed { get; set; }
        public string Lenders { get; set; }
        public bool IsVerified { get; set; }
    }
}
