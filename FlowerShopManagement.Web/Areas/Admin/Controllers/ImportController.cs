﻿using FlowerShopManagement.Application.Interfaces;
using FlowerShopManagement.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowerShopManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]")]
[Authorize(Policy = "StaffOnly")]
public class ImportController : Controller
{
    private readonly IStockService _stockService;
    private readonly ISupplierService _supplierService;
    private readonly IImportService _importService;
    private readonly IWebHostEnvironment _webHostEnv;

    private const string _reqTemplatePath = "/template/SupplyFormTemplate.html";

    public ImportController(
        IStockService stockService,
        IImportService importService,
        ISupplierService supplierService,
        IWebHostEnvironment webHostEnv)
    {
        _stockService = stockService;
        _importService = importService;
        _supplierService = supplierService;
        _webHostEnv = webHostEnv;
    }

    // IMPORT MAIN PAGE
    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            var requests = _importService.GetRequests();

            return View(requests);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    // REQUEST FORM
    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        // Load data
        var lowOnStockProducts = _stockService.GetLowOnStockProducts();
        var suppliers = await _supplierService.GetAllAsync();

        // Configure viewmodel
        var viewmodel = new ImportViewModel(PaginatedList<ProductModel>.CreateAsync(lowOnStockProducts, 1, 10), suppliers);

        return View(viewmodel);
    }

    [HttpPost]
    [ActionName("CreateRequestForm")]
    public async Task CreateRequestForm(List<string> productIds, List<int> requestQty, string supplierIds)
    {
        var products = await _stockService.GetByIdsAsync(productIds);
        var supplier = await _supplierService.GetOneAsync(supplierIds);
        var htmlPath = _webHostEnv.WebRootPath + _reqTemplatePath;

        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = HttpContext.User.FindFirst("Username")?.Value;

        if (userId is null || userName is null) return;

        if (products.Count <= 0 || requestQty.Count <= 0 || supplier == null)
        {
            throw new NullReferenceException("Insufficient Values!");
        }

        var reqForm = _importService.CreateRequestForm(
            products,
            supplier,
            requestQty,
            userId, userName, htmlPath);

        try
        {
            _importService.SendRequest(reqForm);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    // SUPPLY FORM
    [HttpGet("SupplyForm")]
    public IActionResult SupplyForm(ImportModel model)
    {
        return View(model);
    }
    
    // IMPORT DETAIL
    [HttpGet("Detail")]
    public async Task<IActionResult> Detail(string id)
    {
        var model = await _importService.GetRequest(id);
        return View(model);
    }
}

public class ImportViewModel
{
    public PaginatedList<ProductModel>? _lowOnStockProducts { get; set; }
    public List<SupplierModel>? _suppliers { get; set; } = new List<SupplierModel>();

    public ImportViewModel(
        PaginatedList<ProductModel>? lowOnStockProducts,
        List<SupplierModel>? suppliers)
    {
        _lowOnStockProducts = lowOnStockProducts;
        _suppliers = suppliers ?? new List<SupplierModel>();
    }
}