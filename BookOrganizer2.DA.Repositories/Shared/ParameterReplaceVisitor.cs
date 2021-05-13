using System.Linq.Expressions;

namespace BookOrganizer2.DA.Repositories.Shared
{
    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        public ParameterExpression Target { get; init; }
        public ParameterExpression Replacement { get; init; }

        protected override Expression VisitParameter(ParameterExpression node) 
            => node == Target ? Replacement : base.VisitParameter(node);
    }
}