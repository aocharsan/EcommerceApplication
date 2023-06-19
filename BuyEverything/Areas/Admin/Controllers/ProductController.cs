using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using BuyEverything.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyEverything.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;


        }



        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)    //Update+insert
         {
           //   IEnumerable<SelectListItem> CategoryList = 
          //  ViewBag.AllCategories = CategoryList;
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
            CategoryList = _unitOfWork.Category.
           GetAll().ToList().Select(u => new SelectListItem
           {
               Text = u.Name,
               Value = u.CategoryId.ToString()

           })


        };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else 
            { 
                //update
              productVM.Product=_unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

                return View(productVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM ,IFormFile file)
        {



            if (ModelState.IsValid)  // ie it check valIdations to Model class
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                { 
                    string filename=Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath=Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.Imageurl))
                    {

                        //first delete old image url
                     var oldImagePath=Path.Combine(wwwRootPath,productVM.Product.Imageurl.TrimStart('\\'));
                        //check any file exist in old path or not
                        if (System.IO.File.Exists(oldImagePath))
                        { 
                          System.IO.File.Delete(oldImagePath);
                        
                        }
                    
                    
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                    }
                    productVM.Product.Imageurl = @"\images\product\" + filename;
                
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
               
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.
           GetAll().ToList().Select(u => new SelectListItem
           {
               Text = u.Name,
               Value = u.CategoryId.ToString()
              
            });
                return View(productVM);

            }
           
        }
     
       
        #region API CALLS
        [HttpPost]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.GetFirstOrDefault(u=>u.Id==id);
            if (productToBeDeleted == null)
            {
                return Json(new { sucess=false ,message="Error while deleting"});
            
            }
            //also deleted image stored in wwwRoot/product folder
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                               productToBeDeleted.Imageurl.TrimStart('\\'));
            //check any file exist in old path or not
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);

            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();


            return Json(new {success=true,message="Deleted successfully" });
        }

        #endregion




    }
}
