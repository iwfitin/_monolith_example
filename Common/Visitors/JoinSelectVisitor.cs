using System.Linq.Expressions;

namespace Common.Visitors;

public sealed class JoinSelectVisitor : ExpressionVisitor
{
    private ParameterExpression NewParam { get; }

    private ParameterExpression OldParam { get; }

    public JoinSelectVisitor(ParameterExpression newParam, ParameterExpression oldParam)
    {
        NewParam = newParam;
        OldParam = oldParam;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == OldParam
            ? NewParam
            : base.VisitParameter(node);
    }
}
