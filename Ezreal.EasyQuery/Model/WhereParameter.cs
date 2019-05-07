using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ezreal.EasyQuery.Model
{


    public class WhereParameter
    {
        /// <summary>
        /// 搜索列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 搜索值
        /// </summary>
        public object ColumnValue { get; set; }
        /// <summary>
        /// 连接方式
        /// </summary>
        public enumJoinPattern Pattern { get; set; }
        /// <summary>
        /// 连接方式
        /// </summary>
        [Flags]
        public enum enumJoinPattern
        {
            And = 1,
            Or = 2,
            Both = 3
        }



    }


    public class WhereParameterArguments
    {
        protected List<WhereParameterAttribute> _whereParameterAttributeList;
        [Flags]
        public enum enumWherePattern
        {
            Equal = 1,
            NotEqual = 2,
            Like = 4,
            NotLike = 8,
            Less = 16,
            LessOrEqual = 32,
            More = 64,
            MoreOrEqual = 128,
            Between = 256,
            NotBetween = 512,
            In = 1024,
            NotIn = 2048,
            StartWith = 4096,
            EndWith = 8192,
            All = 16383,

        }
        public List<WhereParameter> EqualArguments { get; set; }
        public List<WhereParameter> NotEqualArguments { get; set; }
        public List<WhereParameter> LikeArguments { get; set; }
        public List<WhereParameter> NotLikeArguments { get; set; }
        public List<WhereParameter> LessArguments { get; set; }
        public List<WhereParameter> LessOrEqualArguments { get; set; }
        public List<WhereParameter> MoreArguments { get; set; }
        public List<WhereParameter> MoreOrEqualArguments { get; set; }
        public List<WhereParameter> BetweenArguments { get; set; }
        public List<WhereParameter> NotBetweenArguments { get; set; }
        public List<WhereParameter> InArguments { get; set; }
        public List<WhereParameter> NotInArguments { get; set; }
        public List<WhereParameter> StartWithArguments { get; set; }
        public List<WhereParameter> EndWithArguments { get; set; }

        public virtual void InvokeParameterFilter(IEnumerable<WhereParameterAttribute> whereParameterAttributes)
        {
            if (_whereParameterAttributeList.IsNullOrNoItems())
            {
                _whereParameterAttributeList = whereParameterAttributes.ToList();
            }
        }
        public virtual void InitializeFromJsonObjectString(string jsonObjectString)
        {
            WhereParameterArguments whereParameterArguments = JsonConvert.DeserializeObject<WhereParameterArguments>(jsonObjectString);
            Initialize(whereParameterArguments);
        }

        protected virtual void Initialize(WhereParameterArguments whereParameterArguments)
        {
            InitializeEqualArguments(whereParameterArguments.EqualArguments);
            InitializeNotEqualArguments(whereParameterArguments.NotEqualArguments);
            InitializeLikeArguments(whereParameterArguments.LikeArguments);
            InitializeNotLikeArguments(whereParameterArguments.NotLikeArguments);
            InitializeLessArguments(whereParameterArguments.LessArguments);
            InitializeLessOrEqualArguments(whereParameterArguments.LessOrEqualArguments);
            InitializeMoreArguments(whereParameterArguments.MoreArguments);
            InitializeMoreOrEqualArguments(whereParameterArguments.MoreOrEqualArguments);
            InitializeBetweenArguments(whereParameterArguments.BetweenArguments);
            InitializeNotBetweenArguments(whereParameterArguments.NotBetweenArguments);
            InitializeInArguments(whereParameterArguments.InArguments);
            InitializeNotInArguments(whereParameterArguments.NotInArguments);
            InitializeStartWithArguments(whereParameterArguments.StartWithArguments);
            InitializeEndWithArguments(whereParameterArguments.EndWithArguments);
        }

        public virtual string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
        protected virtual List<WhereParameter> InvokeCriteriaFilter(List<WhereParameter> arguments, enumWherePattern wherePattern)
        {
            arguments = arguments?.Where(
                arg => !arg.ColumnName.IsNullOrWhiteSpace()
                &&
                _whereParameterAttributeList.Exists(attr =>
                attr.ColumnName.Contains(arg.ColumnName.Trim()) &&
                (attr.WherePattern & wherePattern) == wherePattern)).ToList();
            return arguments;
        }
        protected virtual void InitializeEqualArguments(List<WhereParameter> equalArguments)
        {
            equalArguments = InvokeCriteriaFilter(equalArguments, enumWherePattern.Equal);
            this.EqualArguments = equalArguments;
        }
        protected virtual void InitializeNotEqualArguments(List<WhereParameter> notEqualArguments)
        {
            notEqualArguments = InvokeCriteriaFilter(notEqualArguments, enumWherePattern.Equal);
            this.NotEqualArguments = notEqualArguments;
        }
        protected virtual void InitializeLikeArguments(List<WhereParameter> likeArguments)
        {
            likeArguments = InvokeCriteriaFilter(likeArguments, enumWherePattern.Like);
            this.LikeArguments = likeArguments;
        }
        protected virtual void InitializeNotLikeArguments(List<WhereParameter> notLikeArguments)
        {
            notLikeArguments = InvokeCriteriaFilter(notLikeArguments, enumWherePattern.Like);
            this.NotLikeArguments = notLikeArguments;
        }
        protected virtual void InitializeLessArguments(List<WhereParameter> lessArguments)
        {
            lessArguments = InvokeCriteriaFilter(lessArguments, enumWherePattern.Less);
            this.LessArguments = lessArguments;
        }
        protected virtual void InitializeLessOrEqualArguments(List<WhereParameter> lessOrEqualArguments)
        {
            lessOrEqualArguments = InvokeCriteriaFilter(lessOrEqualArguments, enumWherePattern.LessOrEqual);
            this.LessOrEqualArguments = lessOrEqualArguments;
        }
        protected virtual void InitializeMoreArguments(List<WhereParameter> moreArguments)
        {
            moreArguments = InvokeCriteriaFilter(moreArguments, enumWherePattern.More);
            this.MoreArguments = moreArguments;

        }
        protected virtual void InitializeMoreOrEqualArguments(List<WhereParameter> moreOrEqualArguments)
        {
            moreOrEqualArguments = InvokeCriteriaFilter(moreOrEqualArguments, enumWherePattern.MoreOrEqual);
            this.MoreOrEqualArguments = moreOrEqualArguments;
        }
        protected virtual void InitializeBetweenArguments(List<WhereParameter> betweenArguments)
        {
            betweenArguments = InvokeCriteriaFilter(betweenArguments, enumWherePattern.Between);
            this.BetweenArguments = betweenArguments;
        }
        protected virtual void InitializeNotBetweenArguments(List<WhereParameter> notBetweenArguments)
        {
            notBetweenArguments = InvokeCriteriaFilter(notBetweenArguments, enumWherePattern.Between);
            this.NotBetweenArguments = notBetweenArguments;
        }
        protected virtual void InitializeInArguments(List<WhereParameter> inArguments)
        {
            inArguments = InvokeCriteriaFilter(inArguments, enumWherePattern.In);
            this.InArguments = inArguments;
        }
        protected virtual void InitializeNotInArguments(List<WhereParameter> notInArguments)
        {
            notInArguments = InvokeCriteriaFilter(notInArguments, enumWherePattern.In);
            this.NotInArguments = notInArguments;
        }
        protected virtual void InitializeStartWithArguments(List<WhereParameter> startWithArguments)
        {
            startWithArguments = InvokeCriteriaFilter(startWithArguments, enumWherePattern.StartWith);
            this.StartWithArguments = startWithArguments;
        }
        protected virtual void InitializeEndWithArguments(List<WhereParameter> endWithArguments)
        {
            endWithArguments = InvokeCriteriaFilter(endWithArguments, enumWherePattern.EndWith);
            this.EndWithArguments = endWithArguments;
        }

    }
    public class WhereParameterArguments<TSource> : WhereParameterArguments
    {
        List<System.Reflection.PropertyInfo> _sourcePropertyInfos = typeof(TSource).GetProperties().ToList();
        readonly MethodInfo _objectToStringMethod = typeof(object).GetMethod(nameof(object.ToString), new Type[] { });
        readonly MethodInfo _stringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
        public override void InvokeParameterFilter(IEnumerable<WhereParameterAttribute> whereParameterAttributes)
        {
            if (_whereParameterAttributeList.IsNullOrNoItems())
            {
                _whereParameterAttributeList = _whereParameterAttributeList ?? new List<WhereParameterAttribute>();
                whereParameterAttributes.ToList().ForEach(wpa =>
                {

                    wpa.ColumnName = wpa.ColumnName.Where(cName => _sourcePropertyInfos.Exists(p => p.Name.Equals(cName)));
                    _whereParameterAttributeList.Add(wpa);
                });
            }
        }
        protected override List<WhereParameter> InvokeCriteriaFilter(List<WhereParameter> arguments, enumWherePattern wherePattern)
        {
            arguments = base.InvokeCriteriaFilter(arguments, wherePattern);
            arguments?.ForEach(arg =>
            {
                List<WhereParameterAttribute> whereParameterAttributeList = _whereParameterAttributeList.Where(
                    attr =>
                    attr.ColumnName.Contains(arg.ColumnName)
                    && (attr.JoinPattern & arg.Pattern) == arg.Pattern
                    && (attr.WherePattern & wherePattern) == wherePattern)?.ToList();
                Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(arg.ColumnName))?.PropertyType;
                if (whereParameterAttributeList.IsNullOrNoItems()) return;
                if (targetType.IsNull()) return;
                targetType = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) ? targetType.GenericTypeArguments[0] : targetType;
                if ((wherePattern & (enumWherePattern.Between | enumWherePattern.NotBetween | enumWherePattern.In | enumWherePattern.NotIn)) == wherePattern)
                {
                    object[] targetArray = Array.ConvertAll(((string)arg.ColumnValue)?.Trim().Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries), t => Convert.ChangeType(t, targetType));

                    if (targetArray.IsNullOrNoItems()
                    || ((wherePattern & (enumWherePattern.In | enumWherePattern.NotIn)) == wherePattern && targetArray.Length != 2))
                    {
                        arguments.Remove(arg);
                    }
                    arg.ColumnValue = targetArray;
                }
                else if ((wherePattern & enumWherePattern.Like) == enumWherePattern.Like)
                {
                    //No conversion required
                }
                else
                {
                    arg.ColumnValue = Convert.ChangeType(arg.ColumnValue, targetType);
                }
            });
            return arguments;
        }
        public Expression<Func<TSource, bool>> GetWhereExpression() => this.GetWhereExpression<TSource>();
        public Expression<Func<TDBOSource, bool>> GetWhereExpression<TDBOSource>()
        {
            Expression where = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
            ParameterExpression parameter = Expression.Parameter(typeof(TDBOSource), "t");
            try
            {
                Action<WhereParameter.enumJoinPattern, Expression, Expression> expressionComposer = (pattern, rsultExpression, currentExpression) =>
                    {
                        if ((pattern & WhereParameter.enumJoinPattern.And) == WhereParameter.enumJoinPattern.And)
                        {
                            where = where.IsNull() ? currentExpression : Expression.AndAlso(where, currentExpression);
                        }
                        else
                        {
                            where = where.IsNull() ? currentExpression : Expression.OrElse(where, currentExpression);
                        }
                    };
                this.EqualArguments?.ForEach(ea =>
                {

                    MemberExpression member = Expression.PropertyOrField(parameter, ea.ColumnName);
                    ConstantExpression constant = Expression.Constant(ea.ColumnValue);
                    Expression currentExpression = Expression.Equal(member, constant);
                    expressionComposer(ea.Pattern, where, currentExpression);
                });
                this.NotEqualArguments?.ForEach(nea =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nea.ColumnName);
                    ConstantExpression constant = Expression.Constant(nea.ColumnValue);
                    Expression currentExpression = Expression.NotEqual(member, constant);
                    expressionComposer(nea.Pattern, where, currentExpression);
                });


                this.LikeArguments?.ForEach(la =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, la.ColumnName);
                    ConstantExpression constant = Expression.Constant(la.ColumnValue.ToString());
                    Expression currentExpression = Expression.Call(Expression.Call(member, _objectToStringMethod), _stringContainsMethod, constant);
                    expressionComposer(la.Pattern, where, currentExpression);
                });
                this.NotLikeArguments?.ForEach(nla =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nla.ColumnName);
                    ConstantExpression constant = Expression.Constant(nla.ColumnValue.ToString());
                    Expression currentExpression = Expression.Not(Expression.Call(Expression.Call(member, _objectToStringMethod), _stringContainsMethod, constant));
                    expressionComposer(nla.Pattern, where, currentExpression);
                });
                this.LessArguments?.ForEach(la =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, la.ColumnName);
                    ConstantExpression constant = Expression.Constant(la.ColumnValue);
                    Expression currentExpression = Expression.LessThan(member, constant);
                    expressionComposer(la.Pattern, where, currentExpression);
                });
                this.LessOrEqualArguments?.ForEach(loea =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, loea.ColumnName);
                    ConstantExpression constant = Expression.Constant(loea.ColumnValue);
                    Expression currentExpression = Expression.LessThanOrEqual(member, constant);
                    expressionComposer(loea.Pattern, where, currentExpression);
                });
                this.MoreArguments?.ForEach(ma =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, ma.ColumnName);
                    ConstantExpression constant = Expression.Constant(ma.ColumnValue);
                    Expression currentExpression = Expression.GreaterThan(member, constant);
                    expressionComposer(ma.Pattern, where, currentExpression);
                });
                this.MoreOrEqualArguments?.ForEach(moea =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, moea.ColumnName);
                    ConstantExpression constant = Expression.Constant(moea.ColumnValue);
                    Expression currentExpression = Expression.GreaterThanOrEqual(member, constant);
                    expressionComposer(moea.Pattern, where, currentExpression);
                });
                this.BetweenArguments?.ForEach(ba =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, ba.ColumnName);
                    Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(ba.ColumnName))?.PropertyType;
                    object[] valueArray = ba.ColumnValue as object[];
                    ConstantExpression constantStart = Expression.Constant(valueArray[0]);
                    ConstantExpression constantEnd = Expression.Constant(valueArray[1]);
                    Expression currentExpression = Expression.And(Expression.GreaterThanOrEqual(member, constantStart), Expression.LessThanOrEqual(member, constantEnd));
                    expressionComposer(ba.Pattern, where, currentExpression);
                });
                this.NotBetweenArguments?.ForEach(nba =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nba.ColumnName);
                    Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(nba.ColumnName))?.PropertyType;
                    object[] valueArray = nba.ColumnValue as object[];
                    ConstantExpression constantStart = Expression.Constant(valueArray[0]);
                    ConstantExpression constantEnd = Expression.Constant(valueArray[1]);
                    Expression currentExpression = Expression.And(Expression.LessThan(member, constantStart), Expression.GreaterThan(member, constantEnd));
                    expressionComposer(nba.Pattern, where, currentExpression);
                });
                this.InArguments?.ForEach(ia =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, ia.ColumnName);
                    ConstantExpression constant = Expression.Constant(ia.ColumnValue);
                    Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(ia.ColumnName))?.PropertyType;
                    MethodInfo containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(targetType);
                    Expression currentExpression = Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(targetType)), member);
                    expressionComposer(ia.Pattern, where, currentExpression);
                });
                this.NotInArguments?.ForEach(nia =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nia.ColumnName);
                    ConstantExpression constant = Expression.Constant(nia.ColumnValue);
                    Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(nia.ColumnName))?.PropertyType;
                    MethodInfo containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(targetType);
                    Expression currentExpression = Expression.Not(Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(targetType)), member));
                    expressionComposer(nia.Pattern, where, currentExpression);
                });
                this.StartWithArguments?.ForEach(swa =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, swa.ColumnName);
                    ConstantExpression constant = Expression.Constant(swa.ColumnValue.ToString());
                    var startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                    Expression currentExpression = Expression.Call(Expression.Call(member, _objectToStringMethod), startsWithMethod, constant);
                    expressionComposer(swa.Pattern, where, currentExpression);
                });
                this.EndWithArguments?.ForEach(ewa =>
                {

                    MemberExpression member = Expression.PropertyOrField(parameter, ewa.ColumnName);
                    ConstantExpression constant = Expression.Constant(ewa.ColumnValue.ToString());
                    var endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                    Expression currentExpression = Expression.Call(Expression.Call(member, _objectToStringMethod), endsWithMethod, constant);
                    expressionComposer(ewa.Pattern, where, currentExpression);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            Expression<Func<TDBOSource, bool>> expression = Expression.Lambda<Func<TDBOSource, bool>>(where, new[] { parameter });
            return expression;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class WhereParameterAttribute : Attribute
    {
        public WhereParameterAttribute(
            WhereParameterArguments.enumWherePattern wherePattern = WhereParameterArguments.enumWherePattern.Equal,
            WhereParameter.enumJoinPattern joinPattern = WhereParameter.enumJoinPattern.And, params string[] columnName)
        {
            ColumnName = columnName;
            this.WherePattern = wherePattern;
            this.JoinPattern = joinPattern;
        }

        /// <summary>
        /// 搜索列
        /// </summary>
        public IEnumerable<string> ColumnName { get; set; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public WhereParameterArguments.enumWherePattern WherePattern { get; private set; }


        /// <summary>
        /// 连接方式
        /// </summary>
        public WhereParameter.enumJoinPattern JoinPattern { get; private set; }

    }
}
