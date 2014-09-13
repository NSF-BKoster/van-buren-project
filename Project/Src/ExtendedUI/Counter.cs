using System;
//using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
//using System.Drawing.Design;
//using System.Windows.Forms;
using Engine;
using Engine.UISystem;
//using Engine.MapSystem;
//using Engine.EntitySystem;
using Engine.MathEx;
using Engine.Renderer;

using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace GameEntities
{
    public class ECounter : EControl
    {
        ETextBox Box1;
        ETextBox Box2;

        [Serialize]
        public override string Text
        {
            get { return Box1.Text; }
            set
            {
                Box1.Text = value;
            }
        }
        private string text;

        protected override void  OnAttach()
        {
            Box1 = new ETextBox();
            Box2 = new ETextBox();
            Box1.Name = "A";
            Box2.Name = "B";

            Box1.Text = "1";
            Box2.Text = "2";

 	        base.OnAttach();
        }

        protected override void OnRenderUI(GuiRenderer renderer)
        {

        }

        protected override void OnResize()
        {

        }

        public void SwapLabels()
        {
            string tmp = Box1.Name;
            Box1.Name = Box2.Name;
            Box2.Name = tmp;
        }
    }
}
