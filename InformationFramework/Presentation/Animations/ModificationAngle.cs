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
                        var newvalue = 360 - ((response + delta) >= 360 ? (response + delta) : 0);
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
