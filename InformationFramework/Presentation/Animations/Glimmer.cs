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

            var fadeout = new ModificationColor {
                TargetColor = ColorFactory.ToFloatColor(Color.Black),
                Vector = 6.00f
            };
            var fadein = new ModificationColor {
                Active = true,
                TargetColor = HighlightColor,
                Vector = 10.00f,
                Modifications = new[]{ fadeout }
            };
            effectmodifications.Add(fadein); 
            lastmodification = fadeout;

            for (int x = 0; x < Random.Next(20, 60); x++) {
                var dark_light = new ModificationColor
                {
                    TargetColor = x % 2 == 0 ? new FloatColor { R = Random.Next(10, 30) } : ColorFactory.ToFloatColor(Color.Black),
                    Vector = 0.50f,
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
