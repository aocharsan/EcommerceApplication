using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category");


            return View(productList);
        }
        public IActionResult Details(int? prodId)
        {
           Product prod = _unitOfWork.Product.GetFirstOrDefault(u=>u.Id==prodId , includeProperties: "Category");


            return View(prod);
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