using System.ComponentModel.DataAnnotations;

namespace sdcWeb.Models
{
    public class SignUpShortFormModel
    {
        [Display(Name="Email Address")]
        public string? EmailAddress { get; set; }
    }
}
