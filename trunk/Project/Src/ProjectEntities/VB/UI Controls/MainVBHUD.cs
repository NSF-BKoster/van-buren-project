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
using ProjectEntities;

namespace ProjectCommon
{
    public class MainVBHUD : Control
    {
        private Control combatPnl;
        [Serialize, Browsable(false)]
        public Control CombatPanel
        {
            get
            {
                return combatPnl;
            }
            set
            {
                if (combatPnl != null)
                    Controls.Remove(combatPnl);

                combatPnl = value;
                Controls.Add(combatPnl);
            }
        }

        private Control datalog;
        [Serialize, Browsable(false)]
        public Control DataLog
        {
            get
            {
                return datalog;
            }
            set
            {
                if (datalog != null)
                    Controls.Remove(datalog);

                datalog = value;
                Controls.Add(datalog);
            }
        }

        private ItemControl itmControl;
        [Serialize, Browsable(false)]
        public ItemControl ItemControl
        {
            get
            {
                return itmControl;
            }
            set
            {
                if (itmControl != null)
                    Controls.Remove(itmControl);

                itmControl = value;
                Controls.Add(itmControl);
            }
        }

        private TextBox healthBox;
        [Serialize, Browsable(false)]
        public TextBox HealthBox
        {
            get
            {
                return healthBox;
            }
            set
            {
                if (healthBox != null)
                    Controls.Remove(healthBox);

                healthBox = value;
                Controls.Add(healthBox);
            }
        }

        protected override Control.StandardChildSlotItem[] OnGetStandardChildSlots()
        {
            return new Control.StandardChildSlotItem[] { new Control.StandardChildSlotItem("CombatPanel", CombatPanel),
            new Control.StandardChildSlotItem("DataLog", DataLog),
            new Control.StandardChildSlotItem("ItemControl", ItemControl),
            new Control.StandardChildSlotItem("HealthBox", HealthBox)};
        }

        public VBCharacter selectedUnit
        {
            get;
            set;
        }
    }
}
