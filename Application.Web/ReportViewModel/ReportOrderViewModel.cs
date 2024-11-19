using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Web.ReportViewModel
{
    public class ReportOrderViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyPhone { get; set; }
        public string FooterLine1 { get; set; }
        public string FooterLine2 { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ImageName { get; set; }
        public string ProductName { get; set; }
        public string ProductBarcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
        public string PaymentBy { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string OrderId { get; set; }
        public string BarcodeImageName { get; set; }
        public string OrderStatus { get; set; }
        public string OrderDate { get; set; }
        public string TotalWeight { get; set; }
        public string DeliveryDateTime { get; set; }
        public string FrozenItem { get; set; }
        public string OrderMode { get; set; }
        public string RegistrationNo { get; set; }
    }    
}