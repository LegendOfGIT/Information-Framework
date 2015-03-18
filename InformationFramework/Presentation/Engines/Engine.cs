using System;
using System.Collections.Generic;

using InformationFramework.Presentation.Objects;

namespace InformationFramework.Presentation.Engines
{
    interface Engine
    {
        bool Enabled { get; set; }

        void Initialize();

        void Highlight_Enter(IEnumerable<PresentationObject> items);
        void Highlight_Leave(IEnumerable<PresentationObject> items);
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
