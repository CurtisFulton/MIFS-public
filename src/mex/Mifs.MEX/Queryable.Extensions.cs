using Microsoft.EntityFrameworkCore;
using Mifs.MEX.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Mifs.MEX
{
    public static class Queryable_Extensions
    {
        public static IQueryable<PurchaseOrder> FilterApprovalStatus(this IQueryable<PurchaseOrder> source, IMEXDbContext dbContext, params string[] approvalStatusNames)
            => source.Where(po => approvalStatusNames.Contains(dbContext.RecordApprovals.OrderByDescending(x => x.RecordApprovalID)
                                                                                        .FirstOrDefault(recordApproval => recordApproval.EntityID == po.PurchaseOrderID && recordApproval.EntityName == "Purchase Order")
                                                                                        .ApprovalStatusName));

        public static IQueryable<TSource> FilterProcessed<TSource>(this IQueryable<TSource> source, string integrationName, IMEXDbContext dbContext)
            => source.FilterProcessed(integrationName, null, dbContext);

        public static IQueryable<TSource> FilterProcessed<TSource>(this IQueryable<TSource> source, string integrationName, Expression<Func<PurchaseOrder, DateTime?>> syncField, IMEXDbContext dbContext)
            => source.FilterProcessed(integrationName, (LambdaExpression)syncField, dbContext);

        public static IQueryable<TSource> FilterProcessed<TSource>(this IQueryable<TSource> source, string integrationName, Expression<Func<PurchaseOrder, DateTime>> syncField, IMEXDbContext dbContext)
            => source.FilterProcessed(integrationName, (LambdaExpression)syncField, dbContext);

        private static IQueryable<TSource> FilterProcessed<TSource>(this IQueryable<TSource> source, string integrationName, LambdaExpression xSyncFieldLambda, IMEXDbContext dbContext)
        {
            var entityType = dbContext.Model.FindEntityType(typeof(TSource));
            var primaryKeyProperty = entityType.FindPrimaryKey().Properties.Single();

            var xSourceParam = Expression.Parameter(typeof(TSource), "src");
            var xPrimaryKey = Expression.Property(xSourceParam, primaryKeyProperty.PropertyInfo);

            var xLogParam = Expression.Parameter(typeof(MifsProcessedEntityLog), "log");
            xSyncFieldLambda = ParameterReplacer.Replace(xSourceParam, xSyncFieldLambda);

            var xSyncFieldProperty = xSyncFieldLambda?.Body as MemberExpression;
            var xAnyPredicate = GetMifsLogAnyPredicateExpression<TSource>(xLogParam, xPrimaryKey, integrationName, xSyncFieldProperty);

            var xAnyCall = CallAnyOnMifsProcessedEntityLogExpression(dbContext, xAnyPredicate);
            var xNotAny = Expression.Not(xAnyCall);

            // Creates the lamba src => !dbContext.MifsProcessedEntityLog.Any(log => log.EntityID == src.<PrimaryKey> && log.EntityName == <SrcName>)
            var xWhereLambda = Expression.Lambda<Func<TSource, bool>>(xNotAny, xSourceParam);
            return source.Where(xWhereLambda);
        }

        private static Expression<Func<MifsProcessedEntityLog, bool>> GetMifsLogAnyPredicateExpression<TSource>(ParameterExpression xLogParam, 
                                                                                                                MemberExpression xSourcePrimaryKeyProperty, 
                                                                                                                string integrationName,
                                                                                                                MemberExpression xSyncFieldProperty)
        {
            var xLogIdProperty = Expression.Property(xLogParam, nameof(MifsProcessedEntityLog.EntityId));
            var xLogNameProperty = Expression.Property(xLogParam, nameof(MifsProcessedEntityLog.EntityName));
            var xLogIntegrationNameProperty = Expression.Property(xLogParam, nameof(MifsProcessedEntityLog.IntegrationName));

            var xEntityIdEqual = Expression.Equal(xLogIdProperty, xSourcePrimaryKeyProperty);

            var xSourceEntityName = Expression.Constant(typeof(TSource).Name);
            var xEntityNameEqual = Expression.Equal(xLogNameProperty, xSourceEntityName);

            var xIntegrationName = Expression.Constant(integrationName);
            var xIntegrationNameEqual = Expression.Equal(xLogIntegrationNameProperty, xIntegrationName);

            var xAnyPredicateBody = Expression.AndAlso(xEntityIdEqual, xEntityNameEqual);
            xAnyPredicateBody = Expression.AndAlso(xAnyPredicateBody, xIntegrationNameEqual);

            if (xSyncFieldProperty is not null)
            {
                var xLogProcessedDateTimeProperty = Expression.Property(xLogParam, nameof(MifsProcessedEntityLog.ProcessedDateTime));
                var xSyncFieldLessThanProcessedDatetime = Expression.LessThanOrEqual(xSyncFieldProperty, xLogProcessedDateTimeProperty);

                xAnyPredicateBody = Expression.AndAlso(xAnyPredicateBody, xSyncFieldLessThanProcessedDatetime);
            }

            return Expression.Lambda<Func<MifsProcessedEntityLog, bool>>(xAnyPredicateBody, xLogParam);
        }

        private static MethodCallExpression CallAnyOnMifsProcessedEntityLogExpression(IMEXDbContext dbContext, Expression<Func<MifsProcessedEntityLog, bool>> xAnyPredicate)
        {
            var xProcessedEntitiesLogProperty = Expression.Property(Expression.Constant(dbContext), nameof(IMEXDbContext.MifsProcessedEntityLogs));
            var xAnyCall = Expression.Call(typeof(Enumerable),
                                           nameof(Enumerable.Any),
                                           new[] { typeof(MifsProcessedEntityLog) },
                                           xProcessedEntitiesLogProperty,
                                           xAnyPredicate);

            return xAnyCall;
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _param;

            private ParameterReplacer(ParameterExpression param)
            {
                _param = param;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node.Type == _param.Type ? // if types match on both of ends
                  base.VisitParameter(_param) : // replace
                  node; // ignore
            }

            public static T Replace<T>(ParameterExpression param, T exp) where T : Expression
            {
                return (T)new ParameterReplacer(param).Visit(exp);
            }
        }
    }
}
