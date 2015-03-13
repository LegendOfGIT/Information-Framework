using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationFramework.Presentation.Objects;

namespace InformationFramework.Presentation.Modifications
{
    public class ModificationAngle : Modification
    {
        public float TargetVector { get; set; }
        public float ChangingVector { get; set; }
    }   
    public static class AngleFactory
    {
        public static float Create(Startposition position)
        {
            return
                position == Startposition.East ? 90 :
                position == Startposition.North ? 0 :
                position == Startposition.Northeast ? 45 :
                position == Startposition.Northwest ? 315 :
                position == Startposition.South ? 180 :
                position == Startposition.Southeast ? 135 :
                position == Startposition.Southwest ? 225 :
                position == Startposition.West ? 270 :
                0
            ;
        }
        public static void Add(ref float angle, float modification) {
            var response = angle + modification;
            if (response < 0) {
                while (response < 0) { response += 360; }
            }
            else { while (response > 360) { response -= 360; } }

            angle = response;
        }
        public static float Modify(PresentationObject PresentationObject)
        {
            var response = default(float);

            if (PresentationObject != null)
            {
                var modifications = PresentationObject.ActiveModifications;
                var movements = default(IEnumerable<Modification>);

                //  Winkeländerung
                response = PresentationObject.Angle;
                movements = modifications == null ? movements : modifications.Where(item => item is ModificationAngle);
                if (movements != null && movements.Any())
                {
                    foreach (ModificationAngle movement in movements)
                    {
                        var delta = movement.ChangingVector;
                        var newvalue = (response + delta) >= 360 ? 360 - (response + delta) : (response + delta);
                        //  Winkeländerung
                        response =
                            //  Nach Rechts
                            delta > 0 ?
                            response < newvalue && newvalue < movement.TargetVector ? newvalue :
                            movement.TargetVector
                            //  Nach Links
                            :
                            response > newvalue && newvalue > movement.TargetVector ? newvalue :
                            movement.TargetVector
                        ;

                        //  Abschluss der Animation (Auslösen der Folgeanimationen, Events, ...)
                        if (movement.TargetVector == response) {
                            PresentationObject.LeaveModification(movement);
                        }
                    }
                }
            }

            return response;
        }
        public static void Uturn(PresentationObject PresentationObject) {
            if (PresentationObject != null) {
                PresentationObject.Angle = 
                    PresentationObject.Angle + (180 * (PresentationObject.Angle > 180 ? -1 : 1))
                ;
            }
        }
    }
}
