using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.Storage;
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
    public class SubscriptionForm
    {
        public string name { get; set; }
        public string email { get; set; }
        public int amountOwed { get; set; }
        public string lenders { get; set; }
    }

    public class SubscriptionEntity : TableEntity
    {
        public SubscriptionEntity()
        {
            PartitionKey = "subscriptions";
            RowKey = Guid.NewGuid().ToString();
            TimeStamp = DateTime.UtcNow;
            IsVerified = false;
            VerificationToken = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public int AmountOwed { get; set; }
        public string Lenders { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? TimeStamp { get; set; }
        public ETag Etag { get; set; }
        public string VerificationToken { get; set; }

    }

    public static class SubscriptionManagement
    {
        [FunctionName("SubscriptionForm")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Subscription form submission received.");

            // Parse the request body to determine the form type
            string formType = req.Headers["Form-Type"].ToString();

            switch (formType)
            {
                case "subscription":
                    // Parse the request body to get the subscription form data
                    SubscriptionForm subscriptionForm;
                    try
                    {
                        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                        subscriptionForm = JsonSerializer.Deserialize<SubscriptionForm>(requestBody);
                    }
                    catch
                    {
                        return new BadRequestResult();
                    }

                    // Process the subscription form data
                    await ProcessSubscriptionForm(subscriptionForm);
                    break;

                default:
                    return new BadRequestResult();
            }

            // Return a success response
            return new OkResult();
        }
        private static async Task ProcessSubscriptionForm(SubscriptionForm subscriptionForm)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("SubscriptionData");

            // Create a new table entity with the subscription form data
            SubscriptionEntity subscriptionEntity = new SubscriptionEntity()
            {
                Name = subscriptionForm.name,
                Email = subscriptionForm.email,
                AmountOwed = subscriptionForm.amountOwed,
                Lenders = subscriptionForm.lenders
            };

            // Create the table if it doesn't exist
            await table.CreateIfNotExistsAsync();

            // Insert or replace the table entity
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(subscriptionEntity);
            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        private static async Task SendVerificationEmail(SubscriptionEntity subscriber) 
        {
            string verificationLink = @"<a href='https://www.studentdebtcollective.com/verifysubscription?token={verification_token}'>Verify Your Subscription</a>";
        }

        [FunctionName("VerifySubscription")]
        public static async Task<IActionResult> VerifySubscription(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "verifysubscription")] HttpRequest req,
            [Table("SubscriptionEntities")] CloudTable subscriptionTable)
        {
            // Retrieve the verification token from the query parameters
            string verificationToken = req.Query["token"];

            // Validate the verification token
            if (string.IsNullOrEmpty(verificationToken))
            {
                return new BadRequestObjectResult("Invalid verification token");
            }

            // Retrieve the subscription entity from the table based on the verification token
            TableOperation retrieveOperation = TableOperation.Retrieve<SubscriptionEntity>("subscriptions", verificationToken);
            TableResult retrieveResult = await subscriptionTable.ExecuteAsync(retrieveOperation);
            SubscriptionEntity subscriptionEntity = (SubscriptionEntity)retrieveResult.Result;

            // Check if the subscription entity exists and if it is already verified
            if (subscriptionEntity == null || subscriptionEntity.IsVerified)
            {
                return new BadRequestObjectResult("Invalid verification token");
            }

            // Mark the subscription entity as verified
            subscriptionEntity.IsVerified = true;

            // Update the subscription entity in the table
            TableOperation updateOperation = TableOperation.Replace(subscriptionEntity);
            await subscriptionTable.ExecuteAsync(updateOperation);

            // Redirect the user to a success page or display a success message
            return new RedirectResult("https://www.studentdebtcollective.com/verification-success");
        }
    }
}
