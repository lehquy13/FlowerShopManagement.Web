﻿using FlowerShopManagement.Application.Interfaces.MongoDB;
using FlowerShopManagement.Application.Models;
using FlowerShopManagement.Core.Entities;
using FlowerShopManagement.Core.Enums;
using FlowerShopManagement.Infrustructure.MongoDB.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace FlowerShopManagement.Infrustructure.MongoDB.Implements;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(IMongoDBContext mongoDbContext) : base(mongoDbContext) { }

    public void PotentialProduct(DateTime beginDate, DateTime endDate, int limit = 5)
    {
        PipelineDefinition<Order, BsonDocument> pipeline = new BsonDocument[]
        {
            new BsonDocument("$match",
            new BsonDocument
            {
                { "_status", Status.Purchased },
                { "_date",
                new BsonDocument
                {
                    { "$gte", beginDate },
                    { "$lte", endDate }
                } }
            }),
            new BsonDocument("$unwind",
            new BsonDocument
            {
                { "path", "$_products" },
                { "preserveNullAndEmptyArrays", false }
            }),
            new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$_products._name" },
                { "totalCount",
                new BsonDocument("$sum", "$_products._amount") }
            }),
            new BsonDocument("$sort",
            new BsonDocument("totalCount", -1)),
            new BsonDocument("$limit", limit)
        };

        try
        {
            var result = _mongoDbCollection.Aggregate(pipeline).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void PotentialCustomer(DateTime beginDate, DateTime endDate, int limit = 5)
    {
        PipelineDefinition<Order, BsonDocument> pipeline = new BsonDocument[]
        {
            new BsonDocument("$match",
            new BsonDocument
            {
                { "_status", Status.Purchased },
                { "_date",
                new BsonDocument
                {
                    { "$gte", beginDate },
                    { "$lte", endDate }
                } }
            }),
            new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$_customerName" },
                { "totalCount", 
                new BsonDocument("$count", 
                new BsonDocument()) }
            }),
            new BsonDocument("$sort",
            new BsonDocument("totalCount", -1)),
            new BsonDocument("$limit", limit)
        };

        try
        {
            var result = _mongoDbCollection.Aggregate(pipeline).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void TotalCount(DateTime beginDate, DateTime endDate, string criteria = "$hour", Status? status = Status.Purchased)
    {
        PipelineDefinition<Order, BsonDocument> pipeline = new BsonDocument[]
        {
            new BsonDocument("$match",
            new BsonDocument
            {
                { "_status", status is null ?
                new BsonDocument("$exists", true) : status },
                { "_date",
                new BsonDocument
                {
                    { "$gte", beginDate },
                    { "$lte", endDate }
                }
            }}),
            new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", 
                new BsonDocument(criteria, "$_date") },
                { "countResult", 
                new BsonDocument("$count", new BsonDocument()) }
            })
        };

        try
        {
            var result = _mongoDbCollection.Aggregate(pipeline).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public List<SumAggregate> TotalSum(DateTime beginDate, DateTime endDate, string criteria = "$hour", Status status = Status.Purchased)
    {
        PipelineDefinition<Order, BsonDocument> pipeline = new BsonDocument[]
        {
            new BsonDocument("$match",
            new BsonDocument
            {
                { "_status", status },
                { "_date",
                new BsonDocument
                {
                    { "$gte", beginDate },
                    { "$lte", endDate }
                }
            }}),
            new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", 
                new BsonDocument(criteria, "$_date") },
                { "revenue", 
                new BsonDocument("$sum", "$_total") }
            })
        };

        try
        {
            var bsonList = _mongoDbCollection.Aggregate(pipeline).ToList();

            var result = new List<SumAggregate>();

            foreach (var bsonDoc in bsonList)
            {
                var model = BsonSerializer.Deserialize<SumAggregate>(bsonDoc);
                result.Add(model);
            }

            return result;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}