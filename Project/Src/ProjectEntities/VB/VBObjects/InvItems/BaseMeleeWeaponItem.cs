// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;

namespace ProjectEntities
{
    public class BaseMeleeWeaponItemType : MultipleActionItemType
    {
     
    }


    //TODO: split this class into generic weapon and shootable
    public class BaseMeleeWeaponItem : MultipleActionItem
    {
        BaseMeleeWeaponItemType _type = null; public new BaseMeleeWeaponItemType Type { get { return _type; } }

        public override bool Use(Dynamic ent)
        {
            if (base.Use(ent))
            {
                Owner.BaseAttack(ent, GetCurActionMode().Command);
                return true;
            }

            return false;
        }

        public override void ItemClick()
        {
            (Owner.Intellect as RTSUnitAI).DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.PreUse), false);
            CombatManager.StartCombat(Owner.Intellect as VBUnitAI);
        }
    }
}
