﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using InformationFramework.Presentation.Modifications;
using InformationFramework.Animations;

namespace InformationFramework.Presentation.Objects
{
    public class PresentationObject : ICloneable
    {
        public float Angle { get; set; }
        public FloatColor Color { get; set; }
        public bool Enabled { get; set; }
        public PointF? Position { get; set; }
        public List<PointF> Shape { get; set; }
        public int Shapewidth { get { return Shape != null && Shape.Any() ? (int)(Shape.Min(point => point.X) + Shape.Max(point => point.X)) : default(int); } }
        public int Shapeheight { get { return Shape != null && Shape.Any() ? (int)(Shape.Min(point => point.Y) + Shape.Max(point => point.Y)) : default(int); } }
        public float Size { get; set; }
        public float Velocity { get; set; }

        public PresentationObject Shadow { get; set; }

        public PresentationObject(){
            this.Animations = new List<Animation>();
            this.Enabled = true;
        }

        public List<Animation> Animations { get; set; }
        public IEnumerable<PresentationObject> Connections { get; set; }
        public IEnumerable<Modification> ActiveModifications
        {
            get {
                var response = new List<Modification>();

                if (this.Animations != null) {
                    foreach (var animation in this.Animations) {
                        var modifications = animation.Modifications;
                        while (modifications != null && modifications.Any()) {
                            var iteration = new List<Modification>();

                            foreach(var movement in modifications){
                                if (movement.Active) { response.Add(movement); }
                                if (movement.Modifications != null && movement.Modifications.Any()) {
                                    iteration.AddRange(movement.Modifications);
                                }
                            }

                            modifications = iteration;
                        }
                    }
                }

                return response;
            }
        }
        public Startposition Startingposition { get; set; }

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
                        var repetitions = parentmodification.Repetitions;
                        if (repetitions > 0 || repetitions == int.MaxValue)
                        {
                            if (repetitions != int.MaxValue) { 
                                parentmodification.Repetitions--;
                            }
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

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public static class PresentationObjectExtensions
    {
        public static bool HashEquals(this IEnumerable<PresentationObject> source, IEnumerable<PresentationObject> target)
        {
            var hashcodesource = source != null && source.Any() ? string.Join(";", source.OrderBy(item => item.GetHashCode()).Select(item => item.GetHashCode())) : null;
            var hashcodetarget = target != null && target.Any() ? string.Join(";", target.OrderBy(item => item.GetHashCode()).Select(item => item.GetHashCode())) : null;

            return hashcodesource == hashcodetarget;
        }
    }
}
