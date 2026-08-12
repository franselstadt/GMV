using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

using gmvTM.Domain.Classes.DTOs.Base;

namespace gmvTM.Domain
{
    public static class ItemFactory
    {
        public static ILogger? Logger { get; set; }

        public static TItem CreateItem<TItem>(object phantom) where TItem : class, new()
        {
            if (phantom is null)
                throw new ArgumentNullException(nameof(phantom), gmvDomain.Messages.FactoryPhantomRequired);

            TItem item = new TItem();

            foreach (PropertyInfo source in phantom.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                PropertyInfo? target = typeof(TItem).GetProperty(source.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (target is null)
                {
                    if (item is BaseDTO dto)
                    {
                        dto.DynamicProperties[source.Name] = source.GetValue(phantom);
                        Logger?.LogWarning(gmvDomain.Messages.LogFactoryDynamicPropertyAdded, source.Name, typeof(TItem).Name);
                        continue;
                    }

                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.FactoryPropertyNotFound, source.Name, typeof(TItem).Name));
                }

                if (!target.CanWrite)
                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.FactoryPropertyNotFound, source.Name, typeof(TItem).Name));

                target.SetValue(item, CoerceValue(source.GetValue(phantom), source, target, typeof(TItem)));
            }

            return item;
        }

        public static List<TItem> CreateItems<TItem>(IEnumerable<object> phantoms) where TItem : class, new()
        {
            List<TItem> items = new List<TItem>();

            foreach (object phantom in phantoms)
                items.Add(CreateItem<TItem>(phantom));

            return items;
        }

        private static object? CoerceValue(object? value, PropertyInfo source, PropertyInfo target, Type itemType)
        {
            Type targetType = Nullable.GetUnderlyingType(target.PropertyType) ?? target.PropertyType;

            if (value is null)
            {
                if (target.PropertyType.IsValueType && Nullable.GetUnderlyingType(target.PropertyType) is null)
                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.FactoryPropertyTypeMismatch, source.Name, source.PropertyType.Name, itemType.Name, target.Name, target.PropertyType.Name));

                return null;
            }

            if (targetType.IsInstanceOfType(value))
                return value;

            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType) && !targetType.IsEnum)
            {
                try
                {
                    return Convert.ChangeType(value, targetType);
                }
                catch (Exception exception) when (exception is InvalidCastException || exception is FormatException || exception is OverflowException)
                {
                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.FactoryPropertyTypeMismatch, source.Name, source.PropertyType.Name, itemType.Name, target.Name, target.PropertyType.Name));
                }
            }

            throw new InvalidOperationException(string.Format(gmvDomain.Messages.FactoryPropertyTypeMismatch, source.Name, source.PropertyType.Name, itemType.Name, target.Name, target.PropertyType.Name));
        }
    }
}
