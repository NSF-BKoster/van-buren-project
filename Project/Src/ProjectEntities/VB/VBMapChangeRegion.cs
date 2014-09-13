// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.Renderer;
using Engine.PhysicsSystem;
using Engine.UISystem;

namespace ProjectEntities
{
	/// <summary>
	/// Defines the <see cref="MapChangeRegion"/> entity type.
	/// </summary>
    public class VBMapChangeRegionType : MapChangeRegionType
	{
	}

	/// <summary>
	/// Gives an opportunity of moving of the player between maps. 
	/// When the player gets in this region game loads a new map.
	/// </summary>
    public class VBMapChangeRegion : MapChangeRegion
	{
		VBMapChangeRegionType _type = null; public new VBMapChangeRegionType Type { get { return _type; } }


		public override void  MapChangeRegion_ObjectIn( Entity entity, MapObject obj )
        {
            if (string.IsNullOrEmpty(MapName))
            {
                /*
                EControl win = ControlDeclarationManager.Instance.CreateControl("Gui\\worldmap\\WorldMapWindow.gui");
                if (win != null)
                    ScreenControlManager.Instance.Controls.Add(win);*/
            }
            else
                base.MapChangeRegion_ObjectIn(entity, obj);
        }

	}
}
