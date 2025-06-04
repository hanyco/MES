using System.Diagnostics;
using System.Reflection;

using Library.Data.Markers;
using Library.Interfaces;
using Library.Logging;
using Library.Results;
using Library.Threading;
using Library.Types;
using Library.Validations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Data.EntityFrameworkCore;

[DebuggerStepThrough, StackTraceHidden]
public static class DataServiceHelper
{
    #region CRUD
    public static async Task<IDbContextTransaction?> CreateTransactionOnDemand(this IBusinessService? _, DbContext dbContext, bool persist, CancellationToken token) =>
        !persist ? null : dbContext.Database.CurrentTransaction ?? await dbContext.Database.BeginTransactionAsync(token);

    /// <summary>
    /// Deletes an entity from the database.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The async write service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The model.</param>
    /// <param name="persist">Whether to persist the changes to the database.</param>
    /// <param name="detach">Whether to detach the entity from the database.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The result of the operation.</returns>
    public static async Task<Result<int>> Delete<TViewModel, TDbEntity>(this IAsyncWrite<TViewModel> service, [DisallowNull] DbContext dbContext, TViewModel model, bool persist, bool? detach = null, ILogger? logger = null)
        where TDbEntity : class, IIdenticalEntity<long>, new()
        where TViewModel : IHasKey<long?>
    {
        // Check if model and dbContext are not null
        if (!Checker.IfArgumentIsNull(model?.Id).TryParse(out var res1))
        {
            return res1.WithValue<int>(-1);
        }
        if (!Checker.IfArgumentIsNull(dbContext).TryParse(out var res2))
        {
            return res2.WithValue<int>(-1);
        }

        // Get the id of the model
        var id = model.Id.Value;
        logger?.Debug($"Deleting {nameof(TDbEntity)}, id={id}");

        // Remove the entity from the database
        _ = dbContext.RemoveById<TDbEntity>(id);

        // If persist is true, save the changes to the database
        if (persist)
        {
            try
            {
                var result = await dbContext.SaveChangesAsync();
                // If detach is true or persist is true, detach the entity from the database
                if (detach ?? persist)
                {
                    var entity = dbContext.Set<TDbEntity>().Where(e => id == e.Id).First();
                    _ = dbContext.Detach(entity);
                }

                // Log the deletion
                logger?.Info($"Deleted {nameof(TDbEntity)}, id={id}");

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Fail<int>(ex);
            }
        }

        return Result.Success(-1);
    }

    /// <summary>
    /// Retrieves a list of view models from the database asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="toViewModel">The function to convert from database entity to view model.</param>
    /// <param name="asyncLock">The asynchronous lock.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task<IReadOnlyList<TViewModel>> GetAll<TViewModel, TDbEntity>(this IAsyncRead<TViewModel> service, [DisallowNull] DbContext dbContext, Func<IEnumerable<TDbEntity?>, IEnumerable<TViewModel?>> toViewModel, AsyncLock asyncLock)
        where TDbEntity : class
        where TViewModel : class
        => service.GetAll(dbContext.ArgumentNotNull().Set<TDbEntity>(), toViewModel, asyncLock);

    /// <summary>
    /// Retrieves a list of view models from the given queryable of database entities.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="_">The async read service.</param>
    /// <param name="dbEntities">The queryable of database entities.</param>
    /// <param name="toViewModel">The function to convert the database entities to view models.</param>
    /// <param name="asyncLock">The async lock.</param>
    /// <returns>A list of view models.</returns>
    public static async Task<IReadOnlyList<TViewModel>> GetAll<TViewModel, TDbEntity>(this IAsyncRead<TViewModel> _, [DisallowNull] IQueryable<TDbEntity> dbEntities, Func<IEnumerable<TDbEntity?>, IEnumerable<TViewModel?>> toViewModel, AsyncLock asyncLock)
        where TDbEntity : class
        where TViewModel : class
    {
        var query = from entity in dbEntities
                    select entity;
        var dbResult = await query.AsNoTracking().ToListLockAsync(asyncLock);
        //var result = dbResult?.Any() is true ? toViewModel(dbResult).ToReadOnlyList() : Enumerable.Empty<TViewModel>().ToReadOnlyList();
        var result = toViewModel.ArgumentNotNull()(dbResult).Compact().ToReadOnlyList();
        return result;
    }

    /// <summary>
    /// Asynchronously gets an entity by its id from the database and converts it to a view model.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="id">The id of the entity.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="toViewModel">The function to convert the entity to a view model.</param>
    /// <param name="asyncLock">The asynchronous lock.</param>
    /// <returns>The view model.</returns>
    public static Task<TViewModel?> GetById<TViewModel, TDbEntity>(this IAsyncRead<TViewModel> service, long id, [DisallowNull] DbContext dbContext, Func<TDbEntity?, TViewModel?> toViewModel, AsyncLock asyncLock)
        where TDbEntity : class, IIdenticalEntity<long>
        where TViewModel : class
        => service.GetById(id, dbContext.ArgumentNotNull().Set<TDbEntity>(), toViewModel, asyncLock);

    /// <summary>
    /// Retrieves an entity from the database by its Id and converts it to a ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service used to read the ViewModel.</param>
    /// <param name="id">The Id of the entity to retrieve.</param>
    /// <param name="entities">The queryable entities.</param>
    /// <param name="toViewModel">The function used to convert the entity to a ViewModel.</param>
    /// <param name="asyncLock">The async lock.</param>
    /// <returns>The ViewModel.</returns>
    public static async Task<TViewModel?> GetById<TViewModel, TDbEntity>(this IAsyncRead<TViewModel> service, long id, [DisallowNull] IQueryable<TDbEntity> entities, Func<TDbEntity?, TViewModel?> toViewModel, AsyncLock asyncLock)
        where TDbEntity : IHasKey<long>
    {
        Check.MustBeArgumentNotNull(entities, nameof(entities));
        Check.MustBeArgumentNotNull(toViewModel, nameof(toViewModel));
        var query = from entity in entities
                    where entity.Id == id
                    select entity;
        var dbResult = await query.FirstOrDefaultLockAsync(asyncLock);
        return toViewModel(dbResult);
    }

    /// <summary>
    /// Inserts a new entity into the database asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The model.</param>
    /// <param name="convert">The convert function.</param>
    /// <param name="validatorAsync">The validator asynchronous.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <param name="saveChanges">The save changes function.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="onCommitting">The on committing function.</param>
    /// <param name="onCommitted">The on committed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="TaskResultManipulationResultTViewModel, TDbEntity?"/> representing the
    /// asynchronous operation.
    /// </returns>
    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Insert<TViewModel, TDbEntity>(
        this IAsyncWrite<TViewModel> service,
        [DisallowNull] DbContext dbContext,
        [DisallowNull] TViewModel model,
        [DisallowNull] Func<TViewModel, TDbEntity?> convert,
        Func<TViewModel, CancellationToken, Task<Result<TViewModel>>>? validatorAsync = null,
        bool persist = true,
        Func<CancellationToken, Task<Result<int>>>? saveChanges = null,
        ILogger? logger = null,
        Func<TDbEntity, TDbEntity>? onCommitting = null,
        Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<long> =>
        InnerManipulate<TViewModel, TDbEntity, long>(dbContext, model, convert, validatorAsync, dbContext.NotNull().Add, null, persist, (true, null), saveChanges, onCommitted, logger, cancellationToken);

    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Insert<TViewModel, TDbEntity>(
        this IAsyncWrite<TViewModel> service,
        [DisallowNull] DbContext dbContext,
        [DisallowNull] TViewModel model,
        [DisallowNull] Func<TViewModel, TDbEntity?> convert,
        Func<TViewModel, Result<TViewModel>>? validator = null,
        bool persist = true,
        Func<CancellationToken, Task<Result<int>>>? saveChanges = null,
        ILogger? logger = null,
        Func<TDbEntity, TDbEntity>? onCommitting = null,
        Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<long> =>
        InnerManipulate<TViewModel, TDbEntity, long>(
            dbContext,
            model,
            convert,
            (x, _) => validator != null ? Task.FromResult(validator(x)) : Task.FromResult(Result.Success(x)),
            dbContext.NotNull().Add,
            null,
            persist,
            (true, null),
            saveChanges,
            onCommitted,
            logger,
            cancellationToken);

    /// <summary>
    /// Inserts a new entity into the database asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The model.</param>
    /// <param name="convert">The convert function.</param>
    /// <param name="validatorAsync">The validator asynchronous.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <param name="saveChanges">The save changes function.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="onCommitting">The on committing function.</param>
    /// <param name="onCommitted">The on committed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="TaskResultManipulationResultTViewModel, TDbEntity?"/> representing the
    /// asynchronous operation.
    /// </returns>
    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Insert<TViewModel, TDbEntity, TId>(
        [DisallowNull] IAsyncRead<TViewModel> service,
        [DisallowNull] DbContext dbContext,
        [DisallowNull] TViewModel model,
        [DisallowNull] Func<TViewModel, TDbEntity?> convert,
        Func<TViewModel, CancellationToken, Task<Result<TViewModel>>>? validatorAsync = null,
        bool persist = true,
        Func<CancellationToken, Task<Result<int>>>? saveChanges = null,
        ILogger? logger = null,
        Func<TDbEntity, TDbEntity>? onCommitting = null,
        Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<TId>
        where TId : notnull
        => InnerManipulate<TViewModel, TDbEntity, TId>(dbContext, model, convert, validatorAsync, dbContext.NotNull().Add, null, persist, (true, null), saveChanges, onCommitted, logger, cancellationToken);

    /// <summary>
    /// Executes an asynchronous insert operation for a given view model, database entity, and service.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The model.</param>
    /// <param name="convert">The convert function.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <param name="logger">The logger.</param>
    /// <param name="onCommitting">The on committing function.</param>
    /// <param name="onCommitted">The on committed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="TaskResultManipulationResultTViewModel, TDbEntity?"/> representing the
    /// asynchronous operation.
    /// </returns>
    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Insert<TService, TViewModel, TDbEntity>(
        this TService service,
        [DisallowNull] DbContext dbContext,
        [DisallowNull] TViewModel model,
        [DisallowNull] Func<TViewModel, TDbEntity?> convert,
        bool persist = true,
        ILogger? logger = null,
        Func<TDbEntity, TDbEntity>? onCommitting = null,
        Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<long>
        where TService : IAsyncWrite<TViewModel>, IAsyncValidator<TViewModel>, IAsyncSaveChanges
        => InnerManipulate<TViewModel, TDbEntity, long>(dbContext, model, convert, service.ValidateAsync, dbContext.Add, onCommitting, persist, (true, null), service.SaveChangesAsync, onCommitted, logger, cancellationToken);

    /// <summary>
    /// Asynchronously inserts a new entity into the database using the given service, view model,
    /// and database entity.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The view model.</param>
    /// <param name="convert">The conversion function.</param>
    /// <param name="persist">Whether to persist the changes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Insert<TService, TViewModel, TDbEntity>(
        this TService service,
        [DisallowNull] DbContext dbContext,
        [DisallowNull] TViewModel model,
        [DisallowNull] Func<TViewModel, TDbEntity?> convert,
        bool persist = true,
        (bool UseTransaction, IDbContextTransaction? Transaction)? transactionInfo = null,
        CancellationToken cancellationToken = default)
        where TViewModel : ICanSetKey<long?>
        where TDbEntity : class, IIdenticalEntity<long>
        where TService : IAsyncWrite<TViewModel>, IAsyncValidator<TViewModel>, IAsyncSaveChanges, ILoggerContainer
        => InnerManipulate<TViewModel, TDbEntity, long>(
                dbContext,
                model,
                convert,
                service.ValidateAsync,
                dbContext.Add,
                null,
                persist,
                transactionInfo ?? (true, null),
                service.SaveChangesAsync,
                onCommitted: (m, e) => m.Id = e.Id,
                logger: service.Logger, cancellationToken: cancellationToken);

    /// <summary>
    /// Executes an update operation on the specified service, dbContext, model, and convert.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The model.</param>
    /// <param name="convert">The convert.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <param name="onCommitting">The on committing.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="onCommitted">The on committed.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="TaskResultManipulationResultTViewModel, TDbEntity?"/> representing the
    /// asynchronous operation.
    /// </returns>
    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Update<TService, TViewModel, TDbEntity>(this TService service, [DisallowNull] DbContext dbContext, [DisallowNull] TViewModel model, [DisallowNull] Func<TViewModel, TDbEntity?> convert, bool persist = true, Func<TDbEntity, TDbEntity>? onCommitting = null, ILogger? logger = null,
        Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<long>
        where TService : IAsyncWrite<TViewModel>, IAsyncValidator<TViewModel>, IAsyncSaveChanges
        => InnerManipulate<TViewModel, TDbEntity, long>(dbContext, model, convert, service.ValidateAsync, dbContext.Attach, onCommitting, persist, (true, null), service.SaveChangesAsync, onCommitted, logger, cancellationToken);

    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Update<TService, TViewModel, TDbEntity>(this TService service, [DisallowNull] DbContext dbContext, [DisallowNull] TViewModel model, [DisallowNull] Func<TViewModel, TDbEntity?> convert, bool persist = true, Func<TViewModel, Result<TViewModel>>? validator = null, Func<TDbEntity, TDbEntity>? onCommitting = null, ILogger? logger = null,
            Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<long>
        where TService : IAsyncWrite<TViewModel>, IAsyncSaveChanges
        => InnerManipulate<TViewModel, TDbEntity, long>(dbContext, model, convert, (x, _) => validator != null ? validator(x).ToAsync() : Result.Success(x).ToAsync(), dbContext.Attach, onCommitting, persist, (true, null), service.SaveChangesAsync, onCommitted, logger, cancellationToken);

    /// <summary>
    /// Updates an entity in the database asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The model.</param>
    /// <param name="convert">The convert function.</param>
    /// <param name="validatorAsync">The validator asynchronous.</param>
    /// <param name="onCommitting">The on committing function.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <param name="saveChanges">The save changes function.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="onCommitted">The on committed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A <see cref="TaskResultManipulationResultTViewModel, TDbEntity?"/> representing the
    /// asynchronous operation.
    /// </returns>
    public static Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> Update<TViewModel, TDbEntity, TId>(this IAsyncWrite<TViewModel> service, [DisallowNull] DbContext dbContext, [DisallowNull] TViewModel model, [DisallowNull] Func<TViewModel, TDbEntity?> convert, Func<TViewModel, CancellationToken, Task<Result<TViewModel>>>? validatorAsync = null, Func<TDbEntity, TDbEntity>? onCommitting = null, bool persist = true, Func<CancellationToken, Task<Result<int>>>? saveChanges = null, ILogger? logger = null,
        Action<TViewModel, TDbEntity>? onCommitted = null, CancellationToken cancellationToken = default)
        where TDbEntity : class, IIdenticalEntity<TId>
        where TId : notnull
        => InnerManipulate<TViewModel, TDbEntity, TId>(dbContext, model!, convert, validatorAsync, dbContext.Update, onCommitting, persist, (true, null), saveChanges, onCommitted, logger, cancellationToken);

    /// <summary>
    /// Executes manipulation of a given entity and returns the result.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    /// <param name="dbContext">The database context.</param>
    /// <param name="model">The view model.</param>
    /// <param name="convertToEntity">The function to convert the view model to an entity.</param>
    /// <param name="validatorAsync">The function to validate the view model.</param>
    /// <param name="manipulate">The function to manipulate the entity.</param>
    /// <param name="onCommitting">The function to execute before committing the entity.</param>
    /// <param name="persist">A value indicating whether to persist the changes.</param>
    /// <param name="transactionInfo">The transaction information.</param>
    /// <param name="saveChanges">The function to save the changes.</param>
    /// <param name="onCommitted">The function to execute after committing the entity.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the manipulation.</returns>
    private static async Task<Result<ManipulationResult<TViewModel, TDbEntity?>>> InnerManipulate<TViewModel, TDbEntity, TId>(
        DbContext dbContext,
        TViewModel model,
        Func<TViewModel, TDbEntity?> convertToEntity,
        Func<TViewModel, CancellationToken, Task<Result<TViewModel>>>? validatorAsync,
        Func<TDbEntity, EntityEntry<TDbEntity>> manipulate,
        Func<TDbEntity, TDbEntity>? onCommitting,
        bool persist,
        (bool UseTransaction, IDbContextTransaction? Transaction)? transactionInfo,
        Func<CancellationToken, Task<Result<int>>>? saveChanges,
        Action<TViewModel, TDbEntity>? onCommitted,
        ILogger? logger, CancellationToken cancellationToken)
        where TDbEntity : class, IIdenticalEntity<TId>
        where TId : notnull
    {
        Checker.MustBeNotNull(manipulate);
        Checker.MustBeNotNull(convertToEntity);
        //! Check that all arguments are not null
        if (!Checker.IfArgumentIsNull(model).TryParse(out var res1))
        {
            return getResult(res1);
        }

        //! Log that manipulation has started
        if (persist)
        {
            logger?.Debug($"Manipulation started. Entity: {nameof(TDbEntity)}, Action:{nameof(manipulate)}");
        }

        //! Validate manipulation
        if (validatorAsync is not null)
        {
            var validationResult = await validatorAsync(model, cancellationToken);
            if (validationResult.IsFailure)
            {
                return getResult(validationResult, (model, null));
            }
        }

        //! Convert model to entity and execute onCommitting if not null
        var entity = convertToEntity(model) // Convert model to entity
            .NotNull(() => "Entity cannot be null.");
        if (onCommitting is not null)
        {
            onCommitting(entity);
        }

        //! Setup transaction
        IDbContextTransaction? transaction;
        if (persist && transactionInfo is { } t and { UseTransaction: true } && dbContext.Database.CurrentTransaction is null)
        {
            transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
        else
        {
            transaction = null;
        }

        //! Execute manipulation
        var entry = manipulate(entity);

        //! Persist changes
        Result<int> saveResult;
        if (persist)
        {
            try
            {
                if (transaction is not null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
                saveResult = await (saveChanges is not null
                    ? saveChanges(cancellationToken)
                    : CatchResult(() => dbContext.SaveChangesAsync(cancellationToken)));
                if (saveResult.IsSucceed && onCommitted is not null)
                {
                    onCommitted(model, entity);
                }
            }
            finally
            {
                //! Detach entity from current scope
                _ = DbContextHelper.Detach<DbContext, TDbEntity, TId>(dbContext, entity);
            }
        }
        else
        {
            saveResult = Result.Success(-1);
        }

        //! Log result of manipulation
        var result = getResult(saveResult, (model, entity));
        if (persist)
        {
            if (result.IsSucceed)
            {
                logger?.Debug($"Manipulation ended successfully. Entity: {nameof(TDbEntity)}, Action:{nameof(manipulate)}");
            }
            else
            {
                logger?.Debug($"Manipulation ended with error. Entity: {nameof(TDbEntity)}, Action:{nameof(manipulate)}");
            }
        }
        return result;

        //! Helper function to return result
        static Result<ManipulationResult<TViewModel, TDbEntity?>> getResult(Result result, ManipulationResult<TViewModel, TDbEntity?> entity = default)
            => Result.From(result, entity);
    }
    #endregion CRUD

    #region RegisterServices

    public static IServiceCollection RegisterServices<TService>(this IServiceCollection services, Assembly assembly)
        => services.RegisterServices<TService>(assembly, assembly);

    /// <summary>
    /// Registers services of type TService from the specified interface and service assemblies.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="interfaceModule">The interface module.</param>
    /// <param name="serviceModule">The service module.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection RegisterServices<TService>(this IServiceCollection services, Type interfaceModule, Type serviceModule)
            => services.RegisterServices<TService>(interfaceModule.Assembly, serviceModule.Assembly);

    /// <summary>
    /// Registers services from two assemblies based on a given interface.
    /// </summary>
    /// <typeparam name="TServiceInterface">The interface to register services for.</typeparam>
    /// <param name="serviceCollection">The service collection to add services to.</param>
    /// <param name="interfaceAsm">The assembly containing the interface.</param>
    /// <param name="serviceAsm">The assembly containing the services.</param>
    /// <param name="add">An optional action to add services to the service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection RegisterServices<TServiceInterface>(
            this IServiceCollection serviceCollection,
            in Assembly interfaceAsm,
            in Assembly serviceAsm,
            in Action<(IServiceCollection ServiceCollection, Type ServiceInterface, Type ServiceType)>? add = default)
    {
        //Declare a function to add services to the ServiceCollection
        var addToServices = add ?? ((x) => _ = x.ServiceCollection.AddScoped(x.ServiceInterface, x.ServiceType));

        //Get a list of all interfaces that implement TServiceInterface
        var interfaces = interfaceAsm.GetTypes().Where(t => t.IsInterface && t.GetInterfaces().Contains(typeof(TServiceInterface))).ToList();

        //Get a list of all classes that implement TServiceInterface
        var services = serviceAsm.GetTypes().Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(TServiceInterface))).ToList();

        //Loop through each interface and check if there is a corresponding service
        foreach (var iface in interfaces)
        {
            foreach (var svc in services)
            {
                //If a service is found, add it to the ServiceCollection
                if (svc.GetInterface(iface.Name) != null)
                {
                    addToServices((serviceCollection, iface, svc));
                    break;
                }
            }
        }

        //Return the ServiceCollection
        return serviceCollection;
    }

    /// <summary>
    /// Registers services with IService from the assembly of the specified type.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup.</typeparam>
    /// <param name="services">The services.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection RegisterServicesWithIService<TStartup>(this IServiceCollection services)
            => services.RegisterServices<IService>(typeof(TStartup).Assembly);

    #endregion RegisterServices

    #region Save & Submit Changes

    /// <summary>
    /// Saves a view model asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <returns>A <see cref="TaskResultTViewModel"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This code checks if the model's Id is not null and greater than 0, and then either updates
    /// or inserts the model depending on the result.
    /// </remarks>
    public static Task<Result<TViewModel>> SaveViewModel<TViewModel>(this IAsyncWrite<TViewModel> service, TViewModel model, bool persist = true, CancellationToken cancellationToken = default)
        where TViewModel : ICanSetKey<long?>
    {
        Checker.MustBeArgumentNotNull(service);
        Checker.MustBeArgumentNotNull(model);

        if (model.Id is { } id && id > 0)
        {
            //If the Id is not null and greater than 0, update the model
            return service.Update(id, model, persist, cancellationToken);
        }
        else
        {
            //If the Id is null or less than 0, insert the model
            return service.Insert(model, persist, cancellationToken);
        }
    }

    /// <summary>
    /// Saves a view model asynchronously.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <param name="persist">if set to <c>true</c> [persist].</param>
    /// <returns>A <see cref="TaskResultTViewModel"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This code checks if the model has an ID, and if it does, it updates the model, otherwise it
    /// inserts it.
    /// </remarks>
    public static Task<Result<TViewModel>> SaveViewModel<TViewModel>(this IAsyncWrite<TViewModel, Id> service, TViewModel model, bool persist = true)
        where TViewModel : ICanSetKey<Id>
    {
        //Check if the model is not null and has an ID
        if (!model.ArgumentNotNull().Id.IsNullOrEmpty())
        {
            //If it does, update the model
            return service.Update(model.Id, model, persist);
        }
        else
        {
            //Otherwise, insert the model
            return service.Insert(model, persist);
        }
    }

    /// <summary>
    /// Submits changes to the database asynchronously.
    /// </summary>
    /// <param name="service">The service to submit changes to.</param>
    /// <param name="persist">Whether or not to persist the changes.</param>
    /// <param name="transaction">The transaction to commit.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    public static async Task<Result<int>> SubmitChanges<TService>(this TService service, bool persist = true, IDbContextTransaction? transaction = null, CancellationToken token = default)
        where TService : IAsyncSaveChanges, IResetChanges
    {
        // If persist is false, return a success result with -1
        if (!persist)
        {
            return Result.Success(-1);
        }

        try
        {
            // Save the changes and store the result
            var result = await service.SaveChangesAsync(token);
            // If the result is successful, reset the changes
            if (!result.IsSucceed)
            {
                service.ResetChanges();
                return result;
            }

            // If a transaction is provided, commit it
            if (transaction is not null)
            {
                await transaction.CommitAsync(token);
            }

            // Return the result
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Fail<int>(ex);
        }
    }

    #endregion Save & Submit Changes

    /// <summary>
    /// Converts a Task of a Result of a ManipulationResult to a Task of a Result of the Model.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
    /// <param name="manipulationResultTask">The manipulation result task.</param>
    /// <returns>A Task of a Result of the Model.</returns>
    public static Task<Result<TViewModel>> ModelResult<TViewModel, TDbEntity>(this Task<Result<ManipulationResult<TViewModel, TDbEntity>>> manipulationResultTask)
        => manipulationResultTask.ToResultAsync(x => x.Model);
}

/// <summary>
/// Represents a manipulation result containing a view model and a database entity.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <typeparam name="TDbEntity">The type of the database entity.</typeparam>
/// <returns>A manipulation result containing a view model and a database entity.</returns>
public record struct ManipulationResult<TViewModel, TDbEntity>(in TViewModel Model, in TDbEntity Entity)
{
    public static implicit operator (TViewModel Model, TDbEntity? Entity)(ManipulationResult<TViewModel, TDbEntity> value) => (value.Model, value.Entity);
    public static implicit operator ManipulationResult<TViewModel, TDbEntity>((TViewModel Model, TDbEntity Entity) value) => new(value.Model, value.Entity);
    public static implicit operator TViewModel(ManipulationResult<TViewModel, TDbEntity> value) => value.Model;
}