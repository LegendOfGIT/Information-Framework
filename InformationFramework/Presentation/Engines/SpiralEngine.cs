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
    public class SpiralEngine : Engine
    {
        private Scene Scene = default(Scene);
        private InformationItem CurrentItem = default(InformationItem);
        private Startposition Direction = Startposition.North;
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

        public SpiralEngine() { }
        public SpiralEngine(Scene Scene)
        {
            this.Scene = Scene;
            this.Enabled = true;
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
            var items = Informationitems ?? (Provider.ToArray()[0] as FilesystemProvider).GrabItems();
            var previouspresentationobject = default(PresentationObject);
            for (int a = 0; a < items.Count(); a++)
            {
                var item = items.ToArray()[a];

                var type = item.Properties.FirstOrDefault(prop => prop.ID == InformationProperty.Type).Values.FirstOrDefault();
                var presentationobject = new CircleObject(Startposition.Center, 30f) {
                    Color = (type == FilesystemProvider.Directory ? Color.Red : Color.Orange).ToFloatColor()
                };

                var modificationangle = AngleFactory.Create(Startposition.West);
                var initialmodification = new ModificationAngle {
                    Active = true,
                    ChangingVector = modificationangle,
                    TargetVector = modificationangle
                };
                if (a == items.Count()-1) { 
                    initialmodification.OnLeave += ItemsMoved_OnLeave;
                    CurrentItem = items.Last();
                }
                presentationobject.Modifications = new[] { initialmodification };

                //var modificationangle = AngleFactory.Create(Startposition.West);
                //var modifications = new List<Modification>();
                //var modification = (Modification)new ModificationAngle {
                //    Active = true,
                //    ChangingVector = modificationangle,
                //    TargetVector = modificationangle
                //};
                //for (int x = 0; x <= i; x++) {
                //    AngleFactory.Add(ref modificationangle, 22.5f);

                //    if (modification != null) {
                //        var speedup = new ModificationVelocity{ TargetVector = x * 1.3f, ChangingVector = x * 0.90f };
                //        var slowdown = new ModificationVelocity { TargetVector = 0f, ChangingVector = (x * 0.4f) * -1 };
                //        modification.Modifications = new Modification[]{
                //            new ModificationAngle{ TargetVector = modificationangle, ChangingVector = 7.8f },
                //            speedup
                //        };
                //        speedup.Modifications = new[] { slowdown };
                        
                //        var nextmodification = slowdown;
                //        nextmodification.Parent = modification;
                //        modification = nextmodification;
                //    }
                //}

                //while (modification.Parent != null) {
                //    modification = modification.Parent;
                //}

                //presentationobject.Connections = previouspresentationobject == null ? null : new[] { previouspresentationobject };

                //presentationobject.Modifications = new[]{ modification };

                item.PresentationObject = presentationobject;
                previouspresentationobject = presentationobject;
                presentationobject.Shadow = (PresentationObject)presentationobject.Clone();
            }
            Informationitems = items;

            Scene.PresentationObjects = new List<PresentationObject>();
            foreach (var information in Informationitems)
            {
                if (information.PresentationObject != null)
                {
                    Scene.PresentationObjects.Add(information.PresentationObject);
                }
            }
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
        protected void ItemsMoved_OnLeave(object sender, EventArgs e) {
            if (Informationitems != null && CurrentItem != null) {
                this.Enabled = false;
                var currentitemindex = Informationitems.ToList().IndexOf(CurrentItem);

                foreach (var item in Informationitems)
                {
                    var itemindex = Informationitems.ToList().IndexOf(item);

                    if (itemindex <= currentitemindex) {
                        var slowdown = MoveStep(item, true);

                        if (item == Informationitems.Last() || itemindex == currentitemindex)
                        {
                            slowdown.OnLeave += ItemsMoved_OnLeave;
                            CurrentItem = currentitemindex > 0 && Informationitems.Count() > 0 ? Informationitems.ToArray()[currentitemindex - 1] : null;
                        }
                    }
                }
            }
            else { this.Enabled = true; }
        }
        private Modification MoveStep(InformationItem item, bool forward)
        {
            var response = default(Modification);

            var targetvelocityvector = 1.8f;

            var presentationobject = item == null ? null : item.PresentationObject;
            var presentationshadow = presentationobject == null ? null : presentationobject.Shadow;
            var modificationangle = presentationshadow == null ? default(float) : presentationshadow.Angle;
            var shadowvelocity = presentationshadow == null ? default(float) : presentationshadow.Velocity;

            var keepmodifications =
                (forward && Direction == Startposition.South) ||
                (!forward && Direction == Startposition.North)
            ;
            var factor = (forward ? 1 : -1);
            var steps = shadowvelocity / targetvelocityvector;
            steps =
                keepmodifications ? steps :
                steps + (1 * factor)
            ;
            var targetvelocity = steps * targetvelocityvector;
            var speedup = new ModificationVelocity { TargetVector = targetvelocity * factor, ChangingVector = (steps * 0.90f) * factor, Active = true };
            var slowdown = new ModificationVelocity { TargetVector = 0f, ChangingVector = (steps * 4.21f) * (-1 * factor) };

            if (!keepmodifications) { 
                AngleFactory.Add(ref modificationangle, (22.5f * factor));
            }

            presentationobject.Modifications = new Modification[]{
                new ModificationAngle{ TargetVector = modificationangle, ChangingVector = (7.8f * factor), Active = true },
                speedup
            };
            speedup.Modifications = new[] { slowdown };

            presentationobject.Shadow = (PresentationObject)presentationobject.Clone();
            presentationobject.Shadow.Velocity = targetvelocity;
            presentationobject.Shadow.Angle = modificationangle;

            response = slowdown;

            return response;
        }

        public void NavigateDown(EventArgs e)
        {
            if (Informationitems != null && this.Enabled)
            {
                var currentitemindex = Informationitems.ToList().IndexOf(CurrentItem);
                this.Enabled = false;
                foreach (var item in Informationitems)
                {
                    var itemindex = Informationitems.ToList().IndexOf(item);
                    var slowdown = MoveStep(item, false);

                    if (item == Informationitems.Last() || itemindex == currentitemindex)
                    {
                        slowdown.OnLeave += ItemsMoved_OnLeave;
                    }
                }

                Direction = Startposition.South;
            }
        }

        public void NavigatePrevious(EventArgs e)
        {
            if (CurrentItem != null)
            {
                this.CurrentItem = CurrentItem.Parent;
                Informationitems = (Provider.ToArray()[0] as FilesystemProvider).GrabItems(this.CurrentItem);
            }
            PopulateInformation();
        }

        public void NavigateNext(EventArgs e)
        {
            var mouseeventargs = e as MouseEventArgs;
            var highlighteditems = Scene.GetHighlightedItems(mouseeventargs == null ? Cursor.Position : mouseeventargs.Location);

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
            if (Informationitems != null && this.Enabled)
            {
                var currentitemindex = Informationitems.ToList().IndexOf(CurrentItem);
                this.Enabled = false;
                foreach (var item in Informationitems)
                {
                    var itemindex = Informationitems.ToList().IndexOf(item);
                    var slowdown = MoveStep(item, true);

                    if (item == Informationitems.Last() || itemindex == currentitemindex) {
                        slowdown.OnLeave += ItemsMoved_OnLeave;
                    }
                }

                Direction = Startposition.North;
            }
        }
    }
}
