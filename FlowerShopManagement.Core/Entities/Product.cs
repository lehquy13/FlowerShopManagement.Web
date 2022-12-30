﻿using FlowerShopManagement.Core.Enums;

namespace FlowerShopManagement.Core.Entities;

public class Product
{
    public string? _id { get; set; }
    public string _name { get; set; }
    public string _picture { get; set; }
    public Categories _categories { get; set; }
    public float _rating { get; set; }
    public int _uniPrice { get; set; }
    public int _amount { get; set; }
    public float _wholesaleDiscount { get; set; }
    public List<Review> _reviews { get; private set; } = new List<Review>();

    // adding attributes
    public Color _color { get; set; }
    public string _description { get; set; }
    public string _material { get; set; }
    public string _size { get; set; }
    public string _maintainment { get; set; }

    public Product(string? id = null,
        string name = "", string picture = "",
        Categories categories = Categories.Unknown, int amount = 0,
        int uniPrice = 0, float wholesaleDiscount = 0.0f, string description = "",
        string maintainment = "", string size = "0cm x 0cm x 0cm", string material = "", Color color = Color.Sample)
    {
        if (id != null)
            _id = id;
        _name = name;
        _picture = picture;
        _categories = categories;
        _amount = amount;
        _rating = 0.0f;
        _uniPrice = uniPrice;
        _wholesaleDiscount = wholesaleDiscount;
        _reviews = new List<Review>();
        _color = color;
        _description = description;
        _maintainment = maintainment;
        _size = size;
        _material = material;
    }
}
