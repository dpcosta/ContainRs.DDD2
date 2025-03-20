using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainRs.Contracts;

public interface IEventManager
{
    Task<IEnumerable<T>> GetNotProcessedAsync<T>(string eventType, string readerType);
}
