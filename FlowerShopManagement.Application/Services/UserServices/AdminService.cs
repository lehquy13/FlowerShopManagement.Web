﻿using FlowerShopManagement.Application.Interfaces.UserSerivices;
using FlowerShopManagement.Application.Models;
using FlowerShopManagement.Application.MongoDB.Interfaces;
using FlowerShopManagement.Core.Entities;
using FlowerShopManagement.Core.Enums;

namespace FlowerShopManagement.Application.Services.UserServices;

public class AdminService : StaffService, IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AdminService(
        IUserRepository userRepository, 
        ICartRepository cartRepository, 
        ISupplierRepository supplierRepository) 
        : base(userRepository, cartRepository, supplierRepository)
    {
        _userRepository = userRepository;
        _cartRepository = cartRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<bool> AddStaffAsync(UserDetailsModel newStaffModel, Role role)
    {
        try
        {
            // Model to entity
            var staff = newStaffModel.ToNewEntity();

            // Set default password - "1"
            var defaultPassword = Validator.MD5Hash("1");
            staff.password = defaultPassword;

            // Set role
            if (role == Role.Customer) return false; 
            staff.role = role;

            // Add to database
            return await _userRepository.Add(staff);
        }
        catch
        {
            // Failed to add new staff account
            return false;
        }
    }

    public async Task<bool> AddSupplierAsync(SupplierDetailModel newSupplierModel)
    {
        try
        {
            // Model to entity
            var supplier = newSupplierModel.ToNewEntity();

            // Added to database
            return await _supplierRepository.Add(supplier);
        }
        catch
        {
            // Failed to add new supplier
            return false;
        }
    }

    public Task<bool> RemoveSupplierAsync(SupplierModel supplierModel)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EditUserRoleAsync(UserDetailsModel userModel, Role role)
    {
        var staff = new User();

        try
        {
            // Model to entity
            userModel.ToEntity(ref staff);

            // Set role
            staff.role = role;

            // Set modified date
            staff.lastModified = DateTime.Now;

            // Update database
            return await _userRepository.UpdateById(staff._id, staff);
        }
        catch
        {
            // Failed to edit user's role
            return false;
        }
    }
}
