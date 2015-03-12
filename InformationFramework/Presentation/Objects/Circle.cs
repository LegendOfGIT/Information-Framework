using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using InformationFramework.Presentation.Modifications;

namespace InformationFramework.Presentation.Objects
{
    public class CircleObject : PresentationObject
    {
        private float Size { get; set; }

        public CircleObject()
        {
            Initialize();
        }
        public CircleObject(Startposition Startingposition, float Size)
        {
            this.Size = Size;
            this.Startingposition = Startingposition;
            Initialize();
        }
        private void Initialize()
        {
            Shape = new List<PointF>{
                new PointF{ X = 0, Y = 0 },
                new PointF{ X = Size, Y = 0 },
                new PointF{ X = Size, Y = Size },
                new PointF{ X = 0, Y = Size }
            };

            base.SetupStartingposition();
        }
    }
}
