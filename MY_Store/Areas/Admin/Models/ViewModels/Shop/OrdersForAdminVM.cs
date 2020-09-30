using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MY_Store.Areas.Admin.Models.ViewModels.Shop
{
    public class OrdersForAdminVM
    {
        [DisplayName("Order Number")]
        public int OrderNumber { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAndQty { get; set; }
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }
    }
}