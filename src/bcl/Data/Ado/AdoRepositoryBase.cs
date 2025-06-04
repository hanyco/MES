using System.Runtime.CompilerServices;

using Library.Data.SqlServer;
using Library.Resulting;
using Library.Validations;

using Microsoft.Data.SqlClient;

using static Library.Data.SqlServer.SqlStatementBuilder;

namespace Library.Data.Ado;

public abstract class AdoRepositoryBase(in Sql sql)
{
    protected AdoRepositoryBase(in string connectionString)
        : this(Sql.Create(connectionString)) { }

    protected Sql Sql { get; } = sql;

    protected virtual async Task<Result> OnDeleteAsync<TEntity>(TEntity model, bool persist = true, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(model);
        Check.MustBe(persist, () => new NotSupportedException($"{nameof(persist)} must be true in this content."));

        var idColumn = Sql.FindIdColumn<TEntity>().NotNull(() => "ID column not found.").Value;
        var id = model.GetType().GetProperty(idColumn.Name)!.GetValue(model);
        var query = Delete<TEntity>().Where($"{idColumn.Name} = {id}").Build();

        try
        {
            var dbResult = await this.Sql.ExecuteNonQueryAsync(query, cancellationToken: token);
            return Result.Success(dbResult);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex);
        }
    }

    [return: NotNull]
    protected virtual async IAsyncEnumerable<TEntity> OnGetAll<TEntity>([DisallowNull] Func<SqlDataReader, TEntity> mapper, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = Select<TEntity>().WithNoLock().Build();
        await foreach (var entity in this.ExecuteReaderAsync(query, mapper.ArgumentNotNull(), cancellationToken))
        {
            yield return entity;
        }
    }

    [return: NotNull]
    protected virtual async IAsyncEnumerable<TEntity> OnGetAll<TEntity>([DisallowNull] string query, [DisallowNull] Func<SqlDataReader, TEntity> mapper, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var entity in this.ExecuteReaderAsync(query.ArgumentNotNull(), mapper.ArgumentNotNull(), cancellationToken))
        {
            yield return entity;
        }
    }

    [return: NotNull]
    protected virtual async IAsyncEnumerable<TEntity> OnGetAll<TEntity>([DisallowNull] string query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TEntity : new()
    {
        await foreach (var entity in this.ExecuteReaderAsync(query.ArgumentNotNull(), r => Mapper<TEntity>(r, typeof(TEntity).GetProperties()), cancellationToken))
        {
            yield return entity;
        }
    }

    [return: NotNull]
    protected virtual async IAsyncEnumerable<TEntity> OnGetAll<TEntity>([EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TEntity : new()
    {
        var query = Select<TEntity>().WithNoLock().Build();
        await foreach (var entity in this.ExecuteReaderAsync(query, r => Mapper<TEntity>(r, typeof(TEntity).GetProperties()), cancellationToken))
        {
            yield return entity;
        }
    }

    protected virtual Task<TEntity?> OnGetByIdAsync<TEntity>(object idValue, [DisallowNull] Func<SqlDataReader, TEntity> mapper, CancellationToken cancellationToken = default)
    {
        var query = Select<TEntity>().Top(1).Where($"{Sql.FindIdColumn<TEntity>()} = {idValue}").WithNoLock().Build();
        return this.ExecuteReaderAsync(query, mapper.ArgumentNotNull(), cancellationToken).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    protected virtual Task<TEntity?> OnGetByIdAsync<TEntity>(object idValue, CancellationToken cancellationToken = default)
        where TEntity : new()
    {
        var query = Select<TEntity>().Top(1).Where($"{Sql.FindIdColumn<TEntity>()!.Value.Name} = {idValue}").WithNoLock().Build();
        return this.ExecuteReaderAsync(query, r => Mapper<TEntity>(r, typeof(TEntity).GetProperties()), cancellationToken).FirstOrDefaultAsync();
    }

    protected virtual async Task<Result<TEntity>> OnInsertAsync<TEntity>(TEntity model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model);
        Check.MustBe(persist, () => new NotSupportedException($"{nameof(persist)} must be true in this content."));

        try
        {
            var idColumn = Sql.FindIdColumn<TEntity>().NotNull(() => "\"Id\" field not found.").Value;
            var query = Insert().Into<TEntity>().Values(model).ReturnId().Build();
            var dbResult = await this.Sql.ExecuteScalarCommandAsync(query, cancellationToken);
            var returnedId = dbResult.Cast().ToInt();
            model.GetType().GetProperty(idColumn.Name)!.SetValue(model, returnedId);
            return Result.Success(model)!;
        }
        catch (Exception ex)
        {
            return Result.Fail(ex, model)!;
        }
    }

    protected virtual async Task<Result<TEntity>> OnUpdateAsync<TEntity>(object idValue, TEntity model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model);
        Check.MustBe(persist, () => new NotSupportedException($"{nameof(persist)} must be true in this content."));
        try
        {
            var idColumn = Sql.FindIdColumn<TEntity>().Value;//.NotNull(() => "\"Id\" field not found.");
            var query = Update()
                .Table<TEntity>()
                .Set(model)
                .Where($"{idColumn.Name} = {idValue}")
                .ReturnId()
                .Build();
            var dbResult = await this.Sql.ExecuteScalarCommandAsync(query, cancellationToken);
            return Result.Success(model)!;
        }
        catch (Exception ex)
        {
            return Result.Fail(ex, model)!;
        }
    }

    private static TEntity Mapper<TEntity>(in SqlDataReader reader, in System.Reflection.PropertyInfo[] properties)
        where TEntity : new()
    {
        var result = new TEntity();
        foreach (var property in properties)
        {
            property.SetValue(result, reader[property.Name]);
        }
        return result;
    }

    [return: NotNull]
    private IAsyncEnumerable<TEntity> ExecuteReaderAsync<TEntity>([DisallowNull] string query, [DisallowNull] Func<SqlDataReader, TEntity> mapper, CancellationToken cancellationToken = default)
        => this.Sql.ExecuteReaderAsync(query, mapper, cancellationToken);
}