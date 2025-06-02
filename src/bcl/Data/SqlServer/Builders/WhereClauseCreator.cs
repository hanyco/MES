using Library.CodeGeneration;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

namespace Library.Data.SqlServer.Builders;

public static class WhereClauseCreator
{
    public static Result<string> GenerateCode([DisallowNull] WhereClauseCreatorModel args)
    {
        Check.MustBeArgumentNotNull(args);

        var result = new StringBuilder();
        foreach (var operation in args.Operations)
        {
            if (result.Length > 0)
            {
                _ = result.Append(" AND ");
            }
            _ = result.Append($"{SqlStatementBuilder.AddBrackets(operation.Field.Name)}");
            var operand = operation.Operator switch
            {
                WhereClauseCreatorFieldOperator.IsBiggerThan => $" > {operation.Parameters.ArgumentNotNull().First()}",
                WhereClauseCreatorFieldOperator.IsLessThan => $" < {operation.Parameters.ArgumentNotNull().First()}",
                WhereClauseCreatorFieldOperator.Contains => $" IS LIKE('%{operation.Parameters.ArgumentNotNull().First()}%')",
                WhereClauseCreatorFieldOperator.StartsWith => $" IS LIKE('{operation.Parameters.ArgumentNotNull().First()}%')",
                WhereClauseCreatorFieldOperator.EndsWith => $" IS LIKE('%{operation.Parameters.ArgumentNotNull().First()}')",
                WhereClauseCreatorFieldOperator.Equals => $" == {formatOperand(operation, operation.Parameters.ArgumentNotNull().First())}",
                WhereClauseCreatorFieldOperator.NotEquals => $" <> {formatOperand(operation, operation.Parameters.ArgumentNotNull().First())}",
                WhereClauseCreatorFieldOperator.IsNull => " IS NULL",
                WhereClauseCreatorFieldOperator.IsNotNull => " IS NOT NULL",
                _ => throw new NotImplementedException(),
            };
            _ = result.Append(operand);
        }

        var statement = result.ToString();
        return statement;

        static string formatOperand(WhereClauseCreatorOperation operation, object? operand)
        {
            Check.MustBeArgumentNotNull(operand);

            if (operation.Field.Type == typeof(int) || operation.Field.Type == typeof(long))
            {
                return operand.ToString()!;
            }
            if (operation.Field.Type == typeof(DateTime) || operation.Field.Type == typeof(DateOnly) || operation.Field.Type == typeof(TimeOnly))
            {
                return SqlTypeHelper.FormatDate(operand);
            }
            if (ObjectHelper.IsDbNull(operand))
            {
                return "null";
            };
            return $"'{operand}'";
        }
    }
}

public readonly record struct WhereClauseCreatorField(string Name, TypePath Type);

public readonly record struct WhereClauseCreatorOperation(WhereClauseCreatorField Field, WhereClauseCreatorFieldOperator Operator, IEnumerable<object?>? Parameters = null);

public enum WhereClauseCreatorFieldOperator
{
    IsBiggerThan,
    IsLessThan,
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    NotEquals,
    IsNull,
    IsNotNull,
}

public sealed class WhereClauseCreatorModel : IFactory<WhereClauseCreatorModel>
{
    private WhereClauseCreatorModel()
    {
    }

    public IEnumerable<WhereClauseCreatorOperation> Operations => this._operations;
    private HashSet<WhereClauseCreatorOperation> _operations { get; } = [];

    public static WhereClauseCreatorModel Create() =>
        new();

    public WhereClauseCreatorModel AddOperation(WhereClauseCreatorOperation operation)
    {
        Check.MustBe(this._operations.Add(operation));
        return this;
    }
}