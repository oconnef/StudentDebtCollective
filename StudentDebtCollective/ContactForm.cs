using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;

namespace StudentDebtCollective
{
    public static class ContactFormFunction
    {
        [FunctionName("ContactForm")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Contact form submission received.");

            // Parse the request body to determine the form type
            string formType = req.Headers["Form-Type"].ToString();

            switch (formType)
            {
                case "contact":
                    // Parse the request body to get the contact form data
                    ContactForm contactForm;
                    try
                    {
                        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                        contactForm = JsonSerializer.Deserialize<ContactForm>(requestBody);
                    }
                    catch
                    {
                        return new BadRequestResult();
                    }

                    // Send an email with the contact form data
                    await ProcessContactForm(contactForm);
                    break;
                default:
                    return new BadRequestResult();
            }

            // Return a success response
            return new OkResult();
        }

        //private static async Task SendContactFormEmail(ContactForm contactForm)
        //{
        //    string apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        //    string senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
        //    string recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");

        //    var client = new SendGridClient(apiKey);
        //    var message = new SendGridMessage();

        //    message.SetFrom(new EmailAddress(senderEmail));
        //    message.AddTo(new EmailAddress(recipientEmail));
        //    message.SetSubject("Contact Form Submission");

        //    // Compose the email body with the contact form data
        //    string emailBody = $"Name: {contactForm.Name}<br>" +
        //                       $"Email: {contactForm.Email}<br>" +
        //                       $"Question/Comment: {contactForm.QuestionComment}";

        //    message.AddContent(MimeType.Html, emailBody);

        //    await client.SendEmailAsync(message);
        //}

        private static async Task ProcessContactForm(ContactForm contactForm)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("ContactFormData");

            // Create a new table entity with the contact form data
            ContactEntity contactEntity = new ContactEntity()
            {
                Name = contactForm.name,
                Email = contactForm.email,
                QuestionComment = contactForm.questionComment
            };

            // Create the table if it doesn't exist
            await table.CreateIfNotExistsAsync();

            // Insert or replace the table entity
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(contactEntity);
            await table.ExecuteAsync(insertOrReplaceOperation);
        }
    }

    public class ContactForm
    {
        public string name { get; set; }
        public string email { get; set; }
        public string questionComment { get; set; }
    }

    public class ContactEntity : TableEntity
    {
        public ContactEntity()
        {
            PartitionKey = "contact";
            RowKey = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string QuestionComment { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }
        public ETag Etag { get; set; }
    }
}
