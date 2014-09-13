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
    public class CombatPanel : Control
    {
        private BarPanel apBar;
        [Serialize, Browsable(false)]
        public BarPanel ActionPointsBar 
        { 
            get 
            { 
                return apBar; 
            } 
            set 
            {
                if (apBar != null)
                    Controls.Remove(apBar);

                apBar = value;
                Controls.Add(apBar);
            } 
        }

        private Button endTurn;
        [Serialize, Browsable(false)]
        public Button EndTurnButton
        {
            get
            {
                return endTurn;
            }
            set
            {
                if (endTurn != null)
                    Controls.Remove(endTurn);

                endTurn = value;
                Controls.Add(endTurn);
            }
        }

        private Button endCombat;
        [Serialize, Browsable(false)]
        public Button EndCombatButton
        {
            get
            {
                return endCombat;
            }
            set
            {
                if (endCombat != null)
                    Controls.Remove(endCombat);

                endCombat = value;
                Controls.Add(endCombat);
            }
        }

        protected override Control.StandardChildSlotItem[] OnGetStandardChildSlots()
        {
            return new Control.StandardChildSlotItem[] { new Control.StandardChildSlotItem("ActionPointsBar", ActionPointsBar),
            new Control.StandardChildSlotItem("EndTurnButton", EndTurnButton),
            new Control.StandardChildSlotItem("EndCombatButton", EndCombatButton)};
        }


        [Serialize]
        [Category("Bar Settings")]
        [Editor(typeof(EditorSoundUITypeEditor), typeof(UITypeEditor))]
        public string EnableSound { get; set; }

        [Serialize]
        [Category("Bar Settings")]
        [Editor(typeof(EditorSoundUITypeEditor), typeof(UITypeEditor))]
        public string DisableSound { get; set; }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            MainVBHUD p = Parent as MainVBHUD;
            if (p != null && p.selectedUnit != null)
            {
                SetVisible( CombatManager.Instance != null );
                if (ActionPointsBar != null && p.selectedUnit != null) ActionPointsBar.UpdateAPTextures( p.selectedUnit.InCombatAndActive() ? p.selectedUnit.ActionPts : 0 );
            }
        }

        void SetVisible(bool visible)
        {
            if (visible)
                if (!string.IsNullOrEmpty(EnableSound)) GetControlManager().PlaySound(EnableSound);
            else
                if (!string.IsNullOrEmpty(DisableSound)) GetControlManager().PlaySound(DisableSound);

            Visible = visible;
        }
    }
}
