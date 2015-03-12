using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationFramework.Provider
{
    using Models;

    interface Grabbing {
        IEnumerable<InformationItem> GrabItems();
    }
}
