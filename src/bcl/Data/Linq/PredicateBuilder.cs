using System.Linq.Expressions;

namespace Library.Data.Linq;

/// <summary>
/// Provides methods for creating and combining predicates.
/// </summary>
/// <returns>An expression that can be used to test a condition.</returns>
public static class PredicateBuilder
{
    /// <summary>
    /// Combines two expressions using the AndAlso operator.
    /// </summary>
    /// <param name="expr1">The first expression.</param>
    /// <param name="expr2">The second expression.</param>
    /// <returns>A combined expression.</returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    /// <summary>
    /// Returns a predicate that always evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() =>
        f => false;

    /// <summary>
    /// Combines two expressions with an OR operator.
    /// </summary>
    /// <param name="expr1">The first expression.</param>
    /// <param name="expr2">The second expression.</param>
    /// <returns>A combined expression.</returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    /// <summary>
    /// Returns a predicate that always evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() =>
        f => true;
}