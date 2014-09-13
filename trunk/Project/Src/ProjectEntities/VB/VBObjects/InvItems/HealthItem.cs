// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;

namespace ProjectEntities
{
    public class VBHealthItemType : ConsumableItemType
    {
       
    }

    public class VBHealthItem : ConsumableItem
    {
        VBHealthItemType _type = null; public new VBHealthItemType Type { get { return _type; } }

        public override bool Use(Dynamic ent)
        {
            if (base.Use(ent))
            {
                ent.Health += new Random().Next(12, 20);

                //drop it and make sure we wont save it upon load or map transition
                if (Juice == 0)
                    Drop();
            }

            return false;
        }
    }
}
