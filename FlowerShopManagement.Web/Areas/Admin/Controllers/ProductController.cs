﻿using FlowerShopManagement.Application.Interfaces;
using FlowerShopManagement.Application.Models;
using FlowerShopManagement.Application.MongoDB.Interfaces;
using FlowerShopManagement.Core.Entities;
using FlowerShopManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShopManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //Services
        IImportServices _importServices;
        IStockServices _stockServices;
        //Repositories
        IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository, IImportServices importServices, IStockServices stockServices)
        {
            _productRepository = productRepository;
            _importServices = importServices;
            _stockServices = stockServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Set up default values for ProductPage

            ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            List<ProductModel> productMs = await GetUpdatedProducts();
            return View(/*Coult be a ViewModel in here*/);
        }

        //Open edit dialog / modal
        [HttpPost]
        public async Task<IActionResult> Edit(string id)
        {
            ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            //Should get a new one because an admin updates data realtime
            var editProduct = await _productRepository.GetById(id);
            if (editProduct != null)
            {
                return PartialView(/*Coult be a ViewModel in here*/);
            }
            return RedirectToAction("Index");
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductModel productModel)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            //If product get null or Id null ( somehow ) => notfound
            if (productModel == null || productModel.Id == null) return NotFound();

            //Check if the product still exists
            var product = await _productRepository.GetById(productModel.Id); // this may be eleminated
            if (product != null)
            {
                //If product != null => we will update this order by using directly orderModel.Id
                //Check ProductModel for sure if losing some data
                var result = await _productRepository.UpdateById(productModel.Id, productModel.ToEntity());
                if (result != false)
                {
                    //Update successfully, we pull new list of products
                    List<ProductModel> proMs = await GetUpdatedProducts();

                    return PartialView(/*Coult be a UPDATED ViewModel in here*/);//for example: a _ViewAll 

                }
            }
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Detele(ProductModel productModel)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            //If order get null or Id null ( somehow ) => notfound
            if (productModel == null || productModel.Id == null) return NotFound();
            //Check if the order still exists
            var product = await _productRepository.GetById(productModel.Id);
            if (product != null)
            {
                //If order != null => we will detele this order by using directly ProductModel.Id
                //Check productModel for sure if losing some data
                var result = await _productRepository.RemoveById(productModel.Id);
                if (result == false)
                    return RedirectToAction($"Unable to remove {productModel.Id}");
                else
                {
                    //Detele successfully, we pull a new list of orders
                    List<ProductModel> productMs = await GetUpdatedProducts();

                    return PartialView(/*Coult be a UPDATED ViewModel in here*/); //For example: a _ViewAll
                }
            }
            return NotFound();
        }

        //Open an Create Dialog
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            //Set up default values for OrderPage

            ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            /*Set up viewmodel
			 


			*/
            return PartialView(/*Coult be a ViewModel in here*/); 
        }

        // Confirm and create an Order
        [HttpPost]
        public async Task<IActionResult> Create(ProductModel productModel)
        {
            var result = _saleServices.CreateOfflineOrder(productModel, userModel, _orderRepository, _userRepository, _productRepository);
            if (result != null)
            {
                List<OrderModel> orders = await GetUpdatedOrders();
                return PartialView(/*Coult be a ViewModel in here*/); // A updated _ViewAll
            }
            return NotFound(); // Can be changed to Redirect
        }

        public async Task<List<ProductModel>> GetUpdatedProducts()
        {
            List<Product> products = await _productRepository.GetAll();
            List<ProductModel> productMs = new List<ProductModel>();

            foreach (var o in products)
            {
                productMs.Add(new ProductModel(o));
            }
            return productMs;
        }
    }
}
