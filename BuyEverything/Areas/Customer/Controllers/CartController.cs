using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using BuyEverything.Models.ViewModels;
using BuyEverything.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;

namespace BuyEverything.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;



        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppinCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                          includeProperties: "Product"),
                OrderHeader = new OrderHeader()


            };
            foreach (var cart in shoppingCartVM.ShoppinCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);


            }

            return View(shoppingCartVM);
        }

        public IActionResult Summery()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppinCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                          includeProperties: "Product"),
                OrderHeader = new OrderHeader()


            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in shoppingCartVM.ShoppinCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);


            }
            return View(shoppingCartVM);

        }
        [HttpPost]
        [ActionName("Summery")]
        public IActionResult SummeryPOST()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppinCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                      includeProperties: "Product");
            //  OrderHeader = new OrderHeader()  as we are binding it to ShoppingCartVM


            shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);


            foreach (var cart in shoppingCartVM.ShoppinCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);


            }

            shoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            shoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;

            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            //As we save it first bcz we need a header Id into OrderDetails Table
            _unitOfWork.Save();
            //also we need to save order Details to DB
            foreach (var cart in shoppingCartVM.ShoppinCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count

                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();

            }
            //add payment stripe session
            if (applicationUser.Id != null)
            {
                var domain = "http://localhost:5238/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?OrderId={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach (var item in shoppingCartVM.ShoppinCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {

                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title

                            }


                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdatePaymentId(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                //redirect to stripe 
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
     
            return RedirectToAction(nameof(OrderConfirmation), new { OrderId = shoppingCartVM.OrderHeader.Id });

        }

        public IActionResult OrderConfirmation(int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderId);
            //retrieving stripe session where we get PaymentIntentId 
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                //as we know session contains paymentIntentId
                _unitOfWork.OrderHeader.UpdatePaymentId(OrderId, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(OrderId, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            // After successful payment lets empty cart
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                  .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

           HttpContext.Session.Clear();

            return View(OrderId);
        }


        public IActionResult plus(int cartId)
        {
            var shoppingCartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            shoppingCartFromDB.Count += 1;
            _unitOfWork.ShoppingCart.Update(shoppingCartFromDB);
            _unitOfWork.Save();
            return RedirectToAction("Index");


        }


        public IActionResult Minus(int cartId)
        {
            var shoppingCartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, tracked: true);
            if (shoppingCartFromDB.Count <= 1)
            {
                //remove it from cart
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == shoppingCartFromDB.ApplicationUserId).Count()-1);
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDB);
            }
            else
            {
                shoppingCartFromDB.Count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDB);
            }


            _unitOfWork.Save();
            return RedirectToAction("Index");


        }

        public IActionResult Remove(int cartId)
        {
            var shoppingCartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId,tracked:true);
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart
                 .GetAll(u => u.ApplicationUserId == shoppingCartFromDB.ApplicationUserId).Count()-1);
            _unitOfWork.ShoppingCart.Remove(shoppingCartFromDB);
            _unitOfWork.Save();

            return RedirectToAction("Index");

        }



        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;

            }
            else
            {
                if (shoppingCart.Count <= 100)
                    return shoppingCart.Product.Price50;
                return shoppingCart.Product.Price100;
            }





        }

    }
}
