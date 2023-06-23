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
    public class OrderHeaderRepo : Repository<OrderHeader> ,IOrderHeaderRepo
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdatePaymentId(int id, string sessionId, string paymentIntentId)
        {
           var orderFromDB = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            { 
             orderFromDB.SessionId= sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDB.PaymentIntentId = paymentIntentId;
                orderFromDB.PaymentDate=DateTime.Now;
            }

        }    

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDB = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDB != null)
            {
                orderFromDB.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                { 
                  orderFromDB.PaymentStatus= paymentStatus;
                }


            }

        }
    }
}
