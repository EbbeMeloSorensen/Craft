using System;

namespace Craft.Domain
{
    public interface IVersionedObject
    {
        Guid ArchiveID { get; set; }
        DateTime Created { get; set; }
        DateTime Superseded { get; set; }
    }
}