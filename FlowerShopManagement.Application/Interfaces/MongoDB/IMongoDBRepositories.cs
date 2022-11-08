﻿using FlowerShopManagement.Core.Entities;
using System.Runtime.Serialization;

namespace FlowerShopManagement.Infrustructure.MongoDB.Interfaces;

public interface IBaseRepository<TEntity> : IDisposable where TEntity : class
{
    public Task<bool> Add(TEntity entity);

    public Task<IEnumerable<TEntity>> GetAll();

    public Task<TEntity> GetById(string id);

    public Task<TEntity> GetByField(string fieldName, IComparable value);

    public Task<bool> RemoveById(string id);

    public Task<bool> RemoveByField(string fieldName, IComparable value);

    public Task<bool> UpdateById(string id, TEntity entity);

    public Task<bool> UpdateByField(string fieldName, IComparable value, TEntity entity);
}

public interface IUserRepository : IBaseRepository<User> { }

public interface ICartRepository : IBaseRepository<Cart> { }