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
	public class RadiationSourceType : RegionType
	{
	}

	/// <summary>
	/// Gives an opportunity of moving of the player between maps. 
	/// When the player gets in this region game loads a new map.
	/// </summary>
	public class RadiationSource : Region
	{
        [FieldSerialize]
        float damage = 1f;

        public float DamagePerUpdate
        {
            get { return damage; }
            set { damage = value; }
        }

        [FieldSerialize]
        float nextUpdate = 0.25f;

        public float NextUpdate
        {
            get { return nextUpdate; }
            set { nextUpdate = value; }
        }

        [FieldSerialize]
        float damageRange;

        public float DamageRange
        {
            get { return damageRange; }
            set { damageRange = value; }
        }

        internal float lastUpdate;
		//

        RadiationSourceType _type = null; public new RadiationSourceType Type { get { return _type; } }

        protected override void OnObjectIn(MapObject obj)
        {
            if (obj as VBCharacter != null)
                Log.Info("Caution. Radiation zone detected");

            base.OnObjectIn(obj);
        }

        protected override void OnTick()
        {
            if (LastTickTime > lastUpdate)
            {
                foreach (MapObject obj in ObjectsInRegion)
                {
                    if (obj.IsSetForDeletion)
                        continue;

                    VBCharacter tmpChar = obj as VBCharacter;
                    if (tmpChar != null)
                    {
                        //notify player and hurt him
                        float range = (tmpChar.Position - Position).Normalize();

                        if (range < DamageRange)
                            tmpChar.DoDamage(this, tmpChar.Position, null, DamagePerUpdate, false);

                        GeigerCounter tmpCounter = tmpChar.ActiveHeldItem as GeigerCounter;
                        if (tmpCounter != null)
                            tmpCounter.UpdateSourceRange(range - DamageRange);

                    }
                }

                lastUpdate = LastTickTime + NextUpdate;
            }

            base.OnTick();
        }
	}
}
