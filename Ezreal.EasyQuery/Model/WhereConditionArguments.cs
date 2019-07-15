using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public abstract class WhereConditionArguments : IMultilevelArguments
    {


        protected List<WhereConditionFilterAttribute> _whereParameterAttributeList;
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
        public List<WhereCondition> EqualArguments { get; set; }
        public List<WhereCondition> NotEqualArguments { get; set; }
        public List<WhereCondition> LikeArguments { get; set; }
        public List<WhereCondition> NotLikeArguments { get; set; }
        public List<WhereCondition> LessArguments { get; set; }
        public List<WhereCondition> LessOrEqualArguments { get; set; }
        public List<WhereCondition> MoreArguments { get; set; }
        public List<WhereCondition> MoreOrEqualArguments { get; set; }
        public List<WhereCondition> BetweenArguments { get; set; }
        public List<WhereCondition> NotBetweenArguments { get; set; }
        public List<WhereCondition> InArguments { get; set; }
        public List<WhereCondition> NotInArguments { get; set; }
        public List<WhereCondition> StartWithArguments { get; set; }
        public List<WhereCondition> EndWithArguments { get; set; }

        public virtual void InvokeParameterFilter(IEnumerable<WhereConditionFilterAttribute> whereParameterAttributes)
        {

            _whereParameterAttributeList = whereParameterAttributes.ToList();

        }
        public virtual void InitializeFromJsonObjectString(string jsonObjectString)
        {
            WhereConditionArguments whereParameterArguments = JsonConvert.DeserializeObject<WhereConditionArguments>(jsonObjectString);
            Initialize(whereParameterArguments);
        }

        public virtual void Initialize(WhereConditionArguments whereParameterArguments)
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
        protected virtual List<WhereCondition> InvokeCriteriaFilter(List<WhereCondition> arguments, enumWherePattern wherePattern)
        {
            arguments = arguments?.Where(
                arg => !arg.ColumnName.IsNullOrWhiteSpace()
                &&
                _whereParameterAttributeList.Exists(attr =>
                (attr.ColumnName.IsNullOrNoItems() || (!attr.ColumnName.IsNullOrNoItems() && attr.ColumnName.Contains(arg.ColumnName.Trim())))
                &&
                (attr.WherePattern & wherePattern) == wherePattern)).ToList();
            return arguments;
        }
        protected virtual void InitializeEqualArguments(List<WhereCondition> equalArguments)
        {
            equalArguments = InvokeCriteriaFilter(equalArguments, enumWherePattern.Equal);
            this.EqualArguments = equalArguments;
        }
        protected virtual void InitializeNotEqualArguments(List<WhereCondition> notEqualArguments)
        {
            notEqualArguments = InvokeCriteriaFilter(notEqualArguments, enumWherePattern.Equal);
            this.NotEqualArguments = notEqualArguments;
        }
        protected virtual void InitializeLikeArguments(List<WhereCondition> likeArguments)
        {
            likeArguments = InvokeCriteriaFilter(likeArguments, enumWherePattern.Like);
            this.LikeArguments = likeArguments;
        }
        protected virtual void InitializeNotLikeArguments(List<WhereCondition> notLikeArguments)
        {
            notLikeArguments = InvokeCriteriaFilter(notLikeArguments, enumWherePattern.Like);
            this.NotLikeArguments = notLikeArguments;
        }
        protected virtual void InitializeLessArguments(List<WhereCondition> lessArguments)
        {
            lessArguments = InvokeCriteriaFilter(lessArguments, enumWherePattern.Less);
            this.LessArguments = lessArguments;
        }
        protected virtual void InitializeLessOrEqualArguments(List<WhereCondition> lessOrEqualArguments)
        {
            lessOrEqualArguments = InvokeCriteriaFilter(lessOrEqualArguments, enumWherePattern.LessOrEqual);
            this.LessOrEqualArguments = lessOrEqualArguments;
        }
        protected virtual void InitializeMoreArguments(List<WhereCondition> moreArguments)
        {
            moreArguments = InvokeCriteriaFilter(moreArguments, enumWherePattern.More);
            this.MoreArguments = moreArguments;

        }
        protected virtual void InitializeMoreOrEqualArguments(List<WhereCondition> moreOrEqualArguments)
        {
            moreOrEqualArguments = InvokeCriteriaFilter(moreOrEqualArguments, enumWherePattern.MoreOrEqual);
            this.MoreOrEqualArguments = moreOrEqualArguments;
        }
        protected virtual void InitializeBetweenArguments(List<WhereCondition> betweenArguments)
        {
            betweenArguments = InvokeCriteriaFilter(betweenArguments, enumWherePattern.Between);
            this.BetweenArguments = betweenArguments;
        }
        protected virtual void InitializeNotBetweenArguments(List<WhereCondition> notBetweenArguments)
        {
            notBetweenArguments = InvokeCriteriaFilter(notBetweenArguments, enumWherePattern.Between);
            this.NotBetweenArguments = notBetweenArguments;
        }
        protected virtual void InitializeInArguments(List<WhereCondition> inArguments)
        {
            inArguments = InvokeCriteriaFilter(inArguments, enumWherePattern.In);
            this.InArguments = inArguments;
        }
        protected virtual void InitializeNotInArguments(List<WhereCondition> notInArguments)
        {
            notInArguments = InvokeCriteriaFilter(notInArguments, enumWherePattern.In);
            this.NotInArguments = notInArguments;
        }
        protected virtual void InitializeStartWithArguments(List<WhereCondition> startWithArguments)
        {
            startWithArguments = InvokeCriteriaFilter(startWithArguments, enumWherePattern.StartWith);
            this.StartWithArguments = startWithArguments;
        }
        protected virtual void InitializeEndWithArguments(List<WhereCondition> endWithArguments)
        {
            endWithArguments = InvokeCriteriaFilter(endWithArguments, enumWherePattern.EndWith);
            this.EndWithArguments = endWithArguments;
        }

    }
}
