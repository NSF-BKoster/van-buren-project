using System;
using System.ComponentModel;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using Engine.SoundSystem;
using System.Drawing.Design;
using System.Collections.Generic;

namespace ProjectCommon
{
    public class ESwitch : Control
    {
        private bool up = true;
        private int mode = 0;

        [Serialize]
        [Category("Switch")]
        public string DownSound { get; set; }
        [Serialize]
        [Category("Switch")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> Textures { get; set; }

        protected override void OnAttach()
        {
            base.OnAttach();

            MouseDown += new MouseButtonDelegate(ESwitch_MouseDown);
        }

        void ESwitch_MouseDown(Control sender, EMouseButtons button)
        {
            if (button == EMouseButtons.Left)
            {
                GetControlManager().PlaySound(DownSound);
                if (mode == 0 || mode == Textures.Count)
                    up ^= true;

                if (up)
                    mode--;
                else
                    mode++;

                BackTexture = TextureManager.Instance.Load(Textures[mode]);
            }
        }
    }
}