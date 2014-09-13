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

        [FieldSerialize]
        VBCharacter starter;

        [FieldSerialize]
        VBCharacter activeEnt;

        public VBCharacter ActiveEntity
        {
            get { return activeEnt; }
        }

        [FieldSerialize]
        LinkedList<VBCharacter> combatants = new LinkedList<VBCharacter>();

        CombatManagerType _type = null; public new CombatManagerType Type { get { return _type; } }

        public LinkedList<VBCharacter> GetCombatants()
        {
            return combatants;
        }

        public CombatManager()
        {
            if (instance != null)
                Log.Fatal("CombatManager: instance already created");

            instance = this;
        }

        public static void StartCombat(VBCharacter starter)
        {
            if (CombatManager.Instance != null)
                Log.Warning("Combat started incorrectly!");

                CombatManager i = (CombatManager)Entities.Instance.Create("CombatManager", Map.Instance);
                i.PostCreate();
                i.CreateCombatantList(starter);
        }

        public void CreateCombatantList(VBCharacter ch)
        {
            Map.Instance.GetObjects(new Sphere(ch.Position, ch.ViewRadius + 30), MapObjectSceneGraphGroups.UnitGroup, delegate(MapObject mapObject)
            {
                combatants.AddLast((VBCharacter)mapObject);
            });

            starter = activeEnt = ch;
            if (starter != null)
                ch.StartTurn();

            //TODO: ARRANGE LIST BY SEQUENCE
        }

        VBCharacter GetNextPlayerActive(VBCharacter LastPlayer)
        {
            VBCharacter newUnit = LastPlayer;
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
            if (!FoesAlive()) // should i end? - check for entities and if there are any enemies
                End();
            else
            {
                activeEnt = GetNextPlayerActive(activeEnt);
                //FIXME: set cam here
                activeEnt.StartTurn();
            }
        }

        public bool InCombat(VBCharacter u)
        {
            foreach (VBCharacter unit in combatants)
            {
                if (unit != null && !unit.IsSetForDeletion)
                {
                    if (u == unit)
                        return true;
                }
            }

            return false;
        }

        public void JoinCombat(VBCharacter u)
        {
            combatants.AddLast(u);
        }

        bool FoesAlive()
        {
            VBCharacter toCheck = null; 

            foreach (VBCharacter u in combatants)
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
        }

        protected override void OnDestroy()
        {
            instance = null;
            base.OnDestroy();
        }
    }
}
