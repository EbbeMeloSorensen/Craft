using System;

namespace Craft.Domain
{
    public interface IObjectWithGuidID
    {
        Guid ID { get; set; }
    }
}
