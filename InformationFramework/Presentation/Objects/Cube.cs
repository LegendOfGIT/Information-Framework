using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using InformationFramework.Presentation.Modifications;

namespace InformationFramework.Presentation.Objects
{
    public class CubeObject : PresentationObject
    {
        public CubeObject()
        {
            Initialize();
        }
        public CubeObject(Startposition Startingposition) {
            base.Size = 50f;
            this.Startingposition = Startingposition;
        }
        public CubeObject(Startposition Startingposition, float Size)
        {
            base.Size = Size;
            this.Startingposition = Startingposition;
            Initialize();
        }
        private void Initialize()
        {
            Shape = new List<PointF>{
                new PointF{ X = 0, Y = 0 },
                new PointF{ X = base.Size, Y = 0 },
                new PointF{ X = base.Size, Y = base.Size },
                new PointF{ X = 0, Y = base.Size }
            };

            base.SetupStartingposition();
        }
    }
}
