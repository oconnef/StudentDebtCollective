using Microsoft.AspNetCore.Mvc;
using System.IO;
using sdcWeb.Models;
using System.Text;

namespace sdcWeb.Controllers
{
    public class BlogController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            List<Article> articles = new List<Article>();

            // Specify the path to your Articles folder
            string articlesFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Articles");

            // Get a list of text files in the Articles folder
            string[] articleFiles = Directory.GetFiles(articlesFolderPath, "*.txt");

            // Iterate through each text file and parse the article
            foreach (var filePath in articleFiles)
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);

                // Create a new Article instance
                Article article = new Article() { Id = filePath.Split("\\").Last().Split(".").First(), };
                article.MetaTags = new List<string>(); // Initialize the MetaTags property

                // Parse the article data from the lines
                foreach (var line in lines)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        switch (key)
                        {
                            case "Title":
                                article.Title = value;
                                break;
                            case "Author":
                                article.Author = value;
                                break;
                            case "PublicationDate":
                                if (DateTime.TryParse(value, out var publicationDate))
                                {
                                    article.PublicationDate = publicationDate;
                                }
                                break;
                            case "Content":
                                article.Content = value;
                                break;
                            case "MetaTags":
                                // Split and add tags to the MetaTags property
                                article.MetaTags.AddRange(value.Split(',').Select(tag => tag.Trim()));
                                break;
                            default:
                                // Handle other metadata or ignore if needed
                                break;
                        }
                    }
                }

                articles.Add(article);
            }

            return View(articles);
        }

        // View an individual article
        public ActionResult Article(string id)
        {
            // Construct the full path to the article file using the ID
            string articleFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Articles", $"{id}.txt");

            // Check if the article file exists
            if (!System.IO.File.Exists(articleFilePath))
            {
                // Handle the case where the article doesn't exist (e.g., return a 404 page)
                return NotFound();
            }

            string[] lines = System.IO.File.ReadAllLines(articleFilePath);

            // Create a new Article instance
            Article article = new Article() { Id = id };
            article.MetaTags = new List<string>(); // Initialize the MetaTags property

            // Track whether we're currently parsing the content section
            bool isParsingContent = false;
            StringBuilder contentBuilder = new StringBuilder();

            // Parse the article data from the lines
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "Title":
                            article.Title = value;
                            break;
                        case "Author":
                            article.Author = value;
                            break;
                        case "PublicationDate":
                            if (DateTime.TryParse(value, out var publicationDate))
                            {
                                article.PublicationDate = publicationDate;
                            }
                            break;
                        case "Content":
                            // Indicate that we're now parsing the content section
                            isParsingContent = true;
                            contentBuilder.AppendLine(value); // Add the first line of content
                            break;
                        case "MetaTags":
                            // Split and add tags to the MetaTags property
                            article.MetaTags.AddRange(value.Split(',').Select(tag => tag.Trim()));
                            break;
                        default:
                            // Handle other metadata or ignore if needed
                            break;
                    }
                }
                else if (isParsingContent)
                {
                    // Continue adding lines to the content section
                    contentBuilder.AppendLine(line);
                }
            }

            // Set the Content property with the concatenated content
            article.Content = contentBuilder.ToString();


            return View(article);
        }
    }
}
