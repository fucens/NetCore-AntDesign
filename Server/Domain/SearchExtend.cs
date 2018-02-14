using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Domain
{

    public class PageCommand
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public ICollection<string> KeywordFileds { get; set; }

        /// <summary>
        /// 等于
        /// </summary>
        public JObject Eq { get; set; }
        /// <summary>
        /// 包含
        /// </summary>
        public JObject Like { get; set; }
        /// <summary>
        /// 属于
        /// </summary>
        public JObject In { get; set; }
        /// <summary>
        /// 范围 例：range.field1.min LessThanOrEqual field1  LessThanOrEqual range.field1.max
        /// </summary>
        public JObject Range { get; set; }

        /// <summary>
        /// 大于
        /// </summary>
        public JObject Gt { get; set; }
        /// <summary>
        /// 大于等于
        /// </summary>
        public JObject GtEq { get; set; }
        /// <summary>
        /// 小于
        /// </summary>
        public JObject Lt { get; set; }
        /// <summary>
        /// 小于等于
        /// </summary>
        public JObject LtEq { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public JObject Sort { get; set; }
    }


    public class PageResult<T>
    {
        public PageResult(List<T> Items, long Total)
        {
            this.Items = Items;
            this.Total = Total;
        }

        public List<T> Items { get; set; }
        public long Total { get; set; }
    }

    public static class PageExtend
    {
        #region Expressions
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            if (expression1 == null) return expression2;

            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.Or(expression1.Body, invokedExpression), expression1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            if (expression1 == null) return expression2;

            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.And(expression1.Body, invokedExpression), expression1.Parameters);
        }

        public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> expression1, bool where, Expression<Func<T, bool>> expression)
        {
            return where ? expression1.And(expression) : null;
        }
        #endregion

        #region ToSql
        public static string ToSql<TEntity>(this IQueryable<TEntity> self)
        {
            var visitor = self.CompileQuery();
            return string.Join("", visitor.Queries.Select(x => x.ToString().TrimEnd().TrimEnd(';') + ";" + Environment.NewLine));
        }
        public static RelationalQueryModelVisitor CompileQuery<TEntity>(this IQueryable<TEntity> self)
        {
            var q = self as EntityQueryable<TEntity>;
            if (q == null)
            {
                return null;
            }
            var fields = typeof(Microsoft.EntityFrameworkCore.Storage.Database).GetTypeInfo().DeclaredFields;

            var queryCompiler = (QueryCompiler)ReflectionCommon.QueryCompilerOfEntityQueryProvider.GetValue(self.Provider);
            var database = (Microsoft.EntityFrameworkCore.Storage.Database)ReflectionCommon.DatabaseOfQueryCompiler.GetValue(queryCompiler);
            var dependencies = (DatabaseDependencies)ReflectionCommon.DependenciesOfDatabase.GetValue(database);
            var factory = dependencies.QueryCompilationContextFactory;
            var nodeTypeProvider = (INodeTypeProvider)ReflectionCommon.NodeTypeProvider.GetValue(queryCompiler);
            var parser = (QueryParser)ReflectionCommon.CreateQueryParserMethod.Invoke(queryCompiler, new object[] { nodeTypeProvider });
            var queryModel = parser.GetParsedQuery(self.Expression);
            var modelVisitor = (RelationalQueryModelVisitor)database.CreateVisitor(factory, queryModel);
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            return modelVisitor;
        }
        public static class ReflectionCommon
        {
            public static readonly FieldInfo QueryCompilerOfEntityQueryProvider = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");
            public static readonly PropertyInfo DatabaseOfQueryCompiler = typeof(QueryCompiler).GetTypeInfo().DeclaredProperties.First(x => x.Name == "Database");
            public static readonly PropertyInfo DependenciesOfDatabase = typeof(Microsoft.EntityFrameworkCore.Storage.Database).GetTypeInfo().DeclaredProperties.First(x => x.Name == "Dependencies");
            public static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();
            public static readonly MethodInfo CreateQueryParserMethod = QueryCompilerTypeInfo.DeclaredMethods.First(x => x.Name == "CreateQueryParser");
            public static readonly PropertyInfo NodeTypeProvider = QueryCompilerTypeInfo.DeclaredProperties.Single(x => x.Name == "NodeTypeProvider");
            public static readonly PropertyInfo QueriesOfRelationalQueryModelVisitor = typeof(RelationalQueryModelVisitor).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Queries");
        }
        public static EntityQueryModelVisitor CreateVisitor(this Microsoft.EntityFrameworkCore.Storage.Database self, IQueryCompilationContextFactory factory, QueryModel qm)
        {
            return factory.Create(async: false).CreateQueryModelVisitor();
        }
        #endregion

        #region Query
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool where, Expression<Func<T, bool>> expression)
        {
            return where ? query.Where(expression) : query;
        }
        #endregion

        #region Page
        public static PageResult<TResult> ToPage<TEntity, TResult>(this IQueryable<TEntity> query, PageCommand Cmd)
               where TEntity : class
        {
            query = query.AutoWhere(Cmd);
            query = query.AutoSort(Cmd);

            if (!(Cmd.PageIndex == 0 || Cmd.PageSize == 0))
            {
                query = query.Skip((Cmd.PageIndex - 1) * Cmd.PageSize).Take(Cmd.PageSize);
            }

            var sql = query.AutoSelect<TEntity, TResult>().ToSql();


            long total = query.Count();
            var list = query.AutoSelect<TEntity, TResult>().ToList();
            return new PageResult<TResult>(list, total);
        }

        public static IQueryable<TEntity> AutoWhere<TEntity>(this IQueryable<TEntity> query, PageCommand Cmd)
          where TEntity : class
        {
            Expression<Func<TEntity, bool>> where = null;
            var param = Expression.Parameter(typeof(TEntity), "x");

            if (!string.IsNullOrEmpty(Cmd.Keyword))
            {
                Expression<Func<TEntity, bool>> searchWhere = null;
                var words = Cmd.Keyword.Split(' ').Where(x => !string.IsNullOrEmpty(x));
                if (Cmd.KeywordFileds == null)
                {
                    var type = typeof(TEntity);
                    Expression keyWhere = null;
                    foreach (var prop in type.GetProperties())
                    {
                        var left = Expression.PropertyOrField(param, prop.Name);

                        foreach (var word in words)
                        {

                            if (left.Type != typeof(string))
                            {
                                var converter = TypeDescriptor.GetConverter(left.Type);
                                if (converter != null)
                                {
                                    try
                                    {
                                        dynamic val = converter.ConvertFromString(word);
                                        var right = Expression.Constant(val, left.Type);
                                        var exp = Expression.Equal(left, right);

                                        if (keyWhere == null)
                                            keyWhere = exp;
                                        else
                                            keyWhere = Expression.Or(keyWhere, exp);
                                    }
                                    catch (Exception e) { var msg = e.Message; }
                                }
                            }
                            else
                            {
                                var right = Expression.Constant(word, left.Type);
                                var method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                                var exp = Expression.Call(left, method, right);

                                if (keyWhere == null)
                                    keyWhere = exp;
                                else
                                    keyWhere = Expression.Or(keyWhere, exp);
                            }
                        }

                        if (keyWhere != null)
                        {
                            var lambda1 = Expression.Lambda<Func<TEntity, bool>>(keyWhere, param);
                            searchWhere = searchWhere.And(lambda1);
                        }
                    }
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(keyWhere, param);

                    if (searchWhere != null) where = where.And(searchWhere);

                }

                //if (searchWhere != null) where = where.And(searchWhere);
            }

            if (Cmd.Eq != null)
                foreach (var prop in Cmd.Eq.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);
                    var val = Convert.ChangeType(prop.Value.ToString(), left.Type);
                    var right = Expression.Constant(val, left.Type);
                    var exp = Expression.Equal(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.Like != null)
                foreach (var prop in Cmd.Like.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);
                    var val = Convert.ChangeType(prop.Value.ToString(), left.Type);
                    var right = Expression.Constant(val, left.Type);
                    var method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                    var exp = Expression.Call(left, method, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.In != null)
                foreach (var prop in Cmd.In.Properties())
                {
                    var right = Expression.PropertyOrField(param, prop.Name);

                    Type coll = typeof(List<>);
                    Type collOfT = coll.MakeGenericType(right.Type);

                    dynamic collection = Activator.CreateInstance(collOfT);

                    foreach (var item in prop.Value)
                    {
                        var converter = TypeDescriptor.GetConverter(right.Type);
                        if (converter != null)
                        {
                            dynamic ov = converter.ConvertFromString(item.ToString());
                            collection.Add(ov);
                        }
                    }
                    Expression left = Expression.Constant(collection, collOfT);

                    var method = left.Type.GetMethod("Contains", new Type[] { right.Type });
                    var exp = Expression.Call(left, method, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.Range != null)
                foreach (var prop in Cmd.Range.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);

                    var val1 = Convert.ChangeType(prop.Value["min"].ToString(), left.Type);
                    var right1 = Expression.Constant(val1, left.Type);
                    var exp1 = Expression.GreaterThanOrEqual(left, right1);

                    var val2 = Convert.ChangeType(prop.Value["max"].ToString(), left.Type);
                    var right2 = Expression.Constant(val2, left.Type);
                    var exp2 = Expression.LessThanOrEqual(left, right2);

                    var exp = Expression.And(exp1, exp2);

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.Gt != null)
                foreach (var prop in Cmd.Gt.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);
                    var val = Convert.ChangeType(prop.Value.ToString(), left.Type);
                    var right = Expression.Constant(val, left.Type);
                    var exp = Expression.GreaterThan(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.GtEq != null)
                foreach (var prop in Cmd.GtEq.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);
                    var val = Convert.ChangeType(prop.Value.ToString(), left.Type);
                    var right = Expression.Constant(val, left.Type);
                    var exp = Expression.GreaterThanOrEqual(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.Lt != null)
                foreach (var prop in Cmd.Lt.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);
                    var val = Convert.ChangeType(prop.Value.ToString(), left.Type);
                    var right = Expression.Constant(val, left.Type);
                    var exp = Expression.LessThan(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            if (Cmd.LtEq != null)
                foreach (var prop in Cmd.LtEq.Properties())
                {
                    var left = Expression.PropertyOrField(param, prop.Name);
                    var val = Convert.ChangeType(prop.Value.ToString(), left.Type);
                    var right = Expression.Constant(val, left.Type);
                    var exp = Expression.LessThanOrEqual(left, right);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                    where = where.And(lambda);
                }

            query = query.Where(where);
            return query;
        }
        public static IQueryable<TEntity> AutoSort<TEntity>(this IQueryable<TEntity> query, PageCommand Cmd)
            where TEntity : class
        {

            var param = Expression.Parameter(typeof(TEntity), "x");
            if (Cmd.Sort == null)
            {
                return query;
            }
            var propertys = Cmd.Sort.Properties().ToList();
            for (int i = 0; i < propertys.Count(); i++)
            {
                var prop = propertys[i];

                Expression conversion = Expression.Convert(Expression.PropertyOrField(param, prop.Name), typeof(object));
                var lambda = Expression.Lambda<Func<TEntity, object>>(conversion, param);

                var val = prop.Value.ToString();
                if (val == "asc")
                {
                    if (i == 0)
                        query = query.OrderBy(lambda);
                    else
                        query = ((IOrderedQueryable<TEntity>)query).ThenBy(lambda);
                }
                else if (val == "desc")
                {
                    if (i == 0)
                        query = query.OrderByDescending(lambda);
                    else
                        query = ((IOrderedQueryable<TEntity>)query).ThenByDescending(lambda);
                }
            }
            return query;
        }
        public static IQueryable<TResult> AutoSelect<TEntity, TResult>(this IQueryable<TEntity> query)
            where TEntity : class
        {
            var param = Expression.Parameter(typeof(TEntity), "x");

            var newExp = Expression.New(typeof(TResult));

            List<MemberAssignment> asgs = new List<MemberAssignment>();
            foreach (var prop in typeof(TResult).GetProperties())
            {
                var mem = Expression.Property(param, prop.Name);
                var bind = Expression.Bind(prop, mem);
                asgs.Add(bind);
            }
            var init = Expression.MemberInit(newExp, asgs);

            var select = Expression.Lambda<Func<TEntity, TResult>>(init, param);
            return query.Select(select);
        }
        #endregion
    }
}
