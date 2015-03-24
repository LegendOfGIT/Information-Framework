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
        private IEnumerable<InformationItem> Informationitems = default(IEnumerable<InformationItem>);
        private InformationItem Infotextitem = default(InformationItem);
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
                        hightlightitem.Animations.Add(new Shake());
                    }
                }
            }
        }
        protected void MouseMove(object sender, MouseEventArgs e)
        {
        }

        public void Initialize()
        {
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
                var slowdown = new ModificationVelocity { Vector = -0.069f, TargetVector = 0f }; 
                slowdown.OnLeave += new EventHandler(delegate { presentationobject.Enabled = true; });

                var speedup = new ModificationVelocity
                {
                    Active = true,
                    Vector = 6f,
                    TargetVector = 6f,
                    Modifications = new Modification[]{
                        slowdown
                    }
                };
                presentationobject.Animations.Add(new CustomAnimation { Modifications = new[]{ speedup } });
                presentationobject.Animations.Add(new Glimmer(presentationobject));

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

        public void Highlight_Enter(IEnumerable<PresentationObject> items)
        {
            var info =
                items == null || !items.Any() ? null :
                Informationitems.FirstOrDefault(item => item.PresentationObject == items.FirstOrDefault())
            ;
            if (info != null)
            {
                var infopresentation = info.PresentationObject;
                if (infopresentation != null)
                {
                    if (!infopresentation.Animations.Any(animation => animation.Modifications.Any(mod => mod is ModificationSize))) { 
                        infopresentation.Animations.Add(new CustomAnimation{ Modifications = new[]{
                            new ModificationSize{ 
                                Active = true, TargetVector = infopresentation.Size + 33f, Vector = 4.5f
                            }
                        }});
                    }
                    infopresentation.Color = Color.Green.ToFloatColor();
                }

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
        public void Highlight_Leave(IEnumerable<PresentationObject> items)
        {
            var info =
                items == null || !items.Any() ? null :
                Informationitems.FirstOrDefault(item => item.PresentationObject == items.FirstOrDefault())
            ;
            if (info != null)
            {
                var type = info.Properties.FirstOrDefault(prop => prop.ID == InformationProperty.Type).Values.FirstOrDefault();
                var infopresentation = info.PresentationObject;
                if (infopresentation != null)
                {
                    if (!infopresentation.Animations.Any(animation => animation.Modifications.Any(mod => mod is ModificationSize))) {
                        infopresentation.Animations.Add(new CustomAnimation{ Modifications = new[]{
                            new ModificationSize{ 
                                Active = true, TargetVector = infopresentation.Size - 33f, Vector = -6f
                            }
                        }});
                    }

                    if (infopresentation.Enabled) {
                        infopresentation.Color = (type == FilesystemProvider.Directory ? Color.Red : Color.Orange).ToFloatColor();
                    }
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

                            var slowdown = new ModificationVelocity { Vector = -0.069f, TargetVector = 0f };
                            var speedup = new ModificationVelocity
                            {
                                Active = true,
                                Vector = 6f,
                                TargetVector = 6f,
                                Modifications = new Modification[]{
                                    slowdown
                                }
                            };
                            slowdown.OnLeave += new EventHandler(delegate{
                                Informationitems = (Provider.ToArray()[0] as FilesystemProvider).GrabItems(chosenitem);
                                PopulateInformation();
                            });

                            modifications.Add(speedup);
                            presentationobject.Animations.Add(new CustomAnimation{ Modifications = modifications });

                            chosenpresentationobject.Animations.Add(new CustomAnimation{ Modifications = modifications });
                        }
                        else
                        {
                            var fadeout = new ModificationColor
                            {
                                Active = true,
                                Vector = 4.5f,
                                TargetRed = 0f
                            };
                            modifications.Add(fadeout);
                            presentationobject.Animations.Add(new CustomAnimation{ Modifications = modifications });
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
