using System;
using System.Text;
using System.ComponentModel;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Engine.UISystem
{
    /// <summary>
    /// A text control that scrolls like a ticker.
    /// Note it only scrolls when its text is larger than its size.
    /// </summary>
    public class ETBox : EListBox
    {
        public string fullText = null;

        /*string GetLastTextLines(int num)
        {
            int lines = 1;

            foreach (char c in fullText)
            {
                if (c == '\n')
                    lines++;
            }

            if (lines <= num)
                return fullText;


          fullText.

            return Text;
        }*/

        public override string Text
        {
            get
            {return base.Text; }
            set
            {
               
            }
        }


        protected override void OnAttach()
        {
            base.OnAttach();

        }

        protected override void OnResize()
        {
            base.OnResize();

            for (int i = 0; i <= 4; i++)
            {
                this.Items.Add(i);
            }
        }
    }
}
