using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.MapSystem;

namespace Game
{
    public class ECharNameBox : EVBControl
    {
        EControl window = ControlDeclarationManager.Instance.CreateControl("Gui\\Controls\\main_menu\\charNameEdit.gui");

        protected override void OnAttach()
        {
            base.OnAttach();
            Controls.Add(window);

            ((ETextControl)window).OnClickInArea += delegate(EVBControl sender)
            {
                window.Controls["name_edit"].Visible = true;
            };

            ((EButton)window.Controls["name_edit/name_edit_ok"]).Click += delegate(EButton sender)
            {
                ChangeName();
            };
        }

        void ChangeName()
        {
            if (window.Controls["name_edit/name_edit_box"].Text != "")
            {
                window.Text = window.Controls["name_edit/name_edit_box"].Text;
                window.Controls["name_edit"].Visible = false;
            }
        }

        protected override bool OnKeyDown(KeyEvent e)
        {
            if (e.Key == EKeys.Enter)
                ChangeName();

            return base.OnKeyDown(e);
        }
    }
}