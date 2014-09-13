using System;
using System.ComponentModel;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using Engine.SoundSystem;

namespace GameEntities
{
    /// <summary>
    /// A text control that scrolls like a ticker.
    /// Note it only scrolls when its text is larger than its size.
    /// </summary>
    public class ERadioButton : ESwitch
    {
        public override bool DoClick()
        {
            /*if ()
                return false;*/

            if (mode == 1)
                mode = 2;
            else
                mode = 1;

            return true;
        }
    }
}