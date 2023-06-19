using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyEverything.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepo Category { get; }
        IProductRepo Product { get; }
        void Save();


    }
}
