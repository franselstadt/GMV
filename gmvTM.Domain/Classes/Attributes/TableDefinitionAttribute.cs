using System;

namespace gmvTM.Domain
{
    public enum TableDeleteBehavior
    {
        Cascade,
        Restrict,
        SetNull,
        NoAction
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class TableDefinitionAttribute : Attribute
    {
        private int maxLength;
        private bool isRequired;
        private bool isUnique;
        private string? uniqueGroup;
        private Type? foreignKeyOf;
        private TableDeleteBehavior onDelete = TableDeleteBehavior.Cascade;
        private bool autoInclude;

        public int MaxLength
        {
            get { return this.maxLength; }
            set { this.maxLength = value; }
        }

        public bool IsRequired
        {
            get { return this.isRequired; }
            set { this.isRequired = value; }
        }

        public bool IsUnique
        {
            get { return this.isUnique; }
            set { this.isUnique = value; }
        }

        public string? UniqueGroup
        {
            get { return this.uniqueGroup; }
            set { this.uniqueGroup = value; }
        }

        public Type? ForeignKeyOf
        {
            get { return this.foreignKeyOf; }
            set { this.foreignKeyOf = value; }
        }

        public TableDeleteBehavior OnDelete
        {
            get { return this.onDelete; }
            set { this.onDelete = value; }
        }

        public bool AutoInclude
        {
            get { return this.autoInclude; }
            set { this.autoInclude = value; }
        }
    }
}
