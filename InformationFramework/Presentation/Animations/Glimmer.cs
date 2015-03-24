using System.Collections.Generic;

namespace InformationFramework.Animations
{
    using InformationFramework.Presentation.Modifications;
    using InformationFramework.Presentation.Objects;
    using System;
    using System.Drawing;

    public class Glimmer : Animation
    {
        private FloatColor HighlightColor = default(FloatColor);
        private Random Random = default(Random);

        public Glimmer(PresentationObject presentation) 
        {
            if (presentation != null) {
                Random = new Random(presentation.GetHashCode());

                HighlightColor = presentation.Color;

                //  Anfängliches Ausblenden der Objektfarbe
                presentation.Color = ColorFactory.ToFloatColor(Color.Black);

                //  Generierung des Glimmereffekt
                GlimmerEffect();
            }
        }

        private void GlimmerEffect() {
            var effectmodifications = new List<Modification>();
            var lastmodification = default(Modification);

            lastmodification = new ModificationColor {
                Active = true,
                TargetColor = HighlightColor,
                Vector = 10.00f
            };
            effectmodifications.Add(lastmodification);
            for (int x = 0; x < Random.Next(20, 70); x++) {
                var dark_light = new ModificationColor
                {
                    TargetColor = x % 2 == 0 ? new FloatColor { R = 30 } : ColorFactory.ToFloatColor(Color.Black),
                    Vector = 10.00f,
                    Parent = lastmodification
                };
                lastmodification.Modifications = new[] { dark_light };
                lastmodification = dark_light;
            }

            if (lastmodification != null) { 
                lastmodification.OnLeave += new EventHandler(delegate { 
                    GlimmerEffect(); 
                }); 
            }

            base.Modifications = effectmodifications;
        }
    }
}
