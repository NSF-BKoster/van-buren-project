// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.MathEx;

namespace ProjectEntities
{
    public class AmmoItemType : ConsumableItemType
    {
        [FieldSerialize]
        Range dmgModifier;

        public Range DmgModifier
        {
            get { return dmgModifier; }
            set { dmgModifier = value; }
        }

        [FieldSerialize]
        int dmgResistance;

        public int DmgResistance
        {
            get { return dmgResistance; }
            set { dmgResistance = value; }
        }
    }

    public class AmmoItem : ConsumableItem
    {
        AmmoItemType _type = null; public new AmmoItemType Type { get { return _type; } }

        protected override void OnPostCreate(bool loaded)
        {
            if (Juice == 0)
                Juice = Type.MaxJuice;

            base.OnPostCreate(loaded);
        }
    }
}
