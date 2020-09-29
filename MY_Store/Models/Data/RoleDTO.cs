using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MY_Store.Models.Data
{
    [Table("Roles")]
    public class RoleDTO
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}