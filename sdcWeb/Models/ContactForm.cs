namespace sdcWeb.Models
{
    public class ContactFormModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string QuestionComment { get; set; }

        public ContactFormModel()
        {
            Name = string.Empty;
            Email = string.Empty;
            QuestionComment = string.Empty;
        }
    }
}
