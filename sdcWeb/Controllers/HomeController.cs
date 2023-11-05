using ChargeBee.Models;
using Microsoft.AspNetCore.Mvc;
using sdcWeb.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace sdcWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactFormModel());
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactFormModel model)
        {
            using (var client = new HttpClient()) {

                var fields = new Dictionary<string, string>
                {
                    { "entry.2005620554", model.Name },
                    { "entry.1045781291", model.Email},
                    { "entry.839337160", model.QuestionComment },
                };
                Uri uri = new Uri("https://docs.google.com/forms/u/0/d/e/1FAIpQLSdiBc4UBUm3aiCcpa8GN32sp2B9ddXr_5lKbg-vZnues-Vm0w/formResponse");
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("entry.2005620554", model.Name),
                    new KeyValuePair<string, string>("entry.1045781291", model.Email),
                    new KeyValuePair<string, string>("entry.839337160", model.QuestionComment),
                }); 
                var response = client.PostAsync(uri, formContent);
                string result = response.Result.ToString();
            }
            return RedirectToAction("ContactReceived", "Home", new { username = model.Name });
        }

        [HttpGet]
        public IActionResult ContactReceived(string username)
        {
            ContactReceivedModel model = new ContactReceivedModel() { Name = username };
            return View(model);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ShortSignup([FromForm]string EmailAddress)
        {
            return RedirectToAction("Subscription", "Home", new { emailAddress = EmailAddress });
        }

        [HttpGet]
        public IActionResult Subscription(string emailAddress)
        { 
            var subscriber = new SubscriptionModel { EmailAddress = emailAddress, Birthday = DateTime.Now };
            return View(subscriber);
        }

        [HttpPost]
        public IActionResult Subscription(SubscriptionModel model)
        {
            //bool success = false;
            //if (ModelState.IsValid) {
            //    //send to func
            //    using (var _client = new HttpClient())
            //    {
            //        HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/FunctionForController");
            //        HttpResponseMessage response = _client.SendAsync(newRequest).Result;

            //        dynamic responseResutls = response.Content;
            //    }
            //    //return RedirectToAction("ContactReceived", new { name = model.Name });



            //    //redirect to thanks on success
            //    success = true;
            //}

            return View(model);
        }

        [HttpGet]
        public IActionResult LoanCalculator() {
            return View();        
        }

        [HttpGet]
        public IActionResult SubscriptionSuccess(string id, string state)
        {
            //do something with id? 

            return View();
        }
    }
}