using System.Linq.Expressions;

namespace CargoTrack.Business.Helpers
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var leftBody = new ParameterReplacer(left.Parameters[0], parameter).Visit(left.Body);
            var rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(leftBody!, rightBody!), parameter);
        }

        private sealed class ParameterReplacer(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
        {
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == source ? target : base.VisitParameter(node);
            }
        }
    }
}
