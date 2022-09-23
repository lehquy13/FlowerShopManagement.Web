﻿using FlowerShopManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FlowerShopManagement.Core.Interfaces
{
    public interface ICustomerService
    {
        //List<Customer> Get(string id);

        //Customer Create(Customer customer);

        //void Update(string id, Customer customer);

        //void Remove(string id);

        public List<Customer> Get();

        public Customer? Get(string id);

        public void Create(Customer customer);

        public void Remove(string id);

        public void Update(string id, Customer customer);
    }
}