using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationFramework.Presentation.Objects;
using System.Drawing;

namespace InformationFramework.Presentation.Modifications
{
    public static class ConnectionFactory
    {
        public static void Connect(Graphics graphics, PresentationObject PresentationObject, PointF OffsetPosition, object Graphicslocker)
        {
            if (PresentationObject != null && PresentationObject.Connections != null && PresentationObject.Connections.Any())
            {
                foreach (var ConnectedObject in PresentationObject.Connections)
                {
                    var shape = PresentationObject.Shape;
                    var connectedshape = ConnectedObject.Shape;
                    if ((shape != null && shape.Any()) && (connectedshape != null && connectedshape.Any()))
                    {
                        var pointx = default(float);
                        var pointy = default(float);

                        pointx = default(float);
                        pointy = default(float);
                        foreach (var shapepoint in shape)
                        {
                            pointx += shapepoint.X;
                            pointy += shapepoint.Y;
                        }
                        var p1 = new PointF(
                            (PresentationObject.Position.Value.X + OffsetPosition.X) + (pointx / shape.Count),
                            (PresentationObject.Position.Value.Y + OffsetPosition.Y) + (pointy / shape.Count)
                        );

                        pointx = default(float);
                        pointy = default(float);
                        foreach (var shapepoint in connectedshape)
                        {
                            pointx += shapepoint.X;
                            pointy += shapepoint.Y;
                        }
                        var p2 = new PointF(
                            (ConnectedObject.Position.Value.X + OffsetPosition.X) + (pointx / connectedshape.Count),
                            (ConnectedObject.Position.Value.Y + OffsetPosition.Y) + (pointy / connectedshape.Count)
                        );

                        lock (Graphicslocker) { graphics.DrawLine(new Pen(Brushes.Gray, 1), p1, p2); }
                    }
                }
            }
        }
    }
}
