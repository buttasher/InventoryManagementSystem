using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;

namespace InventoryManagementSystem.Controllers
{
    public class AiReportingController : Controller
    {
        private readonly AiReportingService _aiReportingService;
        private readonly HttpClient _httpClient;

        // Constructor to inject the dependencies
        public AiReportingController(AiReportingService aiReportingService, HttpClient httpClient)
        {
            _aiReportingService = aiReportingService;
            _httpClient = httpClient;
        }

        // Index action (you can display the report generation page here)
        public IActionResult Index()
        {
            return View();
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

                // ✅ Ensure the directory exists before saving the file
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

                // ✅ Ensure the directory exists before saving the file
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

        [HttpPost]
        public async Task<IActionResult> GenerateProfitLossReport()
        {
            // Step 1: Fetch profit/loss data from the service
            var profitLossDetails = await _aiReportingService.GetProfitLossReport();

            // Step 2: Convert the data into JSON format
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(profitLossDetails),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Step 3: Send the data to the Python Flask API for processing
            var response = await _httpClient.PostAsync("http://localhost:5000/generate-profit-loss-report", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                // Ensure the directory exists before saving the file
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, "profit_loss_report.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                ViewData["ProfitLossReportGenerated"] = true;
                ViewData["ProfitLossReportDownloadUrl"] = "/pdfs/profit_loss_report.pdf";

                return RedirectToAction("ProductInsights");
            }
            else
            {
                return BadRequest("Error generating the profit/loss report.");
            }
        }


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
