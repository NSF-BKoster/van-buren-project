// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine.EntitySystem;
using Engine;
using Engine.SoundSystem;
using Engine.MapSystem;
using Engine.MathEx;
using Engine.UISystem;
using Engine.Renderer;

namespace ProjectEntities
{
    public class CombatManagerType : EntityType
    {
    }

    public class CombatManager : Entity
    {
        static CombatManager instance;

        public static CombatManager Instance
        {
            get { return instance; }
        }

        public static bool IsEnabled
        {
            get { return instance != null; }
        }

        [FieldSerialize]
        VBUnitAI starter;

        [FieldSerialize]
        VBUnitAI activeEnt;

        public VBUnitAI ActiveEntity
        {
            get { return activeEnt; }
        }

        [FieldSerialize]
        int turnNumber;

        [FieldSerialize]
        LinkedList<VBUnitAI> combatants = new LinkedList<VBUnitAI>();

        CombatManagerType _type = null; public new CombatManagerType Type { get { return _type; } }

        public LinkedList<VBUnitAI> GetCombatants()
        {
            return combatants;
        }

        public CombatManager()
        {
            if (instance != null)
                Log.Fatal("CombatManager: instance already created");

            instance = this;
            turnNumber = 0;
        }

        public static void StartCombat(VBUnitAI starter)
        {
            if (CombatManager.Instance != null)
                return;

                CombatManager i = (CombatManager)Entities.Instance.Create("CombatManager", Map.Instance);
                i.PostCreate();
                i.CreateCombatantList(starter);
        }

        public void CreateCombatantList(VBUnitAI ch)
        {

            Map.Instance.GetObjects(new Sphere(ch.ControlledObject.Position, ch.ControlledObject.ViewRadius + 30), MapObjectSceneGraphGroups.UnitGroupMask, delegate(MapObject mapObject)
            {
                VBCharacter c = mapObject as VBCharacter;
                if (c!= null) combatants.AddLast( c.Intellect );
            });

            //reset all tasks
            foreach (VBUnitAI combatant in combatants)
                combatant.ResetForCombat();

            starter = activeEnt = ch;
            ch.InitiatetTurn();

            //TODO: ARRANGE LIST BY SEQUENCE
        }

        VBUnitAI GetNextPlayerActive(VBUnitAI LastPlayer)
        {
            VBUnitAI newUnit = LastPlayer;
            activeEnt = null;

             //Get the next player
            if (combatants.Find(newUnit) == combatants.Last)
                newUnit = combatants.First.Value;
            else
                newUnit = combatants.Find(newUnit).Next.Value;

            if (newUnit == null || newUnit.IsSetForDeletion)
                return GetNextPlayerActive(newUnit);
            else
                return newUnit;
        }

        public void TurnEnded()
        {
            //set next player
            activeEnt = GetNextPlayerActive(activeEnt);
            //FIXME: set cam here
            activeEnt.InitiatetTurn();
            turnNumber += 1;
        }

        public bool InCombat(VBUnitAI u)
        {
            foreach (VBUnitAI unit in combatants)
            {
                if (unit != null && !unit.IsSetForDeletion)
                {
                    if (u == unit)
                        return true;
                }
            }

            return false;
        }

        public void JoinCombat(VBUnitAI u)
        {
            combatants.AddLast(u);
        }

        bool FoesAlive()
        {
            VBUnitAI toCheck = null; 

            foreach (VBUnitAI u in combatants)
            {
                if (u != null && !u.IsSetForDeletion)
                {
                    if (toCheck != null)
                    {
                        if ( u.IsEnemy(toCheck) )
                            return true;
                    }
                    else
                    {
                        toCheck = u;
                    }
                }
            }

            return false;
        }

        public void AttemptEnd()
        {
            if (FoesAlive())
                Log.Info("Combat cannot end with nearby hostile creatures.");
            else
                End();
        }

        protected void End()
        {
            SetForDeletion(false);
            //TODO: update statistics for this combat
        }

        protected override void OnDestroy()
        {
            instance = null;
            base.OnDestroy();
        }
    }
}
