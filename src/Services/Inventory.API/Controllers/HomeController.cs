﻿using Microsoft.AspNetCore.Mvc;

namespace Inventory.Product.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index ( )
        {
            return Redirect("~/swagger");
        }
    }
}
