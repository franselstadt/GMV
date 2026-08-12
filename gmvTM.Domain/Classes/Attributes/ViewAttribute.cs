using System;

namespace gmvTM.Domain
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ViewAttribute : Attribute
    {
    }
}
