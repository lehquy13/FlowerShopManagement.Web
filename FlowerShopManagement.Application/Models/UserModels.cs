﻿using FlowerShopManagement.Core.Entities;
using FlowerShopManagement.Core.Enums;

namespace FlowerShopManagement.Application.Models;

public class UserModel
{
    public string _id { get; set; }

    // Account
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }

    // Profile
    public string Name { get; set; }
    public string Avatar { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthYear { get; set; }
    public string[] Addresses { get; set; }
    public List<string> FavProductIds { get; set; }

    // Extra
    public DateTime CreatedDate { get; private set; }
    public DateTime LastModified { get; set; }

    public UserModel(User entity)
    {
        _id = entity._id;

        Email = entity.email;
        PhoneNumber = entity.phoneNumber;
        Password = entity.password;
        Role = entity.role;

        Name = entity.name;
        Avatar = entity.avatar;
        Gender = entity.gender;
        BirthYear = entity.birthYear;
        Addresses = entity.addresses;
        FavProductIds = entity.favProductIds;

        CreatedDate = entity.createdDate;
        LastModified = entity.lastModified;
    }

    public UserModel()
    {
        //_id = new Guid().ToString();
        //_password = string.Empty;
        //CreatedDate = DateTime.Now;
        //Role = Role.Customer;
        //Gender = Gender.Female;
        //Addresses = new string[2];
    }

    public new void ToEntity(ref User entity)
    {
        //base.ToEntity(ref entity);

        //entity._id = _id;
        //entity.password = _password;
        //entity.role = Role;
        //entity.phoneNumber = PhoneNumber;
        //entity.gender = Gender;
        //entity.birthYear = BirthYear;
        //entity.addresses = Addresses;
        //entity.createdDate = CreatedDate;
        //entity.lastModified = LastModified;
    }

    public User ToNewEntity()
    {
        var entity = new User();

        //entity.password = _password;
        //entity.name = Name;
        //entity.avatar = Avatar;
        //entity.email = Email;
        //entity.phoneNumber = PhoneNumber;
        //entity.gender = Gender;
        //entity.birthYear = BirthYear;
        //entity.addresses = Addresses;

        return entity;
    }

    public bool IsPasswordMatched(string encryptedPassword)
    {
        //return Password == encryptedPassword;
        return true;
    }
}