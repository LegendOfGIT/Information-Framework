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

    public class AnimateCircle
    {
        private float angle = default(float);

        private Form Parent;
        private BufferedGraphics BufferedGraphics = default(BufferedGraphics);

        public AnimateCircle(Form Parent)
        {
            this.Parent = Parent;

            this.Parent.BackColor = Color.White;

            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Parent.Width + 1, this.Parent.Height + 1);
            BufferedGraphics = context.Allocate(this.Parent.CreateGraphics(),
            new Rectangle(0, 0, this.Parent.Width, this.Parent.Height));

            int w = 20;

            this.Parent.Paint += OnPaint;
        }
        protected void Abort_Click(object sender, EventArgs e)
        {
        }
        protected void Suspend_Click(object sender, EventArgs e)
        {
        }
        protected void Resume_Click(object sender, EventArgs e)
        {
        }
        protected void OnPaint(object sender, PaintEventArgs e)
        {

        }
        public void Run()
        {
            angle += 4;
            if (angle > 359)
            {
                angle = 0;
            }

            var graphics = BufferedGraphics.Graphics;
            graphics.Clear(Color.Black);

            Matrix matrix = new Matrix();
            matrix.Rotate(angle, MatrixOrder.Append);
            matrix.Translate(this.Parent.ClientSize.Width / 2, this.Parent.ClientSize.Height / 2, MatrixOrder.Append);
            graphics.Transform = matrix;
            graphics.FillRectangle(Brushes.Azure, -100, -100, 200, 200);
            BufferedGraphics.Render(Graphics.FromHwnd(this.Parent.Handle));
        }
    }
}
