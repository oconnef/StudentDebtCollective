using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace sdc_Web.Pages
{
    public class ContactModel : PageModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string QuestionComment { get; set; }

        public ContactModel() {
            Name = string.Empty;
            Email = string.Empty;
            QuestionComment = string.Empty;
        }

        public void OnGet()
        {

        }
    }
}
