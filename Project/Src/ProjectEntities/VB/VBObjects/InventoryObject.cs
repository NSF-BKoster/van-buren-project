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
using ProjectCommon;

namespace ProjectEntities
{
    /// <summary>
	/// Defines the <see cref="Unit"/> entity type.
	/// </summary>
    public class InventoryObjectType : StatusObjectType
    {
        [FieldSerialize]
        int maxWeight;

        public int MaxWeight
        {
            get { return maxWeight; }
            set { maxWeight = value; }
        }
    }

	/// <summary>
    /// OBJECT WHICH CONTAINS AN INVENTORY FOR LOOTING. CAN VE A CHARACTER, LOCKER OR CORPSE
	/// </summary>
    public class InventoryObject : StatusObject
	{
        [FieldSerialize]
        List<InventoryObjectItem> items = new List<InventoryObjectItem>();

        [TypeConverter(typeof(CollectionTypeConverter))]
        public List<InventoryObjectItem> Inventory
        {
            get { return items; }
            set { items = value; }
        }

        ///////////////////////////////////////////
        public class InventoryObjectItem
        {
            [FieldSerialize]
            VBItemType itemType;

            [FieldSerialize]
            int actionMode;

            [FieldSerialize]
            int juice = -1;

            [FieldSerialize]
            AmmoItemType ammoType;

            public int Juice
            {
                get 
                {
                    //In the mapper -1 means default value
                    if (itemType != null && EntitySystemWorld.Instance.WorldSimulationType != WorldSimulationTypes.Editor)
                    {
                        ConsumableItemType tmp = itemType as ConsumableItemType;
                        if (tmp != null && juice == -1)
                            juice = tmp.MaxJuice;
                    }

                    return juice; 
                }
                set { juice = value; }
            }

            //
            public void SetType(VBItemType _type)
            {
                itemType = _type;
            }

            public void SetJuice(int _juice)
            {
                juice = _juice;
            }

            public VBItemType ItemType
            {
                get { return itemType; }
                set { itemType = value;}
            }

            public int ActionMode
            {
                get { return actionMode; }
                set { actionMode = value; }
            }

            public AmmoItemType AmmoType
            {
                get { return ammoType; }
                set { ammoType = value; }
            }
        }
        ///////////////////////////////////////////

        //
        InventoryObjectType _type = null; public new InventoryObjectType Type { get { return _type; } }

        [Browsable(false)]
        public virtual int GetMaxWeight
        {
            get { return Type.MaxWeight; }
        }

        public void AddNewObject(string type)
        {
            TakeItem(Entities.Instance.Create(type, Map.Instance) as VBItem);
        }

        public override bool Interact(Dynamic activator)
        {
            EngineConsole.Instance.ExecuteString("createWindow inv");
            return base.Interact(activator);
        }

        public int InvWeight()
        {
            int count = 0;
            foreach (InventoryObjectItem itm in Inventory)
                count += itm.ItemType.Weight;

            return count;
        }

        public bool CanHoldItem(VBItemType i)
        {
            if (InvWeight() + i.Weight <= GetMaxWeight)
                return true;

            return false;
        }

        public virtual bool TakeItem(VBItem i)
        {
            if (!CanHoldItem(i.Type))
                return false;

            bool shouldAdd = true;

            //is it ammo and i already have one?
            if (i.Type as AmmoItemType != null)
            {
                InventoryObjectItem existing = Inventory.Find(search => search.ItemType == i.Type);
                if (existing != null)
                {
                    existing.Juice += (i as AmmoItem).Juice;
                    shouldAdd = false;
                }
            }

            if (shouldAdd)
            {
                InventoryObjectItem tmp = new InventoryObjectItem();
                tmp.SetType(i.Type);

                ConsumableItem ctmp = i as ConsumableItem;
                if (ctmp != null)
                {
                    tmp.SetJuice(ctmp.Juice);

                    if (ctmp.Juice > 0)
                    {
                        VBWeaponItem wtmp = ctmp as VBWeaponItem;
                        if (wtmp != null) tmp.AmmoType = wtmp.AmmoTypeLoaded;
                    }
                }

                Inventory.Add(tmp);
            }

            i.SetForDeletion(false);
            return true;
        }

        public virtual bool SwapItem(InventoryObjectItem i, InventoryObject giver)
        {
            if ( InvWeight() + i.ItemType.Weight <= GetMaxWeight)
                return false;

            Inventory.Add(i);
            giver.Inventory.Remove(i);

            return true;
        }

        public virtual VBItem DropItem(InventoryObjectItem i)
        {
            Inventory.Remove(i);

            if (i.Juice == 0)
                return null;

            VBItem tmp = Entities.Instance.Create(i.ItemType, Map.Instance) as VBItem;
            tmp.Position = Position;

            MultipleActionItem matmp = tmp as MultipleActionItem;
            if (matmp != null)
            {
                matmp.ActionMode = i.ActionMode;

                ConsumableItem ctmp = tmp as ConsumableItem;
                if (ctmp != null) ctmp.Juice = i.Juice;
            }

            tmp.PostCreate();
            tmp.PhysicsModel.Bodies[0].ClearForces();

            return tmp;
        }
    }
}
