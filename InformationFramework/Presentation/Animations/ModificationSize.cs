using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationFramework.Presentation.Objects;
using System.Drawing;

namespace InformationFramework.Presentation.Modifications
{
    public class ModificationSize : Modification
    {
        public float TargetVector { get; set; }
        public float Vector { get; set; }
    }

    public static class SizeFactory
    {
        public static float Modify(PresentationObject PresentationObject)
        {
            var response = default(float);

            if (PresentationObject != null)
            {
                var activemodifications = PresentationObject.ActiveModifications;
                var modifications = PresentationObject.Animations;
                var movements = default(IEnumerable<Modification>);

                response = PresentationObject.Size;
                movements = activemodifications == null ? movements : activemodifications.Where(item => item is ModificationSize);
                if (movements != null && movements.Any()) {
                    foreach (ModificationSize movement in movements) {
                        //  Modifizieren der eigentlichen Größe
                        response = 
                            response < movement.TargetVector ? response + movement.Vector :
                            response > movement.TargetVector ? response + movement.Vector :
                            response
                        ;

                        response =
                            movement.Vector > 0 && response > movement.TargetVector ? movement.TargetVector :
                            movement.Vector < 0 && response < movement.TargetVector ? movement.TargetVector :
                            response
                        ;
                        PresentationObject.Size = response;

                        //  Anwenden der Größe
                        var shape = PresentationObject.Shape;
                        if (shape != null) { 
                            var center = new PointF(PresentationObject.Shapeheight / 2, PresentationObject.Shapewidth / 2);

                            var newshape = new List<PointF>();
                            foreach (var point in shape) {
                                newshape.Add(new PointF(
                                    point.X + (movement.Vector * (point.X > center.X ? 1 : -1)),
                                    point.Y + (movement.Vector * (point.Y > center.Y ? 1 : -1))
                                ));
                            }
                            PresentationObject.Shape = newshape;
                        }

                        //  Abschluss der Animation (Auslösen der Folgeanimationen, Events, ...)
                        if (movement.TargetVector == PresentationObject.Size) {
                            PresentationObject.LeaveModification(movement);
                        }
                    }
                }
            }

            return response;
        }
    }
}
