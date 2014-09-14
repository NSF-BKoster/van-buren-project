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
    public class VBBaseNPCAIType : VBUnitAIType
    {
       
    }

    public class VBBaseNPCAI: VBUnitAI
    {
        VBBaseNPCAIType _type = null; public new VBBaseNPCAIType Type { get { return _type; } }
        
      
        public VBBaseNPCAI()
        {

        }

        protected override void TickTasks()
        {
            if (!OutsideCombatOrActive())
                return;
            
            base.TickTasks();
        }
    }
}