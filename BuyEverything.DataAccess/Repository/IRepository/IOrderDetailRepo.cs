using BuyEverything.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyEverything.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepo :IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
        



    }
}
