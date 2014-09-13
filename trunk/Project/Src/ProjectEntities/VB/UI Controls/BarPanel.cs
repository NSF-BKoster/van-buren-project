using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using System.ComponentModel;
using Engine.EntitySystem;
using System.Drawing.Design;
using Engine.Utils;

namespace ProjectCommon
{
    public class BarPanel : Control
    {
        [Serialize]
        [Category("Bar Settings")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public Texture PlusTexture { get; set; }

        [Serialize]
        [Category("Bar Settings")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public Texture BarTextureOn { get; set; }

        [Serialize]
        [Category("Bar Settings")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public Texture BarTextureOff { get; set; }

        [Serialize]
        [Category("Bar Settings")]
        public ScaleValue BarSize { get; set; }

        public enum DrawType
        {
            Horizontal,
            Vertical,
        }

        [Serialize]
        [Category("Counter")]
        public DrawType DrawMode
        { get; set; }

        [Serialize]
        [Category("Counter")]
        public float BarDist
        { get; set; }

        [Serialize]
        [Category("Counter")]
        public int Bars
        {
            get
            {
                if (bars < 1) return 1; else return bars;
            }
            set
            {
                if (value < 1) value = 1;
                bars = value;
            }
        }
        private int bars;
        Control[] BarCollection;

        protected override void OnAttach()
        {
            BarCollection = new Control[Bars];

            BarCollection[0] = new Control();
            BarCollection[0].BackTexture = BarTextureOn;
            BarCollection[0].Size = BarSize;
            Controls.Add(BarCollection[0]);

            for (int i = 1; i < BarCollection.Length; i++)
            {
                BarCollection[i] = new Control();
                BarCollection[i].Size = BarSize;

                if (DrawMode == DrawType.Vertical)
                    BarCollection[i].Position = new ScaleValue(BarSize.Type, new Vec2(BarCollection[0].Position.Value.X, BarCollection[i - 1].Position.Value.Y - (BarCollection[i].Size.Value.Y + BarDist)));
                else
                    BarCollection[i].Position = new ScaleValue(BarSize.Type, new Vec2(BarCollection[i - 1].Position.Value.X + BarCollection[i].Size.Value.X + BarDist, BarCollection[0].Position.Value.Y));

                Controls.Add(BarCollection[i]);
            }

            UpdateAPTextures(Bars);
            base.OnAttach();
        }

        public void UpdateAPTextures(int val)
        {
            for (int bar = 0; bar < Bars; bar++)
            {
                if (bar > val - 1)
                {
                    BarCollection[bar].BackTexture = BarTextureOff;
                }
                else
                {
                    if (val > Bars && bar == bars - 1)
                    {
                        BarCollection[bar].BackTexture = PlusTexture;
                        break;
                    }

                    BarCollection[bar].BackTexture = BarTextureOn;
                }
            }
        }
    }
}
