﻿using FlowerShopManagement.Application.Interfaces;
using FlowerShopManagement.Application.Models;
using FlowerShopManagement.Application.MongoDB.Interfaces;
using FlowerShopManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using ValueType = FlowerShopManagement.Core.Enums.ValueType;

namespace FlowerShopManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        //Services
        IStockServices _stockServices;
        //Repositories
        IVoucherRepository _voucherRepository;
        public VoucherController(IVoucherRepository voucherRepository, IStockServices stockServices)
        {
            _stockServices= stockServices;
            _voucherRepository= voucherRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Set up default values for ProductPage

            ViewData["VoucherCategories"] = Enum.GetValues(typeof(VoucherCategories)).Cast<VoucherCategories>().ToList();
            ViewData["ValueType"] = Enum.GetValues(typeof(ValueType)).Cast<ValueType>().ToList();
            ViewData["VoucherStatus"] = Enum.GetValues(typeof(VoucherStatus)).Cast<VoucherStatus>().ToList();
            List<VoucherDetailModel> productMs = await _stockServices.GetUpdatedVouchers(_voucherRepository);
            return View(/*Coult be a ViewModel in here*/);
        }

        //Open edit dialog / modal
        [HttpPost]
        public async Task<IActionResult> Edit(string id)
        {
            ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            //Should get a new one because an admin updates data realtime
            VoucherDetailModel editProduct = new VoucherDetailModel(await _voucherRepository.GetById(id));
            if (editProduct != null)
            {
                return View(/*Coult be a ViewModel in here*/);
            }
            return RedirectToAction("Index");
        }

        [HttpPut]
        public async Task<IActionResult> Update(VoucherDetailModel voucherM)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            //If product get null or Id null ( somehow ) => notfound
            if (voucherM == null || voucherM.Code == null) return NotFound();

            //Check if the product still exists
            var voucher = await _voucherRepository.GetById(voucherM.Code); // this may be eleminated
            if (voucher != null)
            {
                //If product != null => we will update this order by using directly orderModel.Id
                //Check ProductModel for sure if losing some data
                var result = await _voucherRepository.UpdateById(voucherM.Code, voucherM.ToEntity());
                if (result != false)
                {
                    //Update successfully, we pull new list of products
                    List<VoucherDetailModel> proMs = await _stockServices.GetUpdatedVouchers(_voucherRepository);

                    return RedirectToAction("Index"/*Coult be a ViewModel in here*/); // A updated _ViewAll

                }
            }
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Detele(VoucherDetailModel voucherDetailModel)
        {
            //ViewData["Categories"] = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();

            //If order get null or Id null ( somehow ) => notfound
            if (voucherDetailModel == null || voucherDetailModel.Code == null) return NotFound();
            //Check if the order still exists
            var product = await _voucherRepository.GetById(voucherDetailModel.Code);
            if (product != null)
            {
                //If order != null => we will detele this order by using directly ProductModel.Id
                //Check productModel for sure if losing some data
                var result = await _voucherRepository.RemoveById(voucherDetailModel.Code);
                if (result == false)
                    return RedirectToAction($"Unable to remove {voucherDetailModel.Code}");
                else
                {
                    //Detele successfully, we pull a new list of orders
                    List<VoucherDetailModel> productMs = await _stockServices.GetUpdatedVouchers(_voucherRepository);

                    return RedirectToAction("Index"/*Coult be a ViewModel in here*/); // A updated _ViewAll
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
            return View(/*Coult be a ViewModel in here*/);
        }

        // Confirm and create an Order
        [HttpPost]
        public async Task<IActionResult> Create(VoucherDetailModel voucherDetailModel)
        {
            var result = await _stockServices.CreateVoucher(voucherDetailModel, _voucherRepository);
            if (result == true)
            {
                List<VoucherDetailModel> orders = await _stockServices.GetUpdatedVouchers(_voucherRepository);
                return RedirectToAction("Index"/*Coult be a ViewModel in here*/); // A updated _ViewAll
            }
            return NotFound(); // Can be changed to Redirect
        }
    }
}