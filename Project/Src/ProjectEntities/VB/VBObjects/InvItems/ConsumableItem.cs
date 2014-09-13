// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.UISystem;

namespace ProjectEntities
{
    public class ConsumableItemType : MultipleActionItemType
    {
        [FieldSerialize]
        [DefaultValue(-1)]
        int maxjuice;

        public int MaxJuice
        {
            get { return maxjuice; }
            set { maxjuice = value; }
        }
    }

    public class ConsumableItem : MultipleActionItem
    {
        [FieldSerialize]
        int juice;

        public int Juice
        {
            get { return juice; }
            set { juice = value; }
        }

        ConsumableItemType _type = null; public new ConsumableItemType Type { get { return _type; } }

        public virtual void AlertNoConsumable()
        {
            SoundPlay3D("VB\\Sounds\\IISXXXX1.wav", .5f, true);
        }

        public override bool Use(Dynamic ent)
        {
            if (base.Use(ent))
            {
                if (Juice > 0)
                {
                    Owner.IncActionPts(-GetCurActionMode().ActionPoints);
                    Juice--;
                    return true;
                }

                AlertNoConsumable();
                return false;
            }

            return false;
        }
    }
}
