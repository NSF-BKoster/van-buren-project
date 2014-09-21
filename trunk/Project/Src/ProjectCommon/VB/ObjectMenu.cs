using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using System.ComponentModel;
using ProjectEntities;

namespace ProjectCommon
{
    public class EObjectMenu : Control
    {
        /*
         * modes:
         * 
         * talk
         * inspect
         * skill
         * steal
         * */
        
        Dynamic entity;
        List<Button> buttons = new List<Button>();

        public EObjectMenu(Dynamic ent)
        {
            entity = ent;
        }

        protected override void OnAttach()
        {
            ExtendMenu();

            Size = new ScaleValue(ScaleType.Parent, new Vec2(0.05f, 0.2f));
            BackColor = new ColorValue(0, 0, 0, .5f);

            Vec2 pos = new Vec2(Parent.MousePosition.X + (Size.Value.X/2), Parent.MousePosition.Y);
            Position = new ScaleValue(ScaleType.Parent, pos);

            base.OnAttach();
        }

        protected bool CursorIsInArea()
        {
            if ( GetScreenRectangle().IsContainsPoint(MousePosition) && Visible )
                return true;

            return false;
        }
        
        public void ExtendMenu()
        {
            if (entity as RTSCharacter != null)
            {
                    AddButton("Chat");
            }
            else
            {
                if (entity as InventoryObject != null)
                    AddButton("Loot");
            }

            AddButton("Skills", false).OverControl.MouseUp += delegate(Control sender, EMouseButtons button)
            {
                //EngineConsole.Instance.ExecuteString("createWindow Gui\\ActiveSkills.gui");
            };

            AddButton("Look", false).OverControl.MouseUp += delegate(Control sender, EMouseButtons button)
            {
                Log.Info("Get better description of {0}", entity.Type.Name);
            };

            //construct my list or hide what is not necessary
            //check if object has button requirements
            //construct button which has a certain command
            //reposition it to flow down
            FinalizeExtending();
        }

        Button AddButton(string command, bool autoTask = true)
        {
            Button but = ControlDeclarationManager.Instance.CreateControl("Gui\\Controls\\ObjectButton.gui") as Button;
            but.Name = but.Text = command;
            but.HorizontalAlign = Engine.Renderer.HorizontalAlign.Center;

            if (autoTask)
            {
                but.OverControl.MouseUp += delegate(Control sender, EMouseButtons button)
                {
                    //PlayerIntellect.Instance.SetTask(entity, (PlayerIntellect.TaskType)Enum.Parse(typeof(PlayerIntellect.TaskType), command));
                };
            }
            Controls.Add(but);
            return but;
        }

        void FinalizeExtending()
        {
            int n = 0;
            foreach (Control c in Controls)
            {
                c.Position = new ScaleValue(ScaleType.Parent, new Vec2(0, (c.Size.Value.Y + 5) * n));
                n++;
            }

            //Size = new ScaleValue(ScaleType.Texture, new Vec2(Size.Value.X, Controls[0].Size.Value.Y * n * (5*n) )); 
        }

        protected override bool OnMouseUp(EMouseButtons button)
        {
            SetShouldDetach();
            return base.OnMouseUp(button);
        }
    }
}