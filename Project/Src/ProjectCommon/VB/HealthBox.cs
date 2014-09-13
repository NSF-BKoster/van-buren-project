using Engine.Renderer;
using Engine.UISystem;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace ProjectCommon
{
    public class HealthBox : TextBox
    {
        /*
        public override string StringFormat()
        {
            return "000";
        }*/

        public int NumValue()
        {
            return Convert.ToInt32(Text);
        }

        [Serialize]
        [Category("General")]
        public float UpdateDelay
        { get; set; }

        private int realval;
    
        [Browsable(false)]
        public int RealValue
        {
            get { return realval; }
            set
            {
                realval = value;
                uptoDate = false;
            }
        }
        
        float NextUpdate;
        public bool uptoDate = true;

        public void ForceValue(int val)
        {
            RealValue = val;
            Text = val.ToString();
        }

        protected override void OnTick(float delta)
        {
            if (!uptoDate)
            {
                if (Time > NextUpdate)
                {
                    if (NumValue() == RealValue)
                        uptoDate = true;
                    else
                    {
                        NextUpdate = Time + UpdateDelay;
                        if (NumValue() < RealValue)
                            Text = (NumValue() + 1).ToString();
                        else
                            Text = (NumValue() - 1).ToString();
                    }
                }
            }

            base.OnTick(delta);
        }
    }
}
