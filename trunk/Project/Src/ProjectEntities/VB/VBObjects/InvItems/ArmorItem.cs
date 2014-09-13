// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;

namespace ProjectEntities
{
    public class ArmorItemType : ItemType
    {
        [FieldSerialize]
        int resistance;

        public int Resistance
        {
            get { return resistance; }
            set { resistance = value; }
        }

        [FieldSerialize]
        int threshold;

        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }
    }

    public class ArmorItem : Item
    {
        ArmorItemType _type = null; public new ArmorItemType Type { get { return _type; } }

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);
        }
    }
}
