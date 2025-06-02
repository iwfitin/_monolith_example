using System.Linq.Expressions;
using Common.Visitors;

namespace Common.Static;

public static class ExpressionHelper
{
    public static Expression<Func<TE, TD>> Merge<TE, TD>(params Expression<Func<TE, TD>>[] expressions)
    {
        var param = Expression.Parameter(typeof(TE), "x");
        var bindings = new List<MemberBinding>();

        foreach (var exp in expressions)
        {
            var visitor = new JoinSelectVisitor(param, exp.Parameters[0]);
            var body = (MemberInitExpression)visitor.Visit(exp.Body);
            bindings.AddRange(body.Bindings);
        }

        var mergeBody = Expression.MemberInit(Expression.New(typeof(TD)), bindings);

        return Expression.Lambda<Func<TE, TD>>(mergeBody, param);
    }
}
