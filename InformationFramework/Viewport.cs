﻿using System;
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
        private IEnumerable<PresentationObject> HighlightedItems = default(IEnumerable<PresentationObject>);
        private Scene Scene = default(Scene);
        private Point LastLocation = default(Point);

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
                LastLocation = default(Point);
                Engine.NavigateNext(e);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Engine.NavigatePrevious(e);
            }
        }
        private void Viewport_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) {
                Engine.NavigateUp(e);
            }
            else {
                Engine.NavigateDown(e);
            }
        }

        private void Viewport_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                this.Dispose();
            }
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if (LastLocation != default(Point))
                {
                    Scene.OffsetPosition = new PointF(
                        Scene.OffsetPosition.X - (LastLocation.X - e.X),
                        Scene.OffsetPosition.Y - (LastLocation.Y - e.Y)
                    );
                }

                LastLocation = e.Location;
            }

            //  Highlighting
            var items = Scene.GetHighlightedItems(e.Location);

            if (!items.HashEquals(HighlightedItems)) {
                Engine.Highlight_Enter(items);
                Engine.Highlight_Leave(HighlightedItems);
            }

            HighlightedItems = items;
        }
    }
}
