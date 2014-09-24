// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.Renderer;
using Engine.PhysicsSystem;
using Engine;

namespace ProjectEntities
{
	/// <summary>
	/// Defines the <see cref="MapChangeRegion"/> entity type.
	/// </summary>
    public class LayerVisibilityZoneType : RegionType
	{
	}

	/// <summary>
	/// Gives an opportunity of moving of the player between maps. 
	/// When the player gets in this region game loads a new map.
	/// </summary>
    public class LayerVisibilityZone : Region
	{
        /*[FieldSerialize]
        List<string> managedObjects;

        public List<string> ManagedObjects
        {
            get { return managedObjects; }
            set { managedObjects = value; }
        }*/
		//

        LayerVisibilityZoneType _type = null; public new LayerVisibilityZoneType Type { get { return _type; } }

        protected override void OnObjectIn(MapObject obj)
        {
            ShowObjects(ShouldShow());

            base.OnObjectIn(obj);
        }

        protected override void OnObjectOut(MapObject obj)
        {
            ShowObjects(ShouldShow());

            base.OnObjectOut(obj);
        }

        void ShowObjects(bool bShow)
        {
            foreach (Entity e in Children)
            {
                MapObject obj = e as MapObject;
			    if( obj != null )
				    obj.Visible = bShow;
            }
        }

        bool ShouldShow()
        {
            //if i have at least a player in it, true
            foreach ( MapObject o in ObjectsInRegion)
            {
                if (o as VBCharacter != null)
                    return true;
            }

            return false;
        }
	}
}
