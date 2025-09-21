using System;

namespace Craft.Domain
{
    public interface IObjectWithValidTime : IVersionedObject
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
    }
}