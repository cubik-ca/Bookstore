using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using NUlid;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Bookstore.RavenDB;

public static class RavenDbExtensions
{
    public static ActionResult<IReadOnlyList<T>> RunApiQuery<T>(
        this IAsyncDocumentSession session,
        Func<IAsyncDocumentSession, IReadOnlyList<T>> query)
    {
        try
        {
            return new OkObjectResult(query(session));
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
        this IAsyncDocumentSession session,
        Func<IAsyncDocumentSession, T?> query)
    {
        try
        {
            var item = query(session);
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
        this IAsyncDocumentSession session,
        string id,
        Func<T, Task> update)
    {
        var item = await session.LoadAsync<T>(id);
        await update(item);
        await session.SaveChangesAsync();
    }

    public static async Task Del<T>(
        this IAsyncDocumentSession session,
        string id)
    {
        var item = await session.LoadAsync<T>(id);
        session.Delete(item);
        await session.SaveChangesAsync();
    }

    public static async Task UpsertItem<T>(
        this IAsyncDocumentSession session,
        string id,
        Func<T, Task> update,
        Func<Task<T>> create)
    {
        if (await session.Advanced.ExistsAsync(id))
        {
            var item = await session.LoadAsync<T>(id);
            await update(item);
        }
        else
        {
            var item = await create();
            await session.StoreAsync(item);
        }
        await session.SaveChangesAsync();
    }

    public static async Task UpdateMultipleItems<T>(
        this IAsyncDocumentSession session,
        Expression<Func<T, bool>> query,
        Func<T, Task> update)
    {
        var items = session.Query<T>().Where(query);
        foreach (var item in await items.ToListAsync())
            await update(item);
        await session.SaveChangesAsync();
    }

    public static async Task Create<T>(
        this IAsyncDocumentSession session,
        Func<Task<T>> create)
    {
        var newItem = await create();
        await session.StoreAsync(newItem);
        await session.SaveChangesAsync();
    }

    public static void EnsureDatabaseExists(this IDocumentStore store, string databaseName)
    {
        try
        {
            store.Maintenance.ForDatabase(databaseName).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(databaseName)));
        }
    }
}