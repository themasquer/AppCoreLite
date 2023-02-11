using AppCoreLite.Attributes;
using AppCoreLite.Enums;
using AppCoreLite.Extensions;
using AppCoreLite.Models;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace AppCoreLite.Utilities
{
    public class ReflectionUtil
    {
        public PropertyInfo? GetPropertyInfo<T>(T instance, string propertyName) where T : class, new()
        {
            return instance.GetType().GetProperty(propertyName);
        }

        public PropertyInfo? GetPropertyInfo<T>(string propertyName) where T : class, new()
        {
            return typeof(T).GetProperty(propertyName);
        }

        public List<Property>? GetProperties<T>(TagAttributes tagAttribute = TagAttributes.None) where T : class
        {
            List<Property>? properties = null;
            PropertyInfo[] propertyInfoArray = typeof(T).GetProperties();
            List<Attribute> customAttributes;
            Type attributeType;
            string displayName;
            bool attributeFound;
            if (propertyInfoArray != null && propertyInfoArray.Length > 0)
            {
                properties = new List<Property>();
                if (tagAttribute == TagAttributes.None)
                {
                    foreach (var propertyInfo in propertyInfoArray)
                    {
                        displayName = "";
                        customAttributes = propertyInfo.GetCustomAttributes().ToList();
                        if (customAttributes != null && customAttributes.Count > 0)
                        {
                            foreach (var customAttribute in customAttributes)
                            {
                                if (customAttribute.GetType() == typeof(DisplayNameAttribute))
                                {
                                    displayName = ((DisplayNameAttribute)customAttribute).DisplayName;
                                    break;
                                }
                            }
                        }
                        properties.Add(new Property(propertyInfo.Name, displayName));
                    }
                }
                else if (tagAttribute == TagAttributes.Order || tagAttribute == TagAttributes.StringFilter || tagAttribute == TagAttributes.Export)
                {
                    switch (tagAttribute)
                    {
                        case TagAttributes.Order:
                            attributeType = typeof(OrderTagAttribute);
                            break;
                        case TagAttributes.StringFilter:
                            attributeType = typeof(StringFilterTagAttribute);
                            break;
                        default:
                            attributeType = typeof(ExportTagAttribute);
                            break;
                    }
                    foreach (var propertyInfo in propertyInfoArray)
                    {
                        displayName = "";
                        attributeFound = false;
                        customAttributes = propertyInfo.GetCustomAttributes().ToList();
                        if (customAttributes != null && customAttributes.Count > 0)
                        {
                            foreach (var customAttribute in customAttributes)
                            {
                                if (customAttribute.GetType() == attributeType)
                                    attributeFound = true;
                                if (customAttribute.GetType() == typeof(DisplayNameAttribute))
                                    displayName = ((DisplayNameAttribute)customAttribute).DisplayName;
                            }
                            if (attributeFound)
                                properties.Add(new Property(propertyInfo.Name, displayName));
                        }
                    }
                }
            }
            return properties;
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
            var toLowerMethod = typeof(string).GetMethods().Where(m => m.Name == "ToLower").FirstOrDefault();
            var containsMethod = typeof(string).GetMethods().Where(m => m.Name == "Contains" && m.GetParameters()[0].ParameterType == typeof(string)).FirstOrDefault();
            var containsCall = Expression.Call(Expression.Call(property, toLowerMethod), containsMethod, Expression.Constant(value ?? ""));
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
            var property = GetPropertyInfo<T>(orderExpression);
            if (property == null)
                return query;
            var valueProperty = orderExpression.EndsWith(orderExpressionSuffix) ? GetPropertyInfo<T>(orderExpression.Substring(0, orderExpression.Length - orderExpressionSuffix.Length)) : GetPropertyInfo<T>(orderExpression);
            if (valueProperty == null)
                valueProperty = GetPropertyInfo<T>(orderExpression);
            if (valueProperty == null)
                return query;
            ParameterExpression parameter = Expression.Parameter(typeof(T), "c");
            Expression body = valueProperty.Name.Split('.').Aggregate<string, Expression>(parameter, Expression.PropertyOrField);
            return orderDirectionAscending
                ? (IOrderedQueryable<T>)Queryable.OrderBy(query, (dynamic)Expression.Lambda(body, parameter))
                : (IOrderedQueryable<T>)Queryable.OrderByDescending(query, (dynamic)Expression.Lambda(body, parameter));
        }

        public DataTable? ConvertToDataTable<T>(List<T> list) where T : class, new()
        {
            DataTable? dataTable = null;
            PropertyInfo? propertyInfo;
            object? propertyValue;
            var properties = GetProperties<T>(TagAttributes.Export);
            var displayNameAttributes = properties?.Select(p => p.DisplayName).ToList();
            if (properties != null && displayNameAttributes != null && displayNameAttributes.Count > 0)
            {
                dataTable = new DataTable();
                for (int i = 0; i < properties.Count; i++)
                {
                    propertyInfo = GetPropertyInfo<T>(properties[i].Name);
                    if (propertyInfo != null)
                        dataTable.Columns.Add(displayNameAttributes[i], Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }
                foreach (T item in list)
                {
                    DataRow row = dataTable.NewRow();
                    for (int i = 0; i < properties.Count; i++)
                    {
                        propertyValue = GetPropertyInfo(item, properties[i].Name)?.GetValue(item);
                        row[displayNameAttributes[i]] = propertyValue ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }
    }
}
