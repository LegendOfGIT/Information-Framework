using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InformationFramework.Models;
using InformationFramework.Provider;
using System.Drawing;
using InformationFramework.Presentation.Modifications;
using InformationFramework.Animations;
using InformationFramework.Presentation.Objects;

namespace InformationFramework.Presentation.Engines
{
    public class SpreadEngine : Engine
    {
        private Scene Scene = default(Scene);
        private InformationItem CurrentItem = default(InformationItem);
        private IEnumerable<PresentationObject> HighlightedItems = default(IEnumerable<PresentationObject>);
        private IEnumerable<InformationItem> Informationitems = default(IEnumerable<InformationItem>);
        private EventHandler Items_OnEnter = default(EventHandler);
        private EventHandler Items_OnLeave = default(EventHandler);
        private InformationItem Infotextitem = default(InformationItem);
        private Point LastLocation = default(Point);
        private IEnumerable<ProviderBase> Provider = new List<ProviderBase> {
            new FilesystemProvider{}
        };

        public bool Enabled { get; set; }

        public SpreadEngine() { }
        public SpreadEngine(Scene Scene)
        {
            this.Scene = Scene;
        }


        protected void MouseUp(object sender, MouseEventArgs e)
        {
            var highlighteditems = Scene.GetHighlightedItems(e.Location);
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (highlighteditems != null && highlighteditems.Any())
                {
                    foreach (var hightlightitem in highlighteditems)
                    {
                        hightlightitem.Modifications = new Shake().Movements;
                    }
                }
            }
        }
        protected void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (LastLocation != default(Point))
                {
                    Scene.OffsetPosition = new PointF(
                        Scene.OffsetPosition.X - (LastLocation.X - e.X),
                        Scene.OffsetPosition.Y - (LastLocation.Y - e.Y)
                    );
                }

                LastLocation = e.Location;
            }

            //  Highlight
            if (Informationitems != null)
            {
                Informationitems.ToList().ForEach(infoitem =>
                {
                    var type = infoitem.Properties.FirstOrDefault(prop => prop.ID == InformationProperty.Type).Values.FirstOrDefault();
                    if (infoitem.PresentationObject != null && infoitem.PresentationObject.Enabled)
                    {
                        infoitem.PresentationObject.Color = (type == FilesystemProvider.Directory ? Color.Red : Color.Orange).ToFloatColor();
                    }
                });
            }
            var items = Scene.GetHighlightedItems(e.Location);

            if (items != HighlightedItems && items.Any())
            {
                if (Items_OnEnter != null) { Items_OnEnter.Invoke(items, new EventArgs()); }
                if (Items_OnLeave != null) { Items_OnLeave.Invoke(HighlightedItems, new EventArgs()); }
            }

            if (items != null)
            {
                foreach (var highlighteditem in items)
                {
                    highlighteditem.Color = Color.Green.ToFloatColor();
                }
            }
            HighlightedItems = items;
        }

        protected void HighlightedItems_OnEnter(object sender, EventArgs e)
        {
            var items = sender as IEnumerable<PresentationObject>;
            var info =
                items == null || !items.Any() ? null :
                Informationitems.FirstOrDefault(item => item.PresentationObject == items.FirstOrDefault())
            ;
            if (info != null)
            {
                Infotextitem = Infotextitem ?? new InformationItem { };
                var type = info.Properties.FirstOrDefault(prop => prop.ID == InformationProperty.Type).Values.FirstOrDefault();
                var textpresentation = new TextObject(Startposition.Northwest, 20)
                {
                    Text = string.Format(
                        "{0}: {1}",
                        type,
                        info.Properties.FirstOrDefault(prop => prop.ID == FilesystemProvider.Directory).Values.FirstOrDefault()
                    ),
                    Color = (type == FilesystemProvider.Directory ? Color.Red : Color.Orange).ToFloatColor()
                };

                Scene.PresentationObjects.Remove(Infotextitem.PresentationObject);
                Infotextitem.PresentationObject = textpresentation;
                Scene.PresentationObjects.Add(Infotextitem.PresentationObject);
            }
        }
        protected void HighlightedItems_OnLeave(object sender, EventArgs e)
        {
        }
        protected void ReturnToCenter_OnLeave(object sender, EventArgs e)
        {
            var chosenitem = Informationitems.FirstOrDefault(item => item.PresentationObject == sender as PresentationObject);
            Informationitems = (Provider.ToArray()[0] as FilesystemProvider).GrabItems(chosenitem);
            PopulateInformation();
        }
        protected void SpreadItems_OnLeave(object sender, EventArgs e)
        {
            if (Informationitems != null)
            {
                Informationitems.ToList().ForEach(item => { if (item.PresentationObject != null) { item.PresentationObject.Enabled = true; } });
            }
        }

        public void Initialize()
        {
            this.Items_OnEnter += HighlightedItems_OnEnter;

            var form = Scene.Parent;
            if (form != null)
            {
                form.MouseUp += MouseUp;
                form.MouseMove += MouseMove;
            }
        }
        public void PopulateInformation()
        {
            var centeritem = new CubeObject(Startposition.Center, 10f)
            {
                Color = Color.LightGray.ToFloatColor()
            };
            var items = Informationitems ?? (Provider.ToArray()[0] as FilesystemProvider).GrabItems();
            for (int i = 0; i < items.Count(); i++)
            {
                var item = items.ToArray()[i];

                var type = item.Properties.FirstOrDefault(prop => prop.ID == InformationProperty.Type).Values.FirstOrDefault();
                var presentationobject = new CircleObject(Startposition.Center, 25f)
                {
                    Color = (type == FilesystemProvider.Directory ? Color.Red : Color.Orange).ToFloatColor(),
                    Angle = 360 - ((360 * i) / items.Count()),
                    Connections = new[] { centeritem },
                    Enabled = false
                };
                var modifications = new List<Modification>();
                var slowdown = new ModificationVelocity { ChangingVector = -0.069f, TargetVector = 0f }; slowdown.OnLeave += SpreadItems_OnLeave;
                var speedup = new ModificationVelocity
                {
                    Active = true,
                    ChangingVector = 6f,
                    TargetVector = 6f,
                    Modifications = new Modification[]{
                        slowdown
                    }
                };
                modifications.Add(speedup);
                presentationobject.Modifications = modifications;

                item.PresentationObject = presentationobject;
            }
            Informationitems = items;

            Scene.PresentationObjects = new List<PresentationObject>();
            Scene.PresentationObjects.Add(centeritem);
            foreach (var information in Informationitems)
            {
                if (information.PresentationObject != null)
                {
                    Scene.PresentationObjects.Add(information.PresentationObject);
                }
            }
        }

        public void NavigateDown(EventArgs e)
        {
        }
        public void NavigatePrevious(EventArgs e)
        {
            if (CurrentItem != null) {
                this.CurrentItem = CurrentItem.Parent;
                Informationitems = (Provider.ToArray()[0] as FilesystemProvider).GrabItems(this.CurrentItem);
            }
            PopulateInformation();
        }
        public void NavigateNext(EventArgs e)
        {
            var mouseevent = e as MouseEventArgs;
            var highlighteditems = Scene.GetHighlightedItems(mouseevent == null ? Cursor.Position : mouseevent.Location);

            LastLocation = default(Point);

            var chosenitem = Informationitems.GetInformationItem(highlighteditems.FirstOrDefault());

            chosenitem = chosenitem != null && chosenitem.PresentationObject != null && chosenitem.PresentationObject.Enabled ? chosenitem : null;
            if (chosenitem != null)
            {
                this.CurrentItem = chosenitem;

                foreach (var presentationobject in Informationitems.Select(item => { return item.PresentationObject; }))
                {
                    presentationobject.Enabled = false;

                    var chosenpresentationobject = chosenitem.PresentationObject;
                    if (chosenpresentationobject != null)
                    {
                        var modifications = new List<Modification>();
                        //  Ausgewähltes Item mit einer Animation zur Mitte zurücklaufen lassen.
                        if (presentationobject == chosenpresentationobject)
                        {
                            AngleFactory.Uturn(chosenpresentationobject);

                            var slowdown = new ModificationVelocity { ChangingVector = -0.069f, TargetVector = 0f };
                            var speedup = new ModificationVelocity
                            {
                                Active = true,
                                ChangingVector = 6f,
                                TargetVector = 6f,
                                Modifications = new Modification[]{
                                        slowdown
                                    }
                            };
                            slowdown.OnLeave += ReturnToCenter_OnLeave;
                            modifications.Add(speedup);
                            presentationobject.Modifications = modifications;

                            chosenpresentationobject.Modifications = modifications;
                        }
                        else
                        {
                            var fadeout = new ModificationColor
                            {
                                Active = true,
                                ChangingVector = 4.5f,
                                TargetRed = 0f
                            };
                            modifications.Add(fadeout);
                            presentationobject.Modifications = modifications;
                        }
                    }
                }
            }
        }
        public void NavigateUp(EventArgs e)
        {
        }
    }
}
