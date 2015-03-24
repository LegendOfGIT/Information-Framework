using System.Collections.Generic;

using InformationFramework.Presentation.Modifications;

namespace InformationFramework.Animations
{
    public abstract class Animation
    {
        public IEnumerable<Modification> Modifications { get; set; }
    }
}
