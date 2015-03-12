using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace InformationFramework.Presentation.Objects
{
    public class TextObject : PresentationObject
    {
        public string Text { get; set; }

        public TextObject()
        {
            Initialize();
        }
        public TextObject(Startposition Startingposition) {
            base.Size = 50f;
            this.Startingposition = Startingposition;
        }
        public TextObject(Startposition Startingposition, float Size)
        {
            base.Size = Size;
            this.Startingposition = Startingposition;
            Initialize();
        }
        private void Initialize()
        {
            base.SetupStartingposition();
        }
    }
}
