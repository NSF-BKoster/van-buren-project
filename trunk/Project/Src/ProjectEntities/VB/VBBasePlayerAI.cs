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
    public class VBBasePlayerAIType : VBUnitAIType
    {
       
    }

    public class VBBasePlayerAI: VBUnitAI
    {
        VBBasePlayerAIType _type = null; public new VBBasePlayerAIType Type { get { return _type; } }
        
      
        public VBBasePlayerAI()
        {

        }

        protected override bool InactiveFindTaskAI()
        {
            //pure virtual function
            return false;
        }

        /// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnTick()"/>.</summary>
        protected override void OnTick()
        {
            base.OnTick();

            if (InitialWeapons == null)
                base.UpdateInitialWeapons();

            TickTasks();

           //no inactivefindtasks
        }
    }
}