// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.Utils;
using Engine;
using Engine.UISystem;

namespace ProjectEntities
{
    public class TimedExplosiveItemType : ConsumableItemType
    {

    }

    public class TimedExplosiveItem : ConsumableItem
    {
        ExplosiveItemType _type = null; public new ExplosiveItemType Type { get { return _type; } }

        float m_flDetonateTime = 0;

        public void DetonateIn(float detTime)
        {
            if (detTime == 0)
                return;

            m_flDetonateTime = LastTickTime + detTime;
        }

        protected override void OnTick()
        {
            base.OnTick();

            if (m_flDetonateTime != 0 && m_flDetonateTime <= LastTickTime)
                Die();
        }
    }
}
