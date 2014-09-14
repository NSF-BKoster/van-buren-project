// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEntities;
using System.IO;

namespace ProjectCommon
{
    public class Inventory : SlotHolder
    {
        TextBox weightLabel = new TextBox();

        [Serialize]
        [Editor(typeof(EditorTrueTypeFontUITypeEditor), typeof(UITypeEditor))]
        public Font WeightFont
        {
            get { return weightLabel.Font; }
            set
            {
                weightLabel.Font = value;
            }
        }

        protected override void OnAttach()
        {
            weightLabel.HorizontalAlign = Engine.Renderer.HorizontalAlign.Center;
            weightLabel.VerticalAlign = Engine.Renderer.VerticalAlign.Top;

            Controls.Add(weightLabel);
            base.OnAttach();
        }

        public void FillItems(InventoryObject obj)
        {
            owner = obj;

            foreach (InventoryObject.InventoryObjectItem itm in obj.Inventory)
            {
                VBCharacter rtsOwner = owner as VBCharacter;
                if (rtsOwner != null)
                {
                    if (rtsOwner.Inventory.IndexOf(itm)+1 == rtsOwner.ActiveItems[0] || rtsOwner.Inventory.IndexOf(itm)+1 == rtsOwner.ActiveItems[1])
                        continue;
                }

                if (!FoundAndUpdatedEntry(itm))
                    Controls.Add(new TestSlot(itm)); 
            }

            RefreshItemsPosition();
        }

        protected override void OnControlDetach(Control control)
        {
            if (!IsShouldDetach())
                RefreshItemsPosition();

            base.OnControlDetach(control);
        }

        protected override void OnControlAttach(Control control)
        {
            RefreshItemsPosition();
            base.OnControlAttach(control);
        }

        void RefreshItemsPosition()
        {
            int p = 0, e = 0, t = 0;

            foreach (Control slot in Controls)
            {
                if (slot as TestSlot != null)
                {
                    if (t % 2 == 0)
                    {
                        slot.Position = new ScaleValue(ScaleType.ScaleByResolution, new Vec2(0, (p * slot.Size.Value.Y)));
                        p++;
                    }
                    else
                    {
                        slot.Position = new ScaleValue(ScaleType.ScaleByResolution, new Vec2(slot.Size.Value.X + (slot.Size.Value.X / 4), (e * slot.Size.Value.Y)));
                        e++;
                    }
                    t++;
                }
            }

            if (owner != null)
                weightLabel.Text = string.Format("Total Weight: {0} / {1}", owner.InvWeight(), owner.GetMaxWeight());
        }
    }

    public class SlotHolder : Control
    {
        public InventoryObject owner;

        List<InventoryObject.InventoryObjectItem> unassigned_itms = new List<InventoryObject.InventoryObjectItem>();

        public enum HolderType
        {
            Inventory,
            Slot1,
            Slot2,
            Armor,
            Drop,
        }
        [Serialize]
        public HolderType holderType
        {
            get { return hType; }
            set
            {
                hType = value;
            }
        }
        private HolderType hType;

        public bool FoundAndUpdatedEntry(InventoryObject.InventoryObjectItem _itm)
        {
            foreach (Control ec in Controls)
            {
                TestSlot tst = ec as TestSlot;
                if (tst != null && tst.assignedItem.ItemType == _itm.ItemType)
                {
                    if ((_itm.ItemType as AmmoItemType) != null)
                    {
                        tst.SetQuantity(_itm.Juice);
                    }
                    else
                    {
                        tst.SetQuantity(tst.quantity + 1);
                        unassigned_itms.Add(_itm);
                    }

                    return true;
                }
            }

            return false;
        }

        public InventoryObject.InventoryObjectItem GetFirstOfTypeUnassignedItem( VBItemType _type )
        {
            foreach (InventoryObject.InventoryObjectItem tmp in unassigned_itms)
            {
                if (tmp.ItemType == _type)
                {
                    unassigned_itms.Remove(tmp);
                    return tmp;
                }
            }

            return null;
        }

        public bool CursorIsInArea()
        {
            Rect rect = new Rect(Vec2.Zero, new Vec2(1f, 1f));
            if (rect.IsContainsPoint(base.MousePosition) && Visible)
                return true;

            return false;
        }
    }
    public class TestSlot : Control
    {
        public InventoryObject.InventoryObjectItem assignedItem;
        TextBox clabel = new TextBox();
        public int quantity = 1;
        bool Active = false;

        public TestSlot(InventoryObject.InventoryObjectItem _item)
        {
            assignedItem = _item;

            MouseDown += ETestSlot_MouseDown;

            Size = new ScaleValue(ScaleType.ScaleByResolution, new Vec2(50f, 50f));

            Texture b = TextureManager.Instance.Load(assignedItem.ItemType.InvIcon, Texture.Type.Type2D, 0);
            if (b != null)
                BackTexture = b;
            else
                Log.Warning("Could not load icon for " +assignedItem.ItemType.Name);

            clabel.HorizontalAlign = HorizontalAlign.Right;
            clabel.VerticalAlign = VerticalAlign.Bottom;
            clabel.TextOffset = new ScaleValue(ScaleType.Parent, new Vec2(0.3f, 0.3f));
            clabel.TopMost = clabel.Visible = true;
        }

        void ETestSlot_MouseDown(Control sender, EMouseButtons button)
        {
            if (Time > 0.5 && button == EMouseButtons.Left)
            {
                GetControlManager().DefaultCursor = assignedItem.ItemType.InvIcon;
                GetControlManager().PlaySound("VB\\Sounds\\InventoryPickUp.ogg");

                Active = true;
            }
        }

        protected override bool OnMouseUp(EMouseButtons button)
        {
            if (Active)
            {
                foreach (Control c in Parent.Parent.Controls)
                {
                    SlotHolder receiver = c as SlotHolder;

                    if (receiver != null && receiver.CursorIsInArea())
                    {
                        SlotHolder sender = Parent as SlotHolder;
                        if (sender != null && sender != receiver)
                            SetSlot(sender, receiver);

                        break;
                    }
                }

                GetControlManager().PlaySound("VB\\Sounds\\InventoryPutDown.ogg");
                GetControlManager().DefaultCursor = "VB\\GUI\\Cursors\\GUIDefault.tga";
                Active = false;
            }

            return base.OnMouseUp(button);
        }


        void SetSlot(SlotHolder _sender, SlotHolder _receiver)
        {
            if (_receiver.holderType > SlotHolder.HolderType.Inventory)
            {
                //TODO: SWAP ITEMS IN INVENTORY
                if (_receiver.Controls.Count != 0)
                    return;

                //TODO: MAKE RECEIVER CHECK FOR ITEM TYPE 

                VBCharacter chSnd = _sender.owner as VBCharacter;
                switch (_receiver.holderType)
                {
                    case SlotHolder.HolderType.Armor:
                        //set player armor
                        break;

                    case SlotHolder.HolderType.Slot1:
                        if (chSnd != null) chSnd.SetItem(assignedItem, true);
                        break;

                    case SlotHolder.HolderType.Slot2:
                        if (chSnd != null) chSnd.SetItem(assignedItem, false);
                        break;

                    case SlotHolder.HolderType.Drop:
                        _sender.owner.DropItem(assignedItem);
                        break;
                }
            }
            else
            {
                if (_sender.holderType == SlotHolder.HolderType.Inventory)
                {
                    if (_receiver.owner.CanHoldItem(assignedItem.ItemType))
                    {
                        _sender.owner.Inventory.Remove(assignedItem);
                        _receiver.owner.Inventory.Add(assignedItem);
                    }
                    else return; //full stop
                }
            }

            //TODO: THE PROBLEM WITH THIS SYSTEM IS THAT OBJECTS CANNOT BE SWAPPED BETWEEN SLOTS, INSTEAD OF NOT ACCEPTING THE NEW HOLDER REMOVE THE PREVIOUS ONE

            //if my sender is on primary or secondary reset its slot
            VBCharacter chRec = _receiver.owner as VBCharacter;
            if (chRec != null)
            {
                if (_sender.holderType == SlotHolder.HolderType.Slot1)
                    chRec.ResetSlot(true);

                if (_sender.holderType == SlotHolder.HolderType.Slot2)
                    chRec.ResetSlot(false);
            }

            //if my quantity is > 1, then instead of swapping the slot, add a new identical one to the receiver
            //and decrease the quntity to the initial one while strring the new one to 0

            if (quantity > 1 && (assignedItem.ItemType as AmmoItemType) == null )
            {
                if (_receiver.holderType != SlotHolder.HolderType.Drop)
                {
                    //create a new and identical slot but with the assigned item of the other object
                    TestSlot stmp = new TestSlot( _sender.GetFirstOfTypeUnassignedItem( assignedItem.ItemType ));
                    _receiver.Controls.Add(stmp);
                }

                SetQuantity(quantity - 1);
            }
            else
            {
                Position = new ScaleValue(ScaleType.ScaleByResolution, Vec2.Zero);
                Parent.Controls.Remove(this);

                if (_receiver.holderType != SlotHolder.HolderType.Drop && !_receiver.FoundAndUpdatedEntry(assignedItem))
                        _receiver.Controls.Add(this);
            }
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            //only set this as ammo
            if (assignedItem.ItemType as AmmoItemType != null)
                SetQuantity(assignedItem.Juice);
            else
                SetQuantity(quantity);

            Controls.Add(clabel);
        }

        public void SetQuantity(int val, int minObjects = 1)
        {
            quantity = val;

            if (quantity > minObjects)
            {
                clabel.Text = quantity.ToString();
                clabel.Visible = true;
            }
            else
            {
                clabel.Visible = false;
            }
        }
    }

	/// <summary>
	/// Defines a about us window.
	/// </summary>
    public class TESTinventory : Control
	{
        Control window;
        InventoryObject owner;

        public TESTinventory(InventoryObject _owner)
        {
            owner = _owner;
           /* ListBox x = new ListBox();

            List<ProjectEntities.InventoryObject.InventoryObjectItem> strippedInv = _owner.Inventory;

            for (int i = 0; i < strippedInv.Count; i++)
            {
                List<ProjectEntities.InventoryObject.InventoryObjectItem> excessItems = strippedInv.FindAll(s => s.ItemType == strippedInv[i].ItemType && strippedInv.IndexOf(s) != i);

                strippedInv.Remove()
            }
            
            //x.Items.Add();
            */

        }

		protected override void OnAttach()
		{
			base.OnAttach();

            window = ControlDeclarationManager.Instance.CreateControl("VB\\GUI\\HUD\\playerInventory.gui");
            (window.Controls["inv"] as Inventory).FillItems(owner);

            //set owners for any slots
            foreach (Control c in window.Controls)
            {
                SlotHolder tmpHolder = c as SlotHolder;

                if (tmpHolder != null && tmpHolder.holderType > SlotHolder.HolderType.Inventory)
                {
                    tmpHolder.owner = owner;

                    VBCharacter rtsOwner = owner as VBCharacter;
                    if (rtsOwner != null)
                    {
                        switch (tmpHolder.holderType)
                        {
                            case SlotHolder.HolderType.Slot1:
                                if (rtsOwner.ActiveItems[0] > 0)
                                    tmpHolder.Controls.Add(new TestSlot(owner.Inventory[rtsOwner.ActiveItems[0] - 1]));
                                break;

                            case SlotHolder.HolderType.Slot2:
                                if (rtsOwner.ActiveItems[1] > 0)
                                    tmpHolder.Controls.Add(new TestSlot(owner.Inventory[rtsOwner.ActiveItems[1] - 1]));
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            Controls.Add(window);
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				SetShouldDetach();
				return true;
			}
          
			return false;
		}
	}
}
