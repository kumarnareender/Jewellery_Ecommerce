using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web.App_Code;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private IUserService userService;
        private IRoleService roleService;
        private IOrderService orderService;
        private IProductService productService;
        private readonly IBranchService branchService;
        public CustomerController(IUserService userService, IOrderService orderService, IProductService productService, IRoleService roleService, IBranchService branchService)
        {
            this.userService = userService;
            this.productService = productService;
            this.orderService = orderService;
            this.roleService = roleService;
            this.branchService = branchService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderList()
        {
            return View();
        }

        public ActionResult OrderConfirm()
        {
            return View();
        }
        public ActionResult WholesaleOrderConfirm()
        {
            return View();
        }

        public ActionResult PaymentCancel()
        {
            return View();
        }

        public ActionResult OrderDetails()
        {
            return View();
        }

        public ActionResult EditAddress()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public string GetCustomerId(User userToSave)
        {
            string customerId = string.Empty;

            try
            {
                User user = userService.GetUserByPhone(userToSave.Username);
                if (user != null)
                {
                    customerId = user.Id;

                    user.FirstName = userToSave.FirstName;
                    user.LastName = userToSave.LastName;
                    user.ShipCountry = userToSave.ShipCountry;
                    user.ShipCity = userToSave.ShipCity;
                    user.ShipState = userToSave.ShipState;
                    user.ShipZipCode = userToSave.ShipZipCode;
                    user.ShipAddress = userToSave.ShipAddress;
                    user.Mobile = userToSave.Mobile;
                    user.Email = userToSave.Email;
                    userService.UpdateUserInfo(user);
                }
                else
                {
                    User newUser = new User();
                    customerId = Guid.NewGuid().ToString();

                    Role memberRole = roleService.GetRole(ERoleName.customer.ToString());

                    newUser.Id = customerId;
                    newUser.Username = userToSave.Username;
                    newUser.Password = userToSave.Username;
                    newUser.FirstName = userToSave.FirstName;
                    newUser.LastName = userToSave.LastName;
                    newUser.ShipCountry = userToSave.ShipCountry;
                    newUser.ShipCity = userToSave.ShipCity;
                    newUser.ShipState = userToSave.ShipState;
                    newUser.ShipZipCode = userToSave.ShipZipCode;
                    newUser.ShipAddress = userToSave.ShipAddress;
                    newUser.Mobile = userToSave.Mobile;
                    newUser.Email = userToSave.Email;
                    newUser.IsActive = true;
                    newUser.IsVerified = true;
                    newUser.IsDelete = false;
                    newUser.CreateDate = DateTime.Now;
                    newUser.Roles.Add(memberRole);

                    userService.CreateUser(newUser);
                }
            }
            catch
            {
                customerId = string.Empty;
            }

            return customerId;
        }

        [HttpPost]
        public JsonResult PlaceOrder(Application.Model.Models.Order order)
        {
            //return null;
            string branchName = Utils.GetSetting("CompanyName");
            Branch branch = branchService.GetBranchByName(branchName);
            order.BranchId = Application.Common.Utils.GetBranchId();
            string orderId = Guid.NewGuid().ToString();
            long orderCode = DateTime.Now.Ticks;

            bool isSuccess = false;
            try
            {


                // nullify the user object
                order.User = null;

                order.Id = orderId;
                order.OrderCode = orderCode;
                order.UserId = Utils.GetLoggedInUserId();
                order.StatusId = 1;
                order.PaymentStatusId = 3;
                order.DueAmount = order.PayAmount; // Always 0 in online version
                //order.Discount = 0; // Always 0 in online version because discount is already reduced from price
                order.ActionDate = DateTime.Now; //DateTime.UtcNow;
                order.ActionBy = Utils.GetLoggedInUserName();

                foreach (Model.Models.OrderItem item in order.OrderItems)
                {
                    item.OrderId = orderId;
                    // item.Id = Guid.NewGuid().ToString();
                    item.ActionDate = DateTime.Now;
                    item.CostPrice = productService.GetCostPrice(item.ProductId);
                }

                orderService.CreateOrder(order);
                isSuccess = true;

                // Update inventory stock
                foreach (Model.Models.OrderItem item in order.OrderItems)
                {
                    productService.MinusStockQty(item.ProductId, item.Quantity);
                }

                // Update sold count
                foreach (Model.Models.OrderItem item in order.OrderItems)
                {
                    productService.UpdateSoldCount(item.ProductId);
                }

                // Generate order barcode
                AppCommon.GenerateOrderBarcode(order.OrderCode.ToString());

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                isSuccess = false;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                orderId = orderId,
                orderCode = orderCode
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderList()
        {
            List<OrderViewModel> orderVMList = new List<OrderViewModel>();

            if (Utils.GetLoggedInUser() != null)
            {
                string userId = Utils.GetLoggedInUser().Id;
                IEnumerable<Model.Models.Order> orderList = orderService.GetOrders(userId);

                foreach (Model.Models.Order item in orderList)
                {
                    OrderViewModel order = new OrderViewModel
                    {
                        Id = item.Id,
                        OrderCode = item.OrderCode,
                        Discount = item.Discount,
                        DueAmount = item.DueAmount,
                        PayAmount = item.PayAmount,
                        OrderMode = item.OrderMode,
                        OrderStatus = item.OrderStatus,
                        PaymentStatus = item.PaymentStatus,
                        PaymentType = item.PaymentType,
                        Vat = item.Vat,
                        ActionDate = item.ActionDate,
                        ActionDateString = Utils.GetFormattedDate(item.ActionDate)
                    };

                    orderVMList.Add(order);
                }
            }

            return Json(orderVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderDetails(string orderId)
        {
            OrderViewModel orderVM = new OrderViewModel();

            Model.Models.Order order = orderService.GetOrder(orderId);
            if (order != null)
            {
                orderVM.Id = order.Id;
                orderVM.UserId = order.UserId;
                orderVM.BranchId = order.BranchId;
                orderVM.OrderCode = order.OrderCode;
                orderVM.OrderStatus = order.OrderStatus;
                orderVM.OrderMode = order.OrderMode;
                orderVM.PayAmount = order.PayAmount;
                orderVM.Discount = order.Discount;
                orderVM.Vat = order.Vat;
                orderVM.ShippingAmount = order.ShippingAmount;
                orderVM.DueAmount = order.DueAmount;
                orderVM.ReceiveAmount = order.ReceiveAmount;
                orderVM.ChangeAmount = order.ChangeAmount;
                orderVM.TotalWeight = order.TotalWeight;
                orderVM.DeliveryDate = order.DeliveryDate;
                orderVM.DeliveryTime = order.DeliveryTime;
                orderVM.IsFrozen = order.IsFrozen == null ? false : (bool)order.IsFrozen;
                orderVM.ActionDate = order.ActionDate;
                orderVM.OrderType = order.OrderType;
                orderVM.CustomerId = order.CustomerId;
                //orderVM.StatusId = (int)order.StatusId;
                //  orderVM.PaymentStatusId = order.PaymentStatusId.Value;
                orderVM.PaymentStatus = order.PaymentStatus;
                orderVM.OrderItems = new List<ViewModel.OrderItemViewModel>();
                foreach (Model.Models.OrderItem oi in order.OrderItems)
                {
                    //string title = oi.ProductId == Guid.Empty.ToString() ? oi.Title : oi.Product.Title;

                    OrderItemViewModel o = new ViewModel.OrderItemViewModel
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Title,
                        Price = oi.Price,
                        Discount = oi.Discount,
                        Quantity = oi.Quantity,
                        CostPrice = oi.CostPrice == null ? oi.Price : (decimal)oi.CostPrice,
                        ImageUrl = string.IsNullOrEmpty(oi.ImageUrl) ? "/Images/no-image.png" : oi.ImageUrl,
                        ActionDate = oi.ActionDate,
                        Color = oi.Color,
                        Size = oi.Size,
                        Weight = oi.Weight != null ? oi.Weight.Value : 0,
                        Unit = oi.Unit
                    };
                    orderVM.OrderItems.Add(o);
                }
            }

            return Json(orderVM, JsonRequestBehavior.AllowGet);
        }

        private string SetStripeSession(string orderId, string orderCode, double amount)
        {
            amount = amount * 100;
            // Order currency
            string currency = Utils.GetConfigValue("StripeCurrency");

            // Stripe secret key
            string secretKey = Utils.GetConfigValue("StripeSecretKey");

            StripeConfiguration.ApiKey = secretKey;

            string scheme = System.Web.HttpContext.Current.Request.Url.Scheme;
            string hostName = System.Web.HttpContext.Current.Request.Url.Host;
            hostName += (hostName == "localhost") ? ":" + System.Web.HttpContext.Current.Request.Url.Port : "";

            string baseUrl = scheme + "://" + hostName;

            string keyInfo = orderId + "_" + orderCode + "_" + amount.ToString();

            // Customer name
            string customerName = string.Empty;
            User user = Utils.GetLoggedInUser();
            if (user != null)
            {
                customerName = user.FirstName + " " + user.LastName;
            }

            SessionCreateOptions options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> {
                                new SessionLineItemOptions {
                                    Name = customerName,
                                    Description = "Online Payment",
                                    Amount = long.Parse(amount.ToString()),
                                    Currency = currency,
                                    Quantity = 1,
                                },
                            },
                SuccessUrl = baseUrl + "/Customer/PaymentSuccess_Stripe?keyInfo=" + keyInfo,
                CancelUrl = baseUrl + "/Customer/PaymentCancel_Stripe",
            };

            try
            {
                SessionService service = new SessionService();
                Session session = service.Create(options);
                return session.Id;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public JsonResult CardPayment(string orderId, string orderCode, double amount)
        {
            bool isSuccess = true;
            string sessionId = string.Empty;

            try
            {
                sessionId = SetStripeSession(orderId, orderCode, amount);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new
            {
                isSuccess = isSuccess,
                sessionId = sessionId
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PaymentSuccess_Stripe(string keyInfo)
        {
            string orderId = string.Empty;
            string orderCode = string.Empty;
            string amount = string.Empty;

            string[] data = keyInfo.Split('_');
            if (data.Length == 3)
            {
                orderId = data[0];
                orderCode = data[1];
                amount = data[2];
            }

            // Update payment status to 'Done'
            orderService.OrderPaymentDone(orderId);

            // Redirect to confirmation page
            return RedirectToAction("OrderConfirm", "Customer", new { orderCode = orderCode });
        }

        public ActionResult PaymentCancel_Stripe()
        {
            return RedirectToAction("PaymentCancel", "Customer");
        }
    }
}