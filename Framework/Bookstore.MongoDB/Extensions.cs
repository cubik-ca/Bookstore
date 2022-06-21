using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Bookstore.MongoDB;

public static class Extensions
{
    public static ActionResult<IReadOnlyList<T>> RunApiQuery<T>(
        this IMongoCollection<T> collection,
        Func<IMongoCollection<T>, IReadOnlyList<T>> query)
    {
        try
        {
            return new OkObjectResult(query(collection));
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(
                new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
        }
    }

    public static ActionResult<T?> RunApiQuerySingle<T>(
        this IMongoCollection<T> collection,
        Func<IMongoCollection<T>, T?> query)

    {
        try
        {
            var item = query(collection);
            return new OkObjectResult(item);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(
                new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
        }
    }

    public static async Task Update<T>(
        this IMongoCollection<T> collection,
        string id,
        Func<T, Task> update)
    {
        var item = await collection.Find(Builders<T>.Filter.Eq("_id", id)).SingleOrDefaultAsync();
        await update(item);
        await collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), item);
    }

    public static async Task Delete<T>(
        this IMongoCollection<T> collection,
        string id)
    {
        await collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
    }

    public static async Task UpsertItem<T>(
        this IMongoCollection<T> collection,
        string id,
        Func<T, Task> update,
        Func<Task<T>> create)
    {
        var item = await collection.Find(Builders<T>.Filter.Eq("_id", id)).SingleOrDefaultAsync();
        if (item == null)
        {
            item = await create();
            await collection.InsertOneAsync(item);
        }
        else
        {
            await update(item);
            await collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), item);
        }
    }

    public static async Task UpdateMultipleItems<T>(
        this IMongoCollection<T> collection,
        Expression<Func<T, bool>> query,
        Func<T, Task> update)
    {
        var writes = new List<ReplaceOneModel<T>>();
        foreach (var item in collection.AsQueryable().Where(query))
        {
            await update(item);
            writes.Add(new ReplaceOneModel<T>(Builders<T>.Filter.Eq("id", "Id"), item));
        }

        await collection.BulkWriteAsync(writes);
    }

    public static async Task Create<T>(
        this IMongoCollection<T> collection,
        Func<Task<T>> create)
    {
        var newItem = await create();
        await collection.InsertOneAsync(newItem);
    }
}