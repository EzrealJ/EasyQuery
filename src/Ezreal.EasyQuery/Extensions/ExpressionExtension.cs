using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ezreal.EasyQuery.Extensions
{
   public static class ExpressionExtension
    {
        public static MemberExpression GetNullableStructValueAccessExpression(this MemberExpression memberAccessor)
        {
            MemberExpression nullableStructValueAccessor = null;
            if (IsNullableStructMember(memberAccessor))
            {
                nullableStructValueAccessor = Expression.Property(memberAccessor, "Value");
            }
            return nullableStructValueAccessor;
        }

        public static bool IsNullableStructMember(this MemberExpression memberAccessor)
        {
            return memberAccessor.Type.IsGenericType && memberAccessor.Type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        ///  [nullCheckMember!=null] AndAlso attachExpression
        /// </summary>
        /// <param name="nullCheckMember"></param>
        /// <param name="attachExpression"></param>
        /// <returns></returns>
        public static Expression AttachAndAlsoMemberNotNullCheckExpressionOnLeft(this Expression attachExpression, MemberExpression nullCheckMember)
        {
            if (typeof(ValueType).IsAssignableFrom(nullCheckMember.Type) && !IsNullableStructMember(nullCheckMember))
            {
                return attachExpression;
            }

            return Expression.AndAlso(Expression.NotEqual(nullCheckMember, Expression.Constant(null)),
                attachExpression);
        }
    }
}
