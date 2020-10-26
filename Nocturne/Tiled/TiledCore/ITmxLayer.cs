using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nocturne.Tiled
{
    public interface ITmxLayer : ITmxElement
    {
        Dictionary<string, string> Properties { get; }
    }
}
