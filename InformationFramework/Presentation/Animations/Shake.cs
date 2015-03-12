using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationFramework.Animations
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Threading;
    using System.Windows.Forms;
    using InformationFramework.Presentation.Modifications;

    public class Shake
    {
        public IEnumerable<Modification> Movements;

        public Shake()
        {
            var velocity = 6f;
            var delta = 0.85f;

            var modifications = new Modification[]{ 
                new ModificationAngle{ Active = true, TargetVector = 270, ChangingVector = 270 },
                new ModificationVelocity{ Active = true, TargetVector = (velocity * 2) * -1, ChangingVector = delta }
            };
            var modification = modifications.FirstOrDefault(item => item is ModificationVelocity);
            while (velocity > 0) {
                if (modification != null) {
                    modification.Modifications = new Modification[]{
                        new ModificationAngle{ TargetVector = 90, ChangingVector = 90 },
                        new ModificationVelocity{ TargetVector = velocity * 1, ChangingVector = delta },
                    };
                    modification = modification.Modifications.FirstOrDefault(item => item is ModificationVelocity);
                    modification.Modifications = new Modification[]{
                        new ModificationAngle{ TargetVector = 270, ChangingVector = 270 },
                        new ModificationVelocity{ TargetVector = velocity * -1, ChangingVector = delta },
                    };

                    velocity -= delta;

                    modification = modification.Modifications.FirstOrDefault(item => item is ModificationVelocity);
                }
            }

            modification.Modifications = new Modification[]{
                new ModificationAngle{ TargetVector = 90, ChangingVector = 90 },
                new ModificationVelocity{ TargetVector = 0, ChangingVector = delta },
            };
            //modification = modification.Modifications.FirstOrDefault(item => item is ModificationVelocity);
            //modification.Modifications = new Modification[]{
            //    new ModificationAngle{ TargetVector = 270, ChangingVector = 270 },
            //    new ModificationVelocity{ TargetVector = 0, ChangingVector = delta },
            //};

            Movements = modifications;
        }
    }
}
