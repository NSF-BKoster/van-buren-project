using Engine.MathEx;
using Engine.Renderer;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using Engine.Utils;

namespace Engine.UISystem
{
    public class EControlHolder : EVBControl
    {
       EControl item;
       EControlHolder(EControl c)
       {
           item = c;
       }
    }
}
