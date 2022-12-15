﻿using FlowerShopManagement.Application.Models;
using Microsoft.AspNetCore.Http;

namespace FlowerShopManagement.Application.Interfaces;

public interface IAuthService
{
    public Task<UserModel?> RegisterAsync(HttpContext httpContext, string email, string phoneNb, string password);
    public Task<UserModel?> SignInAsync(HttpContext httpContext, string emailOrPhoneNb, string password);
    public Task<bool> SignOutAsync(HttpContext httpContext);
    public Task<UserModel?> AuthenticateAsync(string id);
    public Task<UserDetailsModel> GetUserAsync(HttpContext httpContext);
    public string? GetUserId(HttpContext httpContext);
    public string? GetUserRole(HttpContext httpContext);
    public Task<UserModel?> GetUser(HttpContext httpContext);
}
