using System.Collections.Generic;

namespace InformationFramework.Animations
{
    using InformationFramework.Presentation.Modifications;
    using InformationFramework.Presentation.Objects;
    using System;
    using System.Drawing;

    public class Glimmer
    {
        private FloatColor HighlightColor = default(FloatColor);
        private Random Random = default(Random);
        public IEnumerable<Modification> Movements;

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
                Vector = 2.00f,
                Repetitions = int.MaxValue
            };
            effectmodifications.Add(lastmodification);
            for (int x = 0; x < Random.Next(20, 70); x++) {
                var dark_light = new ModificationColor
                {
                    Active = true,
                    TargetColor = x % 2 == 0 ? HighlightColor : ColorFactory.ToFloatColor(Color.Black),
                    Vector = 2.00f,
                    Parent = lastmodification
                };
                lastmodification.Modifications = new[] { dark_light };
                lastmodification = dark_light;
            }
            
            Movements = effectmodifications;
        }
    }
}
