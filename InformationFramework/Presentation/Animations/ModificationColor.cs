using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationFramework.Presentation.Objects;
using System.Drawing;


namespace InformationFramework.Presentation.Modifications
{
    public class ModificationColor : Modification
    {
        public FloatColor? TargetColor { get; set; }
        public float? TargetRed { get; set; }
        public float? TargetGreen { get; set; }
        public float? TargetBlue { get; set; }
        public float Vector { get; set; }
    }

    public static class ColorFactory
    {
        public static Brush CreateBrush(PresentationObject PresentationObject) {
            var response = Brushes.Red;

            if (PresentationObject != null) {
                var color = PresentationObject.Color.ToRGBColor();
                response = new SolidBrush(color);
            }

            return response;
        }
        public static void Modify(PresentationObject PresentationObject)
        {
            if (PresentationObject != null) {
                var activemodifications = PresentationObject.ActiveModifications;
                var modifications = PresentationObject.Modifications;
                var colormodifications = default(IEnumerable<Modification>);

                colormodifications = activemodifications == null ? colormodifications : activemodifications.Where(item => item is ModificationColor);
                if (colormodifications != null) {
                    foreach (ModificationColor modification in colormodifications)  {
                        var objectcolor = PresentationObject.Color;
                        var target = default(float?);

                        //  Rot
                        target = default(float?);
                        target = modification.TargetRed.HasValue ? modification.TargetRed.Value : modification.TargetColor.HasValue ? modification.TargetColor.Value.R : target;
                        if (target.HasValue){
                            if (objectcolor.R > target.Value) {
                                objectcolor.R -= modification.Vector; objectcolor.R = objectcolor.R < 0 ? 0 : objectcolor.R;
                            }
                            else if (objectcolor.R < target.Value) {
                                objectcolor.R += modification.Vector; objectcolor.R = objectcolor.R > 255 ? 255 : objectcolor.R;
                            }
                        }

                        //  Grün
                        target = default(float?);
                        target = modification.TargetRed.HasValue ? modification.TargetRed.Value : modification.TargetColor.HasValue ? modification.TargetColor.Value.G : target;
                        if (target.HasValue){
                            if (objectcolor.G > target.Value) {
                                objectcolor.G -= modification.Vector; objectcolor.G = objectcolor.G < 0 ? 0 : objectcolor.G;
                            }
                            else if (objectcolor.G < target.Value) {
                                objectcolor.G += modification.Vector; objectcolor.G = objectcolor.G > 255 ? 255 : objectcolor.G;
                            }
                        }

                        //  Blau
                        target = default(float?);
                        target = modification.TargetRed.HasValue ? modification.TargetRed.Value : modification.TargetColor.HasValue ? modification.TargetColor.Value.B : target;
                        if (target.HasValue){
                            if (objectcolor.B > target.Value) {
                                objectcolor.B -= modification.Vector; objectcolor.B = objectcolor.B < 0 ? 0 : objectcolor.B;
                            }
                            else if (objectcolor.B < target.Value) {
                                objectcolor.B += modification.Vector; objectcolor.B = objectcolor.B > 255 ? 255 : objectcolor.B;
                            }
                        }

                        PresentationObject.Color = objectcolor;

                        //  Abschluss der Animation (Auslösen der Folgeanimationen, Events, ...)
                        var targetred = modification.TargetColor.HasValue ? modification.TargetColor.Value.R : modification.TargetRed.HasValue ? modification.TargetRed.Value : (float?)null;
                        var targetgreen = modification.TargetColor.HasValue ? modification.TargetColor.Value.G : modification.TargetGreen.HasValue ? modification.TargetGreen.Value : (float?)null;
                        var targetblue = modification.TargetColor.HasValue ? modification.TargetColor.Value.B : modification.TargetBlue.HasValue ? modification.TargetBlue.Value : (float?)null;
                        if (
                            (!targetred.HasValue || PresentationObject.Color.R == targetred.Value) &&
                            (!targetgreen.HasValue || PresentationObject.Color.G == targetgreen.Value) &&
                            (!targetblue.HasValue || PresentationObject.Color.B == targetblue.Value)
                        )
                        {
                            PresentationObject.LeaveModification(modification);
                        }
                    }
                }
            }
        }

        public static FloatColor ToFloatColor(this Color color)
        {
            return
                color == null ?
                new FloatColor { } :
                new FloatColor { R = color.R, G = color.G, B = color.B }
            ;
        }
        public static Color ToRGBColor(this FloatColor floatcolor)
        {
            //  Rot
            var r = (int)(floatcolor.R); r = r < 0 ? 0 : r;
            //  Grün
            var g = (int)(floatcolor.G); g = g < 0 ? 0 : g;
            //  Blau
            var b = (int)(floatcolor.B); b = b < 0 ? 0 : b;

            return Color.FromArgb(r, g, b);
        }
    }
}
