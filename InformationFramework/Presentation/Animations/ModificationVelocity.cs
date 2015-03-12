using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationFramework.Presentation.Objects;

namespace InformationFramework.Presentation.Modifications
{
    public class ModificationVelocity : Modification
    {
        public float TargetVector { get; set; }
        public float ChangingVector { get; set; }
    }

    public static class VelocityFactory
    {
        public static float Modify(PresentationObject PresentationObject)
        {
            var response = default(float);

            if (PresentationObject != null)
            {
                var activemodifications = PresentationObject.ActiveModifications;
                var modifications = PresentationObject.Modifications;
                var movements = default(IEnumerable<Modification>);

                response = PresentationObject.Velocity;
                movements = activemodifications == null ? movements : activemodifications.Where(item => item is ModificationVelocity);
                if (movements != null && movements.Any())
                {
                    foreach (ModificationVelocity movement in movements)
                    {
                        var delta = movement.ChangingVector;
                        var newvalue = response + delta;
                        //  Geschwindigkeitsänderung
                        response =
                            //  Beschleunigen
                            delta > 0 ?
                            response < newvalue && newvalue < movement.TargetVector ? newvalue :
                            movement.TargetVector
                            //  Verlangsamen
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
    }
}
