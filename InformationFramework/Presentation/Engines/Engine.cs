using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationFramework.Presentation.Engines
{
    interface Engine
    {
        void Initialize();
        void PopulateInformation();
        void NavigateDown(EventArgs e);
        /// <summary>
        /// Navigiert in der Informationsmenge eine Ebene zurück
        /// </summary>
        void NavigatePrevious(EventArgs e);
        /// <summary>
        /// Navigiert in der Informationsmenge eine Ebene vor
        /// </summary>
        void NavigateNext(EventArgs e);
        void NavigateUp(EventArgs e);
    }
}
