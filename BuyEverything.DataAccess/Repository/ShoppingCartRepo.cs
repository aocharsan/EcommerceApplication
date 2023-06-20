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
    public class ShoppingCartRepo : Repository<ShoppingCart> ,IShoppingCartRepo
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }

       
    }
}
