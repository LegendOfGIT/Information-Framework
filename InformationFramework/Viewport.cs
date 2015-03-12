using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace InformationFramework
{
    using System.Collections.Generic;

    using Provider;
    using InformationFramework.Models;
    using InformationFramework.Presentation.Objects;
    using InformationFramework.Presentation;
    using InformationFramework.Animations;
    using InformationFramework.Presentation.Modifications;
    using InformationFramework.Presentation.Engines;

    public partial class Viewport : Form
    {
        private Engine Engine = default(Engine);
        private Scene Scene = default(Scene);

        public Viewport()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize(){
            //  Vollbild
            var screenbounds = Screen.PrimaryScreen.Bounds;
            this.Size = new Size(screenbounds.Width, screenbounds.Height);
            Scene = new Scene(this);

            Engine = new SpreadEngine(Scene);
            Engine.Initialize();
        }

        private void Animationtimer_Tick(object sender, EventArgs e)
        {
            Scene.Run();
        }

        private void Viewport_Load(object sender, EventArgs e)
        {

        }

        private void Viewport_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                Engine.NavigateNext(e);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Engine.NavigatePrevious(e);
            }
        }
    }
}
