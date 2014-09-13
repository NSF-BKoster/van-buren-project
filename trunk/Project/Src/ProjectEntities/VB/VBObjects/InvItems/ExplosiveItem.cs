// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.MapSystem;

namespace ProjectEntities
{
    public class ExplosiveItemType : MultipleActionItemType
    {
    }

    public class ExplosiveItem : MultipleActionItem
    {
        ExplosiveItemType _type = null; public new ExplosiveItemType Type { get { return _type; } }

        [FieldSerialize]
        int frequency;

        public int Frequency
        {
            get { return frequency; }
        }

        public override void ItemClick()
        {
            ExplosiveItem tmp = Drop() as ExplosiveItem;
            tmp.frequency = ActionMode;
            Engine.Log.Info("Dropped explosive with frequency {0}", tmp.Frequency+1);
        } 
    }
}
