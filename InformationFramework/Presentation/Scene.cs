using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InformationFramework.Presentation.Objects;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System;
using InformationFramework.Presentation.Modifications;

namespace InformationFramework.Presentation
{
    public class Scene
    {
        public Form Parent;
        private BufferedGraphics BufferedGraphics = default(BufferedGraphics);
        private object Graphicslocker = new object();

        public PointF OffsetPosition { get; set; }
        public List<PresentationObject> PresentationObjects = new List<PresentationObject>();

        public Scene(Form Parent)
        {
            this.Parent = Parent;

            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Parent.Width + 1, this.Parent.Height + 1);
            BufferedGraphics = context.Allocate(this.Parent.CreateGraphics(),
            new Rectangle(0, 0, this.Parent.Width, this.Parent.Height));
        }

        public void Run() {
            var graphics = BufferedGraphics.Graphics;
            graphics.Clear(Color.Black);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var matrix = default(Matrix);

            if (PresentationObjects != null)
            {
                PresentationObjects.ForEach(PresentationObject =>
                {
                    var modifications = PresentationObject.ActiveModifications;

                    #region Objektbewegung
                    var point = PresentationObject.Position;
                    if (point.HasValue)
                    {
                        //  Winkeländerung
                        var angle = AngleFactory.Modify(PresentationObject);

                        //  Geschwindigkeitsänderung
                        var velocity = VelocityFactory.Modify(PresentationObject);

                        PresentationObject.Angle = angle;
                        PresentationObject.Velocity = velocity;

                        var deltaX = (float)(Math.Sin((Math.PI * PresentationObject.Angle) / 180) * velocity);
                        var deltaY = (float)(-Math.Cos((Math.PI * PresentationObject.Angle) / 180) * velocity);

                        point = new PointF(point.Value.X + deltaX, point.Value.Y + deltaY);
                    }
                    PresentationObject.Position = point;
                    #endregion
                    //  Form und Farbe
                    //  Farbänderung
                    ColorFactory.Modify(PresentationObject);
                    //  Größenänderung
                    SizeFactory.Modify(PresentationObject);

                    matrix = new Matrix();
                    matrix.Translate(
                        (point == null ? 0 : point.Value.X + OffsetPosition.X),
                        (point == null ? 0 : point.Value.Y + OffsetPosition.Y),
                        MatrixOrder.Append
                    );

                    lock (Graphicslocker) { graphics.Transform = matrix; }

                    var brush = ColorFactory.CreateBrush(PresentationObject);
                    var shape = PresentationObject == null ? null : PresentationObject.Shape;
                    if (shape != null && shape.Any())
                    {
                        //  Kreisform
                        if (PresentationObject is CircleObject)
                        {
                            var point1 = shape.Count > 0 ? shape[0] : new PointF();
                            var point3 = shape.Count > 2 ? shape[2] : new PointF();
                            lock (Graphicslocker) {
                                graphics.FillEllipse(
                                    brush,
                                    point1.X,
                                    point1.Y,
                                    point3.X,
                                    point3.Y
                                );
                            }
                        }
                        //  Polygon
                        else
                        {
                            lock (Graphicslocker) { graphics.FillPolygon(brush, shape.ToArray()); }
                        }
                    }

                    //  Text
                    if (PresentationObject is TextObject)
                    {
                        lock (Graphicslocker) {
                            graphics.DrawString(
                                ((TextObject)PresentationObject).Text,
                                new Font("Arial", PresentationObject.Size),
                                brush,
                                new PointF(0, 0)
                            );
                        }
                    }
                });
            }
            
            #region Linie zwischen verbundenen Objekten
            matrix = new Matrix();
            matrix.Translate(
                0,
                0,
                MatrixOrder.Append
            );
            graphics.Transform = matrix;

            if (PresentationObjects != null) {
                PresentationObjects.AsParallel().ForAll(PresentationObject => {
                    ConnectionFactory.Connect(
                        graphics, 
                        PresentationObject, 
                        OffsetPosition,
                        Graphicslocker
                    );
                });
            }
            #endregion

            BufferedGraphics.Render(Graphics.FromHwnd(this.Parent.Handle));
        }
        public IEnumerable<PresentationObject> GetHighlightedItems(Point mouseposition)
        {
            var response = default(IEnumerable<PresentationObject>);

            response = PresentationObjects.Where(item =>
            {
                var shape = item.Shape;
                if (shape != null && shape.Any() && item.Enabled)
                {
                    var minx = shape.Min(p => p.X); minx += OffsetPosition.X + item.Position.Value.X;
                    var maxx = shape.Max(p => p.X); maxx += OffsetPosition.X + item.Position.Value.X;
                    var miny = shape.Min(p => p.Y); miny += OffsetPosition.Y + item.Position.Value.Y;
                    var maxy = shape.Max(p => p.Y); maxy += OffsetPosition.Y + item.Position.Value.Y;

                    return
                        mouseposition.X >= minx &&
                        mouseposition.X < maxx &&
                        mouseposition.Y >= miny &&
                        mouseposition.Y < maxy
                    ;
                }

                return false;
            });

            return response;
        }
    }
}
