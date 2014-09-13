using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using System.ComponentModel;

namespace ProjectCommon
{
    public class GameLog : ListBox
    {
        protected override void OnAttach()
        {
            base.OnAttach();

            Log.Handlers.InfoHandler +=Handlers_InfoHandler;
        }

        void Handlers_InfoHandler(string text, ref bool dumpToLogFile)
        {
            UpdateTextLog(text);
        }

        public void UpdateTextLog(string tempstring)
        {
            Items.Add("-" + tempstring);

            if (ScrollBar != null && ScrollBar.Visible)
                ScrollBar.Value = ScrollBar.ValueRange.Maximum;
        }
    }
}
