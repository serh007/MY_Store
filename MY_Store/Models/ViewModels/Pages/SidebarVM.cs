﻿using MY_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MY_Store.Models.ViewModels.Pages
{
    public class SidebarVM
    {
        public SidebarVM()
        {
        }

        public SidebarVM(SidebarDTO row)
        {
            Id = row.Id;
            Body = row.Body;

        }
        public int Id { get; set; }
        public string Body { get; set; }
    }
}