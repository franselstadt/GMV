using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using gmvTM.Domain.Items.Base;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Workers
{
    public sealed class TableConfigurationWorker : ITableConfigurationWorker
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            IReadOnlyList<Type> itemTypes = typeof(BaseItem).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(BaseItem).IsAssignableFrom(t))
                .OrderBy(t => t.Name, StringComparer.Ordinal)
                .ToList();

            foreach (Type itemType in itemTypes)
                ConfigureEntity(modelBuilder, itemType);

            foreach (Type itemType in itemTypes)
                ConfigureRelationships(modelBuilder, itemType, itemTypes);
        }

        private static void ConfigureEntity(ModelBuilder modelBuilder, Type itemType)
        {
            BaseItem instance = (BaseItem)Activator.CreateInstance(itemType)!;
            EntityTypeBuilder entity = modelBuilder.Entity(itemType);

            entity.ToTable(instance.TableName);
            entity.HasKey(nameof(BaseItem.ID));

            Dictionary<string, List<string>> uniqueGroups = new Dictionary<string, List<string>>(StringComparer.Ordinal);

            foreach (PropertyInfo property in PersistableProperties(itemType))
            {
                if (property.GetCustomAttribute<ViewAttribute>(inherit: true) is not null)
                {
                    entity.Ignore(property.Name);
                    continue;
                }

                if (string.Equals(property.Name, nameof(BaseItem.ID), StringComparison.Ordinal))
                    continue;

                if (IsCollectionNavigation(property))
                    continue;

                IReadOnlyList<TableDefinitionAttribute> definitions = property
                    .GetCustomAttributes<TableDefinitionAttribute>(inherit: true)
                    .ToList();

                PropertyBuilder column = entity.Property(property.Name);

                foreach (TableDefinitionAttribute definition in definitions)
                {
                    if (definition.MaxLength > 0)
                        column.HasMaxLength(definition.MaxLength);

                    if (definition.IsRequired)
                        column.IsRequired();

                    if (definition.IsUnique)
                        entity.HasIndex(property.Name).IsUnique();

                    if (string.IsNullOrWhiteSpace(definition.UniqueGroup))
                        continue;

                    if (!uniqueGroups.TryGetValue(definition.UniqueGroup, out List<string>? members))
                    {
                        members = new List<string>();
                        uniqueGroups[definition.UniqueGroup] = members;
                    }

                    members.Add(property.Name);
                }
            }

            foreach (List<string> members in uniqueGroups.Values)
                entity.HasIndex(members.ToArray()).IsUnique();
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder, Type itemType, IReadOnlyList<Type> itemTypes)
        {
            foreach (PropertyInfo property in PersistableProperties(itemType))
            {
                if (property.GetCustomAttribute<ViewAttribute>(inherit: true) is not null)
                    continue;

                if (IsCollectionNavigation(property))
                    continue;

                TableDefinitionAttribute? definition = property
                    .GetCustomAttributes<TableDefinitionAttribute>(inherit: true)
                    .FirstOrDefault(d => d.ForeignKeyOf is not null);

                if (definition?.ForeignKeyOf is not Type principalType)
                    continue;

                if (!itemTypes.Contains(principalType))
                    throw new InvalidOperationException(string.Format(gmvDomain.Messages.ForeignKeyPrincipalNotPersistable, itemType.Name, property.Name, principalType.Name));

                DeleteBehavior deleteBehavior = ToDeleteBehavior(definition.OnDelete);
                PropertyInfo? navigation = CollectionNavigationFor(principalType, itemType);

                if (navigation is null)
                {
                    modelBuilder.Entity(itemType)
                        .HasOne(principalType, null)
                        .WithMany()
                        .HasForeignKey(property.Name)
                        .OnDelete(deleteBehavior);

                    continue;
                }

                modelBuilder.Entity(principalType)
                    .HasMany(itemType, navigation.Name)
                    .WithOne()
                    .HasForeignKey(property.Name)
                    .OnDelete(deleteBehavior);

                bool autoInclude = navigation
                    .GetCustomAttributes<TableDefinitionAttribute>(inherit: true)
                    .Any(d => d.AutoInclude);

                if (autoInclude)
                    modelBuilder.Entity(principalType).Navigation(navigation.Name).AutoInclude();
            }
        }

        private static IEnumerable<PropertyInfo> PersistableProperties(Type itemType)
        {
            return itemType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .GroupBy(p => p.Name, StringComparer.Ordinal)
                .Select(g => g.OrderBy(p => p.DeclaringType == itemType ? 0 : 1).First())
                .OrderBy(p => p.DeclaringType == itemType ? 1 : 0)
                .ThenBy(p => p.MetadataToken);
        }

        private static bool IsCollectionNavigation(PropertyInfo property)
        {
            if (!property.PropertyType.IsGenericType)
                return false;

            Type[] arguments = property.PropertyType.GetGenericArguments();
            return arguments.Length == 1 && typeof(BaseItem).IsAssignableFrom(arguments[0]);
        }

        private static PropertyInfo? CollectionNavigationFor(Type principalType, Type childType)
        {
            return principalType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => IsCollectionNavigation(p) && p.PropertyType.GetGenericArguments()[0] == childType);
        }

        private static DeleteBehavior ToDeleteBehavior(TableDeleteBehavior behavior)
        {
            return behavior switch
            {
                TableDeleteBehavior.Cascade => DeleteBehavior.Cascade,
                TableDeleteBehavior.Restrict => DeleteBehavior.Restrict,
                TableDeleteBehavior.SetNull => DeleteBehavior.SetNull,
                _ => DeleteBehavior.NoAction
            };
        }
    }
}
