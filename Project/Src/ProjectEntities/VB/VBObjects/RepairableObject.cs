// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using Engine;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.Utils;
using Engine.SoundSystem;
using Engine.MathEx;

namespace ProjectEntities
{
    public class RepairableObjectType : StatusObjectType
    {
       
    }

    public class RepairableObject : StatusObject
	{
        [FieldSerialize]
        float repairRatio;

        public float RepairRatio
        {
            get { return repairRatio; }
            set { repairRatio = value; }
        }

        //
        RepairableObjectType _type = null; public new RepairableObjectType Type { get { return _type; } }
    }
}
