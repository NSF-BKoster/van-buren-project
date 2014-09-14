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
        
        
        [Browsable(false)]
        VBCharacter Owner
        {
            get
            {
                MainVBHUD p = Parent as MainVBHUD;
                if (p != null && p.selectedUnit != null)
                    return p.selectedUnit;

                return null;
            }
        }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            VBCharacter owner = Owner;
            if (owner != null)
            {
                //dont update if i dont need
                bool checkVisible = CombatManager.IsEnabled;
                if (Visible != checkVisible) 
                    SetVisible(checkVisible);

                if (Visible && ActionPointsBar != null) 
                    ActionPointsBar.UpdateAPTextures(owner.ActionPts);
            }
        }

        protected override void OnAttach()
        {
            if (EndCombatButton != null) EndCombatButton.Click += EndCombatButton_Click;
            if (EndTurnButton != null) EndTurnButton.Click += EndTurnButton_Click;
            base.OnAttach();
        }

        void EndTurnButton_Click(Button sender)
        {
            Owner.Intellect.EndTurn();
        }

        void EndCombatButton_Click(Button sender)
        {
            CombatManager combat = CombatManager.Instance;
            if (combat != null)
                combat.AttemptEnd();
        }

        void SetVisible(bool visible)
        {
            if (visible)
            {
                if (!string.IsNullOrEmpty(EnableSound)) GetControlManager().PlaySound(EnableSound);
            }
            else
            {
                if (!string.IsNullOrEmpty(DisableSound)) GetControlManager().PlaySound(DisableSound);
            }

            Visible = visible;
        }
    }
}
