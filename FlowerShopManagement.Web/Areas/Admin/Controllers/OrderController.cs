﻿using FlowerShopManagement.Application.Interfaces;
using FlowerShopManagement.Application.Interfaces.UserSerivices;
using FlowerShopManagement.Application.Models;
using FlowerShopManagement.Application.MongoDB.Interfaces;
using FlowerShopManagement.Core.Entities;
using FlowerShopManagement.Core.Enums;
using FlowerShopManagement.Web.ViewModels;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlowerShopManagement.Web.Areas.Admin.Controllers
{
    //--------------------------------------Admin Order Controller--------------------------------------------------
    [Area("Admin")]
    public class OrderController : Controller
    {
        //Services
        ISaleService _saleServices;
        IStockService _stockServices;
        IUserService _userServices;
        IStaffService _staffService;
        //Repositories
        IOrderRepository _orderRepository;
        IProductRepository _productRepository;
        IUserRepository _userRepository;

        public OrderController(ISaleService saleServices, IOrderRepository orderRepository, IProductRepository productRepository,
            IUserRepository userRepository, IStockService stockServices, IUserService userServices, IStaffService staffService)
        {
            _orderRepository = orderRepository;
            _saleServices = saleServices;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _stockServices = stockServices;
            _userServices = userServices;
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Order = true;
            //Set up default values for OrderPage

            ViewData["Categories"] = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();
            List<OrderModel> orderMs = await _saleServices.GetUpdatedOrders(_orderRepository);
            return View(new List<OrderModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            //Should get a new one because an admin updates data realtime
            var order = await _saleServices.GetADetailOrder(id, _orderRepository);
            if (order != null)
            {
                return View(/*Coult be a ViewModel in here*/);
            }
            return RedirectToAction("Index");
        }

        [HttpPut]
        public async Task<IActionResult> Update(OrderModel orderModel)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            //If order get null or Id null ( somehow ) => notfound
            if (orderModel == null || orderModel.Id == null) return NotFound();
            //Check if the order still exists
            var order = await _orderRepository.GetById(orderModel.Id); // check exist might be implemented in sale services somehow....
            if (order != null)
            {
                //If order != null => we will update this order by using directly orderModel.Id
                //Check orderModel for sure if losing some data
                var result = await _orderRepository.UpdateById(orderModel.Id, orderModel.ToEntity());
                if (result != false)
                {
                    //Update successfully, we pull new list of orders
                    List<OrderModel> orderMs = await _saleServices.GetUpdatedOrders(_orderRepository);

                    return PartialView(/*Coult be a UPDATED ViewModel in here*/);//for example: a _ViewAll 

                }
            }
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Detele(OrderModel orderModel)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            //If order get null or Id null ( somehow ) => notfound
            if (orderModel == null || orderModel.Id == null) return NotFound();
            //Check if the order still exists
            var order = await _orderRepository.GetById(orderModel.Id);
            if (order != null)
            {
                //If order != null => we will detele this order by using directly orderModel.Id
                //Check orderModel for sure if losing some data
                var result = await _orderRepository.RemoveById(orderModel.Id);
                if (result == false)
                    return RedirectToAction($"Unable to remove {result}");
                else
                {
                    //Detele successfully, we pull a new list of orders
                    List<OrderModel> orderMs = await _saleServices.GetUpdatedOrders(_orderRepository);

                    return PartialView(/*Coult be a UPDATED ViewModel in here*/); //For example: a _ViewAll
                }
            }
            return NotFound();
        }

        //Open an CreatePage
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Set up default values for OrderPage

            ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            List<ProductModel> productMs = await _stockServices.GetUpdatedProducts(_productRepository);
            List<UserDetailsModel>? customerMs = await _staffService.GetUsersAsync();
            /*
                Set up viewmodel
			    Need a current picked items
                Need a current picked customer
                PageList<>
			*/
            if (productMs == null || customerMs == null) return NotFound();
            OrderVM orderVM = new OrderVM();
            orderVM.AllProductModels = productMs;
            orderVM.customerMs = customerMs;
            TempData["orderVM"] = JsonConvert.SerializeObject(orderVM, Formatting.Indented);
            return View(orderVM);
        }

        // Confirm and create an Order
        [HttpPost]
        public async Task<IActionResult> Create(OrderModel orderModel, UserModel userModel)
        {
            var result = _saleServices.CreateOfflineOrder(orderModel, userModel, _orderRepository, _userRepository, _productRepository);
            if (result != null)
            {
                List<OrderModel> orders = await _saleServices.GetUpdatedOrders(_orderRepository);
                return PartialView(/*Coult be a ViewModel in here*/); // A updated _ViewAll table
            }
            return NotFound(); // Can be changed to Redirect
        }

        [HttpPost]
        public async Task<IActionResult> PickItemDialog(string filter = "") // Also handle if there is a filter / search
        {
            OrderVM? orderVM = null;

            if (TempData["orderVM"] != null)
            {
                string s = TempData["orderVM"] as string ?? "";
                if (s != "")
                {
                    orderVM = JsonConvert.DeserializeObject<OrderVM>(s);
                }
            }


            List<ProductModel> productMs = await _stockServices.GetUpdatedProducts(_productRepository);
            if (orderVM != null)
            {
                orderVM.AllProductModels = productMs;
                TempData["orderVM1"] = JsonConvert.SerializeObject(orderVM, Formatting.Indented);
            }
            int pageSizes = 99;
            return PartialView("_PickItem", PaginatedList<ProductModel>.CreateAsync(productMs, 1, pageSizes)); // This will be an view for dialog / modal
        }

        [HttpPost]
        public IActionResult PickedAnItem(List<string> ids, List<int> amounts) // These 2 list should have the same size // need 1 more parameters like PickItemDialog viewmodel
        {
            string s = TempData["orderVM1"] as string ?? "";
            OrderVM? orderVM = null;
            if (s != "")
            {
                orderVM = JsonConvert.DeserializeObject<OrderVM>(s);
            }

            if (orderVM == null || orderVM.Order == null) return NotFound();

            OrderModel orderModel = orderVM.Order;
            if (ids != null && amounts != null && orderVM.ProductModels != null && orderVM.AllProductModels != null)
            {
                for (int i = 0; i < amounts.Count && i < ids.Count; i++)
                {
                    if (amounts[i] != 0 && ids[i] != "")
                    {

                        ProductModel? product = orderVM.AllProductModels.FirstOrDefault(o => o.Id != null && o.Id.Equals(ids[i]));
                        if (product != null)
                        {
                            orderVM.ProductModels.Add(product);

                        }
                    }
                }

            }
            //CurrentOrder will be ready for the next request
            TempData["orderVM"] = JsonConvert.SerializeObject(orderVM, Newtonsoft.Json.Formatting.Indented);

            return PartialView("_PickedItemsTable", orderVM.ProductModels);
        }

        [HttpPost]
        public IActionResult DeteleItem(string id)
        {
            OrderModel? orderModel = null;
            if (TempData["CurrentOrder"] != null)
                orderModel = TempData["CurrentOrder"] as OrderModel;
            if (orderModel != null)
            {
                foreach (var i in orderModel.Products)
                    if (i.IsEqualProduct(id))
                        orderModel.Products.Remove(i);
            }
            //CurrentOrder will be ready for the next request
            TempData["CurrentOrder"] = orderModel;

            //Should update Viewmodel for newest updates
            return PartialView(/*Coult be a ViewModel in here*/);// This will be an update view for current order table
        }
        [HttpPost]
        public async Task<IActionResult> PickCustomerDialog(string filter = "") // Also handle if there is a filter / search
        {
            OrderVM? orderVM = null;
            

            if (TempData["orderVM"] != null)
            {
                string s = TempData["orderVM"] as string ?? "";
                if (s != "")
                {
                    orderVM = JsonConvert.DeserializeObject<OrderVM>(s);
                }

                List<UserDetailsModel>? userMS = await _staffService.GetUsersAsync();
                if (orderVM != null)
                {
                    orderVM.customerMs = userMS;
                    TempData["orderVM1"] = JsonConvert.SerializeObject(orderVM, Formatting.Indented);
                }
                if (userMS != null)
                {
                    int pageSizes = 99;
                    return PartialView("_PickCustomer", PaginatedList<UserDetailsModel>.CreateAsync(userMS, 1, pageSizes)); // This will be an view for dialog / modal
                }

            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> PickedCustomer(string phone)
        {
            OrderVM? orderModel = null;
            if (TempData["orderVM1"] != null)
            {
                string s = TempData["orderVM1"] as string ?? "";
                if (s != "")
                {
                    orderModel = JsonConvert.DeserializeObject<OrderVM>(s);
                }
                var user = await _staffService.GetUserByPhone(phone);
                if (user != null && orderModel != null)
                {
                    orderModel.Customer = user;
                    return PartialView("_PickedCustomer"/*Coult be a ViewModel in here*/); // This will be an update view for current customer table

                }
            }

            return NotFound();
        }

    }
}
