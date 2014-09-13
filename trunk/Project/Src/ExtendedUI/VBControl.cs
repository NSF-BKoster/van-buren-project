using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using System.ComponentModel;
using Engine.Utils;

namespace Engine.UISystem
{
    /// <summary>
    /// THIS IS A SIMPLE BASECLASS CONTROL WITH NEW FUNCTIONS
    /// </summary>
    public class EVBControl : EControl
    {
        

        public Vec2 CenterPosition()
        {
            return new Vec2(Size.Value.X / 2, Size.Value.Y / 2);
        }

        protected virtual bool CursorIsInArea()
        {
            Rect rect = new Rect(Vec2.Zero, new Vec2(1f, 1f));
            if (rect.IsContainsPoint(base.MousePosition) && Visible)
                return true;

            return false;
        }
    }
}
