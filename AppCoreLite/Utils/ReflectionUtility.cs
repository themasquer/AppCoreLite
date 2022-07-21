using AppCoreLite.Attributes;
using AppCoreLite.Enums;
using AppCoreLite.Extensions;
using AppCoreLite.Models;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace AppCoreLite.Utils
{
    public class ReflectionUtility
    {
        public PropertyInfo? GetProperty<T>(T instance, string propertyName) where T : class, new()
        {
            return instance.GetType().GetProperty(propertyName);
        }

        public PropertyInfo? GetProperty<T>(string propertyName) where T : class, new()
        {
            return typeof(T).GetProperty(propertyName);
        }

        public List<Property>? GetProperties<T>(TagAttributes tagAttribute = TagAttributes.None) where T : class
        {
            List<Property>? propertyModels = null;
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<Attribute> attributes;
            string displayName;
            bool attributeFound;
            if (properties != null && properties.Length > 0)
            {
                propertyModels = new List<Property>();
                if (tagAttribute == TagAttributes.None)
                {
                    foreach (var property in properties)
                    {
                        displayName = "";
                        attributes = property.GetCustomAttributes().ToList();
                        if (attributes != null && attributes.Count > 0)
                        {
                            foreach (var attribute in attributes)
                            {
                                if (attribute.GetType() == typeof(DisplayNameAttribute))
                                {
                                    displayName = ((DisplayNameAttribute)attribute).DisplayName;
                                    break;
                                }
                            }
                        }
                        propertyModels.Add(new Property(property.Name, displayName));
                    }
                }
                else if (tagAttribute == TagAttributes.Order)
                {
                    foreach (var property in properties)
                    {
                        displayName = "";
                        attributeFound = false;
                        attributes = property.GetCustomAttributes().ToList();
                        if (attributes != null && attributes.Count > 0)
                        {
                            foreach (var attribute in attributes)
                            {
                                if (attribute.GetType() == typeof(OrderTagAttribute))
                                    attributeFound = true;
                                if (attribute.GetType() == typeof(DisplayNameAttribute))
                                    displayName = ((DisplayNameAttribute)attribute).DisplayName;
                            }
                            if (attributeFound)
                                propertyModels.Add(new Property(property.Name, displayName));
                        }
                    }
                }
                else if (tagAttribute == TagAttributes.StringFilter)
                {
                    foreach (var property in properties)
                    {
                        displayName = "";
                        attributeFound = false;
                        attributes = property.GetCustomAttributes().ToList();
                        if (attributes != null && attributes.Count > 0)
                        {
                            foreach (var attribute in attributes)
                            {
                                if (attribute.GetType() == typeof(StringFilterTagAttribute))
                                    attributeFound = true;
                                if (attribute.GetType() == typeof(DisplayNameAttribute))
                                    displayName = ((DisplayNameAttribute)attribute).DisplayName;
                            }
                            if (attributeFound)
                                propertyModels.Add(new Property(property.Name, displayName));
                        }
                    }
                }
            }
            return propertyModels;
        }

        public void TrimStringProperties<T>(T instance) where T : class, new()
        {
            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)).ToList();
            string? value;
            if (properties != null && properties.Count > 0)
            {
                foreach (var property in properties)
                {
                    value = (string?)property.GetValue(instance);
                    property.SetValue(instance, value?.Trim());
                }
            }
        }

        public Expression<Func<T, bool>> GetPredicateContainsExpression<T>(string propertyName, string value) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            var property = Expression.Property(parameter, propertyName);
            var toUpperMethod = typeof(string).GetMethods().Where(m => m.Name == "ToUpper").FirstOrDefault();
            var containsMethod = typeof(string).GetMethods().Where(m => m.Name == "Contains" && m.GetParameters()[0].ParameterType == typeof(string)).FirstOrDefault();
            var containsCall = Expression.Call(Expression.Call(property, toUpperMethod), containsMethod, Expression.Constant(value ?? ""));
            return Expression.Lambda<Func<T, bool>>(containsCall, parameter);
        }

        public Expression<Func<T, object?>> GetOrderExpression<T>(string propertyName) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            Expression conversion = Expression.Convert(Expression.Property(parameter, propertyName), typeof(object));
            return Expression.Lambda<Func<T, object?>>(conversion, parameter);
        }

        public IQueryable<T> OrderQuery<T>(IQueryable<T> query, bool orderDirectionAscending, string orderExpression, string orderExpressionSuffix)
            where T : class, new()
        {
            if (orderExpression == null)
                return query;
            orderExpression = orderExpression.FirstLetterToUpperOthersToLower();
            var property = GetProperty<T>(orderExpression);
            if (property == null)
                return query;
            var valueProperty = orderExpression.EndsWith(orderExpressionSuffix) ? GetProperty<T>(orderExpression.Substring(0, orderExpression.Length - orderExpressionSuffix.Length)) : GetProperty<T>(orderExpression);
            if (valueProperty == null)
                valueProperty = GetProperty<T>(orderExpression);
            if (valueProperty == null)
                return query;
            ParameterExpression parameter = Expression.Parameter(typeof(T), "c");
            Expression body = valueProperty.Name.Split('.').Aggregate<string, Expression>(parameter, Expression.PropertyOrField);
            return orderDirectionAscending
                ? (IOrderedQueryable<T>)Queryable.OrderBy(query, (dynamic)Expression.Lambda(body, parameter))
                : (IOrderedQueryable<T>)Queryable.OrderByDescending(query, (dynamic)Expression.Lambda(body, parameter));
        }
    }
}
