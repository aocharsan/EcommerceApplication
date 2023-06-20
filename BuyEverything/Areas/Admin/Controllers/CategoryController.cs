using BuyEverything.Data;
using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using BuyEverything.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuyEverything.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db;


        }
        public IActionResult Index()
        {    //It will go to DB,run the command, select * from categories and assign it to the obj  
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly same as Name");  //key is input asp-for

            }

            if (ModelState.IsValid)  // ie it check valIdations to Model class
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "category created successfully";
                return RedirectToAction("Index");
            }

            return View();

        }


        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            //  Category? categoryFromDB = _context.Categories.Find(Id); //used when Id pass is primary
            Category? categoryFromDB = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryId == Id);

            //Category? categoryFromDB1 = _context.Categories.FirstOrDefault(u=>u.Id==Id); //Ideally use for any conditions
            //Category? categoryFromDB2 = _context.Categories.Where(u=>u.Id==Id).FirstOrDefault();
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            if (ModelState.IsValid)  // ie it check valIdations to Model class
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "category updated successfully";
                return RedirectToAction("Index", "Category");
            }

            return View();

        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryId == Id); //used when Id pass is primary
            //Category? categoryFromDB1 = _context.Categories.FirstOrDefault(u=>u.Id==Id); //Ideally use for any conditions
            //Category? categoryFromDB2 = _context.Categories.Where(u=>u.Id==Id).FirstOrDefault();
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? Id)
        {
            Category? obj = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryId == Id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "category deleted successfully";
            return RedirectToAction("Index");

        }



    }
}
