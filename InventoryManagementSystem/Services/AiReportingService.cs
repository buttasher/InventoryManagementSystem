using System.Linq;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface IAiReportingService
    {
        Task<List<TransactionReportDto>> GetTransactiondetails();
    }

    public class AiReportingService : IAiReportingService
    {
        private readonly InventoryManagementSystemContext _context;

        // Constructor to inject the DbContext
        public AiReportingService(InventoryManagementSystemContext context)
        {
            _context = context;
        }

        // Method to fetch the required data from the database
        public async Task<List<TransactionReportDto>> GetTransactiondetails()
        {
            var result = await (from t in _context.Transactions
                                join td in _context.Transactiondetails on t.TransactionId equals td.TransactionId
                                join p in _context.Products on td.ProductId equals p.ProductId
                                select new TransactionReportDto
                                {
                                    TransactionId = t.TransactionId,
                                    CustomerId = t.CustomerId,
                                    ProductId = td.ProductId ?? 0,
                                    ProductName = p.ItemName,
                                    Quantity = td.Quantity
                                }).ToListAsync();

            return result;
        }
        public async Task<List<CustomerSegmentationDto>> GetCustomerSegmentation()
        {
            // Step 1: Fetch data from the database
            var result = await (from t in _context.Transactions
                                join td in _context.Transactiondetails on t.TransactionId equals td.TransactionId
                                join p in _context.Products on td.ProductId equals p.ProductId
                                join c in _context.Customers on t.CustomerId equals c.CustomerId
                                join pc in _context.Productcategories on p.CategoryId equals pc.CategoryId
                                select new
                                {
                                    c.CustomerId,
                                    c.Age,
                                    pc.CategoryName,
                                    p.ItemName,
                                    t.TotalAmount
                                }).ToListAsync();

            // Step 2: Process the data (parse Age) and return a list of DTOs
            var customerSegmentationDtoList = result.Select(item => new CustomerSegmentationDto
            {
                CustomerId = item.CustomerId,
                Age = int.TryParse(item.Age, out var ageVal) ? ageVal : 0, // Parsing Age here
                ProductCategory = item.CategoryName,
                ProductName = item.ItemName,
                TransactionAmount = item.TotalAmount ?? 0m // Handle nullable TotalAmount
            }).ToList();

            return customerSegmentationDtoList;
        }
        public async Task<List<ProfitLossReportDto>> GetProfitLossReport()
        {
            var result = await (from t in _context.Transactions
                                join td in _context.Transactiondetails on t.TransactionId equals td.TransactionId
                                join p in _context.Products on td.ProductId equals p.ProductId
                                select new
                                {
                                    TransactionId = t.TransactionId,
                                    CustomerId = t.CustomerId,
                                    ProductId = p.ProductId,
                                    ProductName = p.ItemName,
                                    Quantity = td.Quantity,
                                    CostPriceStr = p.CostPrice,
                                    SellingPriceStr = p.SellingPrice,
                                    TransactionDate = t.TransactionDate
                                }).ToListAsync();

            var report = result.Select(item =>
            {
                decimal.TryParse(item.CostPriceStr, out decimal costPrice);
                decimal.TryParse(item.SellingPriceStr, out decimal sellingPrice);
                decimal totalRevenue = sellingPrice * item.Quantity;
                decimal profitLoss = (sellingPrice - costPrice) * item.Quantity;

                return new ProfitLossReportDto
                {
                    TransactionId = item.TransactionId,
                    CustomerId = item.CustomerId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    CostPrice = costPrice,
                    SellingPrice = sellingPrice,
                    TotalRevenue = totalRevenue,
                    ProfitLoss = profitLoss,
                     TransactionDate = (DateTime)item.TransactionDate
                };
            }).ToList();

            return report;
        }




    }

    // DTO for the response to send to the Python script
    public class TransactionReportDto
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class CustomerSegmentationDto
    {
        public int CustomerId { get; set; }
        public int Age { get; set; }
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public decimal TransactionAmount { get; set; }
    }

    public class ProfitLossReportDto
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; } // match this name
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ProfitLoss { get; set; } // match this name
    }
}
