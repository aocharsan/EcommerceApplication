using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using BuyEverything.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Claims;

namespace BuyEverything.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork   _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {
            //get userid og loggedin User
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                //get session for recover existing product count in the cart
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
            }
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(productList);
        }
        public IActionResult Details(int prodId)
        {
            ShoppingCart cart = new()
            {
                Product= _unitOfWork.Product.GetFirstOrDefault(u => u.Id == prodId, includeProperties: "Category"),
                Count=1,
                ProductId=prodId
           };
          


            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            

            var claimsIdentity=(ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;
            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ApplicationUserId == userId
                                      && u.ProductId == cart.ProductId);


           

            if (cartFromDB != null)
                {
                    //i.e shopping cart exist


                    cartFromDB.Count += cart.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDB);
                    _unitOfWork.Save();



                }
                else
                {
                    //add cart to DB
                    _unitOfWork.ShoppingCart.Add(cart);
                    _unitOfWork.Save();
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                             _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());


                }
         
         
           
                return RedirectToAction(nameof(Index));
            

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}