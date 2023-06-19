using BuyEverything.Data;
using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuyEverything.DataAccess.Repository
{
    public class ProductRepo : Repository<Product> , IProductRepo
    {
        private readonly ApplicationDbContext _db;

        public ProductRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        public void Update(Product obj)
        {
          var objFromDB=_db.Products.FirstOrDefault(u=>u.Id==obj.Id);
            if(objFromDB!=null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.Description = obj.Description;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Price = obj.Price;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price50= obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.Author= obj.Author;
                if (obj.Imageurl != null)
                { 
                 objFromDB.Imageurl = obj.Imageurl;
                }


            }
        }

       
    }
}
