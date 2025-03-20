using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainRs.DDD;

public interface IAgreggateRoot
{
    ICollection<IDomainEvent> Events { get; }
}
