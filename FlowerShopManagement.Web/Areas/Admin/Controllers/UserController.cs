﻿using Microsoft.AspNetCore.Mvc;

namespace FlowerShopManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.User = true;
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
