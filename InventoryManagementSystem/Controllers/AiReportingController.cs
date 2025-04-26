using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using System.IO;

namespace InventoryManagementSystem.Controllers
{
    public class AiReportingController : Controller
    {
        private readonly AiReportingService _aiReportingService;
        private readonly HttpClient _httpClient;
        private readonly InventoryManagementSystemContext _context;
      
        // Constructor to inject the dependencies
        public AiReportingController(AiReportingService aiReportingService, HttpClient httpClient, InventoryManagementSystemContext context)
        {
            _aiReportingService = aiReportingService;
            _httpClient = httpClient;
            _context = context;
            
        }

        // Action to handle report download
        public async Task<IActionResult> DownloadReport()
        {
            // Save the notification first
            var adminId = 1; // Replace with the actual admin userId or fetch from context
            var notification = new Notification
            {
                Message = "The report has been downloaded successfully.",
                Role = "admin",  // Admin role
                IsRead = false,  // Notification is unread initially
                CreatedAt = DateTime.Now,
                UserId = adminId
            };

            try
            {
                _context.Notifications.Add(notification); // Add to the Notifications table
                await _context.SaveChangesAsync(); // Save to the database

                // Check if the notification is saved correctly
                var savedNotification = await _context.Notifications.OrderByDescending(n => n.CreatedAt).FirstOrDefaultAsync();
                if (savedNotification != null)
                {
                    Console.WriteLine("Notification saved with ID: " + savedNotification.Id);
                }
                else
                {
                    Console.WriteLine("Failed to save notification.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving notification: " + ex.Message);
            }

          

            // Proceed with the download logic (or any additional logic before download)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "report.pdf");

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/pdf", "report.pdf");
            }
            else
            {
                return NotFound("Report file not found.");
            }
        }

      


        // Action to generate the report and send data to the Python script
        [HttpPost]
        public async Task<IActionResult> GenerateReport()
        {
            // Step 1: Fetch data from the database using the service
            var transactionDetails = await _aiReportingService.GetTransactiondetails();


            // Step 2: Convert the data into JSON format
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(transactionDetails),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Step 3: Send the data to the Python Flask API for processing
            var response = await _httpClient.PostAsync("http://localhost:5000/generate-report", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                // Ensure the directory exists before saving the file
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, "report.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                

           


                ViewData["ReportGenerated"] = true;
                ViewData["ReportDownloadUrl"] = "/pdfs/report.pdf";

                return RedirectToAction("SalesTrend");
            }
            else
            {
                // If there's an error, return a bad request
                return BadRequest("Error generating the report.");
            }
        }

 

        // Action to generate the customer segmentation report
        public async Task<IActionResult> GenerateCustomerSegmentationReport()
        {
            // Step 1: Fetch customer segmentation data from the service
            var customerSegmentationDetails = await _aiReportingService.GetCustomerSegmentation();

          

            // Step 2: Convert the data into JSON format
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(customerSegmentationDetails),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Step 3: Send the data to the Python Flask API for processing
            var response = await _httpClient.PostAsync("http://localhost:5000/generate-customer-segmentation-report", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                // Ensure the directory exists before saving the file
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, "customer_segmentation_report.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

             
                ViewData["CustomerSegmentationReportGenerated"] = true;
                ViewData["CustomerSegmentationReportDownloadUrl"] = "/pdfs/customer_segmentation_report.pdf";

                return RedirectToAction("CustomerSegmentation");
            }
            else
            {
                // If there's an error, return a bad request
                return BadRequest("Error generating the customer segmentation report.");
            }
        }

        // Action to generate the profit/loss report
        [HttpPost]
        public async Task<IActionResult> GenerateProfitLossReport()
        {
            var profitLossDetails = await _aiReportingService.GetProfitLossReport();

          
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(profitLossDetails),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("http://localhost:5000/generate-profit-loss-report", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, "profit_loss_report.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);


                // ✅ Optional: TempData for success message
                TempData["ProfitLossReportGenerated"] = true;
                TempData["ProfitLossReportDownloadUrl"] = "/pdfs/profit_loss_report.pdf";

                return RedirectToAction("ProductInsights");
            }
            else
            {
                return BadRequest("Error generating the profit/loss report.");
            }
        }

        // Views for each report
        public IActionResult SalesTrend()
        {
          

            return View();
        }

        public IActionResult CustomerSegmentation()
        {
            

            return View();
        }

        public IActionResult ProductInsights()
        {
            

            return View();
        }
    }
}
