using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; }=Guid.CreateVersion7();
    }
}
