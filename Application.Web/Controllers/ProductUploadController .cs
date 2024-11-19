using Application.Common;
using Application.Model.Models;
using Application.Service;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class ProductUploadController : Controller
    {
        private readonly IProductService productService;
        private readonly IBranchService branchService;
        public ProductUploadController(IProductService productService, IBranchService branchService)
        {
            this.productService = productService;
            this.branchService = branchService;
        }

        public ActionResult Index()
        {
            return View("UploadExcel");
        }


        [HttpGet]
        public void DownloadCSV()
        {
            string message = string.Empty;
            string path = System.Web.HttpContext.Current.Server.MapPath("~/SampleFile/");
            string fileName = "samplefile.csv";

            string filePath = Path.Combine(path, fileName);

            string csvContentStr = System.IO.File.ReadAllText(filePath);

            //This saves content as CSV File.  
            SaveCSVFile("CSVFileName", csvContentStr);


        }

        public void SaveCSVFile(string fileName, string csvContentStr)
        {
            try
            {
                fileName = fileName + "_" + string.Format("{0:MMMM}", DateTime.Today) + "_" + string.Format("{0:yyyy}", DateTime.Today);

                Response.Clear();
                // This is content type for CSV.  
                Response.ContentType = "Text/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + ".csv\"");

                Response.Write(csvContentStr); //Here Content write in page.  
            }
            finally
            {
                Response.End();
            }
        }

        [HttpPost]
        public JsonResult UploadProducts()
        {
            bool isSuccess = false;
            string message = string.Empty;


            List<CSVData> data = ReadCSV(Request);

            if (data != null)
            {
                CreateProducts(data);
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }


        private List<CSVData> ReadCSV(HttpRequestBase request)
        {
            try
            {

                List<CSVData> products = new List<CSVData>();
                foreach (string name in request.Files)
                {
                    HttpPostedFileBase file = request.Files[name];

                    string originalFileName = file.FileName;
                    string fileExtension = Path.GetExtension(originalFileName);
                    string fileName = "CsvProducts.csv";

                    string directoryPath = System.Web.HttpContext.Current.Server.MapPath("~/ExcelFiles/");

                    string filePath = Path.Combine(directoryPath, fileName);
                    DirectoryInfo di = new DirectoryInfo(directoryPath);

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    foreach (FileInfo f in di.GetFiles())
                    {
                        f.Delete();
                    }

                    // Save csv file
                    file.SaveAs(filePath);

                    CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Encoding = Encoding.UTF8,
                        HasHeaderRecord = false,
                        MissingFieldFound = null,
                        IgnoreBlankLines = true,
                        Mode = /*Parameter.FieldsEnclosedInQuotes ? CsvHelper.CsvMode.RFC4180 :*/ CsvHelper.CsvMode.RFC4180
                    };
                    Type type = typeof(string);
                    using (StreamReader reader = new StreamReader(filePath))
                    using (CsvReader csvReader = new CsvReader(reader, config))
                    {
                        csvReader.Read();


                        while (csvReader.Read())
                        {
                            IParser parser = csvReader.Parser;

                            bool qtyIsNumeric = int.TryParse(parser[4], out int qty);
                            bool wightNumeric = decimal.TryParse(parser[2], out decimal weight);
                            bool salePriceIsDecimal = decimal.TryParse(parser[5], out decimal salePrice);
                            bool purchasePriceIsDecimal = decimal.TryParse(parser[6], out decimal purchasePrice);
                            bool categoryIdIsNumeric = int.TryParse(parser[7], out int categoryId);

                            CSVData product = new CSVData()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Barcode = parser[0],
                                Description = parser[1],
                                Unit = parser[3],
                                StockQty = qtyIsNumeric ? qty : 0,
                                SalePrice = salePriceIsDecimal ? salePrice : 0,
                                PurchasePrice = purchasePriceIsDecimal ? purchasePrice : 0,
                                //Category = parser[7],
                                weight = wightNumeric ? weight : 0,
                                CategoryId = categoryIdIsNumeric ? categoryId : 0
                            };
                            products.Add(product);
                        }

                    }
                }
                return products;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool CreateProducts(List<CSVData> products)
        {
            bool isSuccess = false;
            try
            {
                string branchName = Utils.GetSetting("CompanyName");

                Branch branch = branchService.GetBranchByName(branchName);

                IEnumerable<Product> allProducts = productService.GetAllProducts(4);
                foreach (CSVData item in products)
                {
                    if (allProducts.Where(x => x.Barcode == item.Barcode).Any())
                    {
                        continue;
                    }

                    Product product = new Product()
                    {
                        Id = item.Id,
                        Barcode = item.Barcode,
                        Description = item.Description,
                        Title = item.Description,
                        CostPrice = item.PurchasePrice,
                        OnlinePrice = item.SalePrice,
                        RetailPrice = item.SalePrice,
                        WholesalePrice = item.SalePrice,
                        Unit = item.Unit,
                        CategoryId = item.CategoryId,
                        Quantity = item.StockQty,
                        Weight = item.weight,

                        IsApproved = true,
                        IsDeleted = false,
                        Code = 1234,
                        BranchId = branch != null ? branch.Id : 4,
                        //BranchId = 4,
                        IsSync = false,
                        ActionDate = DateTime.Now,
                        LowStockAlert = 5,
                        UserId = Utils.GetLoggedInUserId(),
                        Status = EAdStatus.Running.ToString(),
                        ShortCode = "Temp",
                        Gst = 0,
                    };

                    productService.CreateProduct(product);
                    isSuccess = true;
                }

                return isSuccess;
            }
            catch (Exception name)
            {
                return false;
            }
        }

        public class CSVData
        {
            public string Id { get; set; }
            public string Barcode { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public int StockQty { get; set; }
            public decimal SalePrice { get; set; }
            public decimal PurchasePrice { get; set; }
            public decimal weight { get; set; }
            public int CategoryId { get; set; }

        }

    }
}