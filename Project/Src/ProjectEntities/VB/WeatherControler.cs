// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using Engine.MathEx;
using Engine.SoundSystem;
using Engine.FileSystem;

namespace ProjectEntities
{
	/// <summary>
	/// Defines the <see cref="SpawnPoint"/> entity type.
	/// </summary>
    public class WeatherControlerType : MapObjectType
	{
    
        public MapObjectAttachedObject[] weathertypes { get; set;  }

	}

    public class WeatherControler : MapObject
	{
		//


        WeatherControlerType _type = null; public new WeatherControlerType Type { get { return _type; } }

        protected override void Client_OnTick()
        {
            UpdateWeatherEffects();

            base.Client_OnTick();
        }

        public void UpdateWeatherEffects()
        {
            
        }
	
	}
}
