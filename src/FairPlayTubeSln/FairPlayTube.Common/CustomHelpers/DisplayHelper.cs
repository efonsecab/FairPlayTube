using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace FairPlayTube.Common.CustomHelpers
{
    public static class DisplayHelper
    {
        public static string DisplayFor<TModel>(Expression<Func<TModel, object>> expression)
        {
            MemberExpression memberExpression = null;
            switch (expression.Body.NodeType)
            {
                //Check https://stackoverflow.com/questions/3049825/given-a-member-access-lambda-expression-convert-it-to-a-specific-string-represe
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expression.Body as UnaryExpression;
                    memberExpression = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    memberExpression = expression.Body as MemberExpression;
                    break;
            }
            PropertyInfo propertyBeingAccessed = (PropertyInfo)memberExpression.Member;
            DisplayAttribute displayAttribute = propertyBeingAccessed.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute is null)
                throw new Exception($"Property '{propertyBeingAccessed.Name}' of type " +
                    $"'{expression.Parameters.First().Type.FullName}' does not have a {nameof(DisplayAttribute)}");
            var displayName = displayAttribute.GetName();
            return displayName;
        }
    }
}
