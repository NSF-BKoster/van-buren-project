// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine;

namespace ProjectEntities
{
    public class VBWeaponItemType : ConsumableItemType
    {
        [FieldSerialize]
        float reloadTime;

        public float ReloadTime
        {
            get { return reloadTime; }
            set { reloadTime = value; }
        }

        [FieldSerialize]
        string clipEmpty;

        public string ClipEmpty
        {
            get { return clipEmpty; }
            set { clipEmpty = value; }
        }

        [FieldSerialize]
        List<AmmoItemType> usableAmmoList;

        public List<AmmoItemType> UsableAmmoList
        {
            get { return usableAmmoList; }
            set { usableAmmoList = value; }
        }
    }

    public class VBWeaponItem : ConsumableItem
    {
        [FieldSerialize]
        AmmoItemType ammoTypeLoaded;

        public AmmoItemType AmmoTypeLoaded
        {
            get { return ammoTypeLoaded; }
            set { ammoTypeLoaded = value; }
        }

        VBWeaponItemType _type = null; public new VBWeaponItemType Type { get { return _type; } }

        public override void ItemClick()
        {
            switch (GetCurActionMode().Command)
            {
                case "reload":
                    TryReload();
                    break;
                case "attack":
                    (Owner.Intellect as RTSUnitAI).DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.PreUse), false);
                    CombatManager.StartCombat(Owner.Intellect as VBUnitAI);
                    break;

                default:
                    base.ItemClick();
                    break;
            }
        }

        protected override void OnPostCreate(bool loaded)
        {
            if (Type.UsableAmmoList == null)
                Log.Warning("Created weapon with no usable ammo types");

            base.OnPostCreate(loaded);
        }

        public override void AlertNoConsumable()
        {
            SoundPlay3D(Type.ClipEmpty, .5f, true);
            Log.Info("Weapon has no ammo.");
        }

        public override bool HasRequirements(MultipleActionItemType.ActionMode act)
        {
            if (act.Command == "reload" && Juice == Type.MaxJuice)
                return false;

            return base.HasRequirements(act);
        }

        public override int GetDamage()
        {
            //Ammo was set incorrectly. There is ammo but no type used! Just use default ammo.
            if (Juice > 0 && AmmoTypeLoaded == null)
                AmmoTypeLoaded = Type.UsableAmmoList[0];

            return base.GetDamage();
        }

        public virtual bool CanReload()
        {
            return Juice < Type.MaxJuice;
        }

        public virtual void TryReload()
        {
            if (Owner.Intellect.InCombatAndActive() && !Owner.Intellect.HasActionPoints(GetCurActionMode().ActionPoints) )
                return;

            if (CanReload())
            {
                foreach (InventoryObject.InventoryObjectItem itm in Owner.Inventory)
                {
                    AmmoItemType amType = itm.ItemType as AmmoItemType;
                    if (amType != null && Type.UsableAmmoList.Contains(amType))
                    {
                        ActionMode = Type.ActionModes.IndexOf(GetActionMode("reload"));

                        if (Juice + itm.Juice > Type.MaxJuice)
                        {
                            int juiceToAdd = Type.MaxJuice - Juice;

                            Juice += juiceToAdd;
                            itm.Juice -= juiceToAdd;
                        }
                        else
                        {
                            Juice += itm.Juice;
                            itm.Juice = 0;
                            Owner.DropItem(itm);
                        }

                        AmmoTypeLoaded = amType;
                        SoundPlay3D(GetCurActionMode().PlaySound, .5f, true);
                        IncActMode();
                        Owner.Intellect.IncActionPts(-GetCurActionMode().ActionPoints);
                        break;
                    }
                }
            }
        }

        public override bool Use(Dynamic ent)
        {
            if (base.Use(ent))
            {
                if (Owner != ent)
                    Owner.BaseAttack(ent, GetCurActionMode().Command);
                else
                    Log.Info("You attempt to attack yourself, but think better of it.");

                return true;
            }

            return false;
        }
    }
}
