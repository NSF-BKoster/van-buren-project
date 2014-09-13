 // Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine;
using Engine.MapSystem;

namespace ProjectEntities
{
    public class ExplosiveDetonatorType : MultipleActionItemType
    {
    }

    public class ExplosiveDetonator : MultipleActionItem
    {
        ExplosiveDetonatorType _type = null; public new ExplosiveDetonatorType Type { get { return _type; } }

        public override void ItemClick()
        {
            AttemptDetonate();
        }

        public void AttemptDetonate()
        {
            Map.Instance.GetObjects(Map.Instance.InitialCollisionBounds, delegate( MapObject mapObject )
			{
                ExplosiveItem tmp = mapObject as ExplosiveItem;
                if (tmp != null && tmp.ActionMode == ActionMode)
                    tmp.Die();
            });
        }
    }
}