// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using ProjectEntities;

namespace ProjectCommon
{
	/// <summary>
	/// Defines a about us window.
	/// </summary>
    public class ObjectInventoryWindow : Control
	{
        Control window;

        List<InventoryObject> invObjects = new List<InventoryObject>();

        public ObjectInventoryWindow(params InventoryObject[] _objects)
        {
            foreach (InventoryObject obj in _objects)
                invObjects.Add(obj);
        }

		protected override void OnAttach()
		{
			base.OnAttach();

            window = ControlDeclarationManager.Instance.CreateControl("VB\\GUI\\HUD\\ObjectInventory.gui");

            foreach (InventoryObject obj in invObjects)
            {
                foreach (Control c in window.Controls)
                {
                    Inventory inv = c as Inventory;

                    if (inv != null && inv.owner == null)
                    {
                        inv.FillItems(obj);
                        break;
                    }
                }
            }

            Controls.Add(window);
		}


		protected override bool OnKeyDown( KeyEvent e )
		{
			if( base.OnKeyDown( e ) )
				return true;
			if( e.Key == EKeys.Escape )
			{
				SetShouldDetach();
				return true;
			}

			return false;
		}
	}
}
