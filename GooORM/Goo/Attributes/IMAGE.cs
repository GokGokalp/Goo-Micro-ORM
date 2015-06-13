using System;

namespace Goo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IMAGE : Attribute
    {
    }
}