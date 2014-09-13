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

namespace ProjectEntities
{
    public class VBItemType : InteractableObjectType
	{
        [FieldSerialize]
        int weight;

        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        [FieldSerialize]
        int itmvalue;

        public int Value
        {
            get { return itmvalue; }
            set { itmvalue = value; }
        }

        [FieldSerialize]
        string invIcon;
        public string InvIcon
        {
            get { return invIcon; }
            set { invIcon = value; }
        }

        [FieldSerialize]
        string actIcon;

        public string ActIcon
        {
            get { return actIcon; }
            set { actIcon = value; }
        }

		public VBItemType()
		{
			AllowEmptyName = true;
		}
	}

	/// <summary>
	/// Items which can be picked up by units. Med-kits, weapons, ammunition.
	/// </summary>
    public class VBItem : InteractableObject
    {
        [FieldSerialize]
        public VBCharacter Owner;

        //
        VBItemType _type = null; public new VBItemType Type { get { return _type; } }

        protected override void OnPostCreate(bool loaded)
        {
            base.OnPostCreate(loaded);

            if (Owner != null)
                AllowSave = false;
        }

        //THIS OCCURS WHEN THE PLAYER HAS CLICKED ON THE ITEM IN THE CONTROL
        public virtual void ItemClick()
        {
            //pure virtual function
        }

        public virtual bool Use(Dynamic ent)
        {
            return true;
        }

        public virtual void UpdateItem()
        {
            // if im on the ground (no owner) attach a FlareTest
            if (Owner != null)
            {

            }
            else
            {

            }
        }

        public virtual VBItem Drop()
        {
            return Owner.DropItem(Owner.GetCurItem);
        }

        public override bool Interact(Dynamic activator)
        {
            (activator as VBCharacter).TakeItem(this);

            return base.Interact(activator);
        }

        public bool IsWeaponItem()
        {
            return (this as VBWeaponItem) != null || (this as BaseMeleeWeaponItem) != null;
        }
    }
}
