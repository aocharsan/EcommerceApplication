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
    public class ApplicationUserRepo : Repository<ApplicationUser> ,IApplicationUserRepo
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        
       
    }
}
