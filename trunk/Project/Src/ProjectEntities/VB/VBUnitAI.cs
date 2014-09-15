// Copyright (C) 2006-2013 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine;
using Engine.MathEx;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;

namespace ProjectEntities
{
    /// <summary>
    /// Defines the <see cref="RTSUnitAI"/> entity type.
    /// </summary>
    public class VBUnitAIType : RTSUnitAIType
    {
       
    }

    public class VBUnitAI : RTSUnitAI
    {
        VBUnitAIType _type = null; public new VBUnitAIType Type { get { return _type; } }
        
        [Browsable(false)]
        public new VBCharacter ControlledObject
        {
            get { return (VBCharacter)base.ControlledObject; }
        }
      
        public VBUnitAI()
        {

        }

        public bool IsEnemy(Intellect e)
        {
            //FIXME: check for enemies made
            return (VBFactionManager.Instance != null && VBFactionManager.Instance.AreEnemies(Faction as VBFactionType, e.Faction as VBFactionType));
        }

        public bool InCombatAndActive()
        {
            return CombatManager.Instance != null && CombatManager.Instance.ActiveEntity == this;
        }

        public bool OutsideCombatOrActive()
        {
            if (CombatManager.Instance == null || InCombatAndActive())
                return true;

            return false;
        }

        public bool HasActionPoints(int pts)
        {
            if (ControlledObject.ActionPts < pts)
            {
                Log.Info("Not enough action points");
                return false;
            }

            return true;
        }

        public void IncActionPts(int val)
        {
            ControlledObject.ActionPts += val;

            if (ControlledObject.ActionPts < 1 && InCombatAndActive())
                EndTurn();
        }


        public void InitiatetTurn()
        {
            ControlledObject.Stop();
            ControlledObject.ActionPts = ControlledObject.GetMaxActionPoints;
        }

        public void EndTurn()
        {
            if (CombatManager.Instance == null)
                return;

            ControlledObject.Stop();

            ControlledObject.ActionPts = 0;
            CombatManager.Instance.TurnEnded();
        }

        protected override void TickTasks()
        {
            if (!OutsideCombatOrActive())
                return;


            switch (CurrentTask.Type)
            {
                case Task.Types.Attack:
                case Task.Types.BreakableAttack:
                    //enter combat if i am attacking someone
                    if (!CombatManager.IsEnabled) CombatManager.StartCombat(this);
                    break;
            }
            
            base.TickTasks();
        }

        public override void ClearTaskList()
        {
            base.ClearTaskList();

            RTSCharacter c = ControlledObject as RTSCharacter;
            if (c != null)
                c.IsRunning = false;
        }
    }
}