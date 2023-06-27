
using BuyEverything.DataAccess.Repository;
using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using BuyEverything.Models.ViewModels;
using BuyEverything.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BuyEverything.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
           OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id),
 

            };



            return View(orderVM);
        }


        #region API CALLS

        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            switch (status)
            {
                case "inprocess":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "approved":
                    objOrderHeader=objOrderHeader.Where(u=>u.OrderStatus==StaticDetails.StatusApproved);
                    break;
                case "pending":
                    objOrderHeader = objOrderHeader.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusPending);
                    break;
                case "completed":
                     objOrderHeader=objOrderHeader.Where(u=>u.OrderStatus==StaticDetails.StatusShipped);
                    break;
                default:
                    break;
            }
            return Json(new { data = objOrderHeader });
        }


      

        #endregion

    }
}
