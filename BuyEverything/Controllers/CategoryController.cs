using BuyEverything.Data;
using BuyEverything.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuyEverything.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext db)
        {
            _context = db;


        }
        public IActionResult Index()
        {    //It will go to DB,run the command, select * from categories and assign it to the obj  
            List<Category> objCategoryList = _context.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        { 
           return View();
        
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
           
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly same as Name");  //key is input asp-for

            }
            
            if (ModelState.IsValid)  // ie it check validations to Model class
            {
                _context.Categories.Add(obj);
                _context.SaveChanges();
                TempData["success"] = "category created successfully";
                return RedirectToAction("Index");
            }

            return View();

        }

       
        public IActionResult Edit(int? id)
        {
            if (id==null || id == 0)
            { 
            return NotFound();  
            }

            Category? categoryFromDB = _context.Categories.Find(id); //used when id pass is primary
            //Category? categoryFromDB1 = _context.Categories.FirstOrDefault(u=>u.Id==id); //Ideally use for any conditions
            //Category? categoryFromDB2 = _context.Categories.Where(u=>u.Id==id).FirstOrDefault();
            if (categoryFromDB == null)
            {
                return NotFound();
            }
             return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            if (ModelState.IsValid)  // ie it check validations to Model class
            {
                _context.Categories.Update(obj);
                _context.SaveChanges();
                TempData["success"] = "category updated successfully";
                return RedirectToAction("Index","Category");
            }

            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _context.Categories.Find(id); //used when id pass is primary
            //Category? categoryFromDB1 = _context.Categories.FirstOrDefault(u=>u.Id==id); //Ideally use for any conditions
            //Category? categoryFromDB2 = _context.Categories.Where(u=>u.Id==id).FirstOrDefault();
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj=_context.Categories.Find(id);
            if (obj == null)
            {
                return  NotFound();
            }
            _context.Categories.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "category deleted successfully";
            return RedirectToAction("Index");

        }



    }
}
