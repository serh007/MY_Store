﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MY_Store.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }
        public DbSet<SidebarDTO> Sidebars { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
    }
}