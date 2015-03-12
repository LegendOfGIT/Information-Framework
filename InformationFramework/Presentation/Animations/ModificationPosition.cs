using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationFramework.Presentation.Objects;
using System.Drawing;


namespace InformationFramework.Presentation.Modifications
{
    public static class PositionFactory
    {
        public static PointF? Modify(PointF? response, PointF modification)
        {
            return
                response.HasValue && modification != null ?
                new PointF(
                    response.Value.X + modification.X,
                    response.Value.Y + modification.Y
                ) :
                response
            ;
        }
    }
}
