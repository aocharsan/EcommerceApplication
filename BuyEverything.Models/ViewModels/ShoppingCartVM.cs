using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyEverything.Models.ViewModels
{
    public class ShoppingCartVM
    {
        
        [ValidateNever]
        public IEnumerable<ShoppingCart> ShoppinCartList { get; set; }  //Property same as declare in ProductController
        public OrderHeader OrderHeader { get; set; }
        
        










    }
}
