using BuyEverything.Data;
using BuyEverything.DataAccess.Repository.IRepository;
using BuyEverything.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyEverything.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepo Category { get; private set; }
        public IProductRepo Product { get; private set; }
        public IShoppingCartRepo ShoppingCart { get; private set; }
        public IApplicationUserRepo ApplicationUser { get; private set; }
        public IOrderHeaderRepo OrderHeader { get; private set; }
        public IOrderDetailRepo OrderDetail { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category=new CategoryRepo(db);
            Product = new ProductRepo(db);
            ShoppingCart = new ShoppingCartRepo(db);
            ApplicationUser = new ApplicationUserRepo(db);
            OrderDetail = new OrderDetailRepo(db);
            OrderHeader=new OrderHeaderRepo(db);
        }
     

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
