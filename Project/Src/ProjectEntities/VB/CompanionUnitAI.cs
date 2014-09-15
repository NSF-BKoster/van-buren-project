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
    public class CompanionUnitAIType : VBUnitAIType
    {
        [FieldSerialize]
        float followRange;

        public float FollowRange
        {
            get { return followRange; }
            set { followRange = value; }
        }
    }

    public class CompanionUnitAI : VBUnitAI
    {
        CompanionUnitAIType _type = null; public new CompanionUnitAIType Type { get { return _type; } }

        [FieldSerialize]
        VBCharacter toFollow;

        public VBCharacter ToFollow
        {
            get { return toFollow; }
            set 
            {
                toFollow = value;
                if (ControlledObject.InitialFaction != null)
                    ControlledObject.InitialFaction = toFollow.InitialFaction;
            }
        }

        public CompanionUnitAI()
        {
        }

        float GetDistance()
        {
            return (ControlledObject.Position - toFollow.Position).Length();
        }

        /*protected override bool InactiveFindTask()
        {
            if (toFollow != null && CombatManager.Instance == null)
            {
                if (GetDistance() > Type.FollowRange)
                {
                    DoTask(new Task(Task.Types.BreakableMove, toFollow.Position), false);
                    return true;
                }
            }

            return base.InactiveFindTask();
        }*/

        protected override void TickTasks()
        {
            if (toFollow != null && CombatManager.Instance == null && CurrentTask.Type != Task.Types.BreakableMove)
            {
                if (toFollow != null && GetDistance() > Type.FollowRange)
                    DoTask(new Task(Task.Types.BreakableMove, toFollow.Position), false);
            }

            base.TickTasks();

            if (toFollow != null && CombatManager.Instance == null && CurrentTask.Type == Task.Types.BreakableMove)
            {
                if (GetDistance() <= Type.FollowRange)
                    DoNextTask();
            }
        }
    }
}