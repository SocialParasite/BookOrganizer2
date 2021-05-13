using System;
using System.Linq.Expressions;

namespace BookOrganizer2.DA.Repositories.Shared
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var visitor = new ParameterReplaceVisitor()
            {
                Target = right.Parameters[0],
                Replacement = left.Parameters[0],
            };

            var rewrittenRight = visitor.Visit(right.Body);
            var andExpression = Expression.AndAlso(left.Body, rewrittenRight);
            return Expression.Lambda<Func<T, bool>>(andExpression, left.Parameters);
        }
    }
}