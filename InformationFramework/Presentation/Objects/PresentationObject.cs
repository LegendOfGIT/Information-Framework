using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using InformationFramework.Presentation.Modifications;

namespace InformationFramework.Presentation.Objects
{
    public class PresentationObject
    {
        public enum Startposition
        {
            Custom,
            Center,
            West,
            Northwest,
            North,
            Northeast,
            East,
            Southeast,
            South,
            Southwest
        }

        public float Angle { get; set; }
        public FloatColor Color { get; set; }
        public bool Enabled { get; set; }
        public PointF? Position { get; set; }
        public List<PointF> Shape { get; set; }
        public int Shapewidth { get { return Shape != null && Shape.Any() ? (int)(Shape.Min(point => point.X) + Shape.Max(point => point.X)) : default(int); } }
        public int Shapeheight { get { return Shape != null && Shape.Any() ? (int)(Shape.Min(point => point.Y) + Shape.Max(point => point.Y)) : default(int); } }
        public float Size { get; set; }
        public float Velocity { get; set; }

        public IEnumerable<PresentationObject> Connections { get; set; }
        public IEnumerable<Modification> Modifications { get; set; }
        public IEnumerable<Modification> ActiveModifications { get {
            var response = new List<Modification>();

            var movements = Modifications;
            while (movements != null && movements.Any()) {
                var iteration = new List<Modification>();

                foreach(var movement in movements){
                    if (movement.Active) { response.Add(movement); }
                    if (movement.Modifications != null && movement.Modifications.Any()) {
                        iteration.AddRange(movement.Modifications);
                    }
                }

                movements = iteration;
            }

            return response;
        }}
        public Startposition Startingposition { get; set; }

        public PresentationObject()
        {
            this.Enabled = true;
        }

        public void SetupStartingposition()
        {
            var shapewidth = Shapewidth;
            var shapeheight = Shapeheight;

            var screen = Screen.PrimaryScreen;
            var screenheight = screen == null ? default(int) : screen.Bounds.Height;
            var screenwidth = screen == null ? default(int) : screen.Bounds.Width;

            var left = 0;
            var middle = (int)((screenwidth / 2) - (shapewidth / 2));
            var right = (int)(screenwidth - shapewidth);

            var top = 0;
            var center = (int)((screenheight / 2) - (shapeheight / 2));
            var bottom = (int)(screenheight - shapeheight);

            Position = new Point(
                Startingposition == Startposition.Custom ? default(int) :
                Startingposition.ToString().ToLower().Contains("east") ? right :
                Startingposition.ToString().ToLower().Contains("west") ? left :
                middle
                ,
                Startingposition == Startposition.Custom ? default(int) :
                Startingposition.ToString().ToLower().Contains("north") ? top :
                Startingposition.ToString().ToLower().Contains("south") ? bottom :
                center
            );
        }
        public void LeaveModification(Modification modification)
        {
            if (modification != null) {
                modification.Active = false;

                //  Auslösen von Folgeanimationen
                if (modification.Modifications != null && modification.Modifications.Any()) {
                    modification.Modifications.ToList().ForEach(mod => { mod.Active = true; });
                }
                //  Keine Folgeanimation. Ggf. Rücksprung zu Animationswiederholung
                else {
                    var parentmodification = modification.Parent;
                    while (parentmodification != null) {
                        if (parentmodification.Repetitions > 0)
                        {
                            parentmodification.Repetitions--;
                            parentmodification.Active = true;
                            break;
                        }
                        parentmodification = parentmodification.Parent;
                    }
                }

                //  Auslösen eines Abschlussevents
                modification.OnLeaveHandler(this);
            }
        }
    }
}
