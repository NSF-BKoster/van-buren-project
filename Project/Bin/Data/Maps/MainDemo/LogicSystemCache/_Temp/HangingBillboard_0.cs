using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.FileSystem;
using Engine.MathEx;
using Engine.Utils;
using Engine.Renderer;
using Engine.PhysicsSystem;
using Engine.SoundSystem;
using Engine.UISystem;
using Engine.EntitySystem;
using Engine.MapSystem;
using ProjectCommon;
using ProjectEntities;

namespace Maps_MainDemo_LogicSystem_LogicSystemScripts
{
	public class HangingBillboard_0 : Engine.EntitySystem.LogicSystem.LogicEntityObject
	{
		Engine.MapSystem.MapObject __ownerEntity;
		
		public HangingBillboard_0( Engine.MapSystem.MapObject ownerEntity )
			: base( ownerEntity )
		{
			this.__ownerEntity = ownerEntity;
			ownerEntity.PostCreated += delegate( Engine.EntitySystem.Entity __entity, bool loaded ) { if( Engine.EntitySystem.LogicSystemManager.Instance != null )PostCreated( loaded ); };
		}
		
		public Engine.MapSystem.MapObject Owner
		{
			get { return __ownerEntity; }
		}
		
		
		public void PostCreated( bool loaded )
		{
			((GameGuiObject)Owner).MainControl.Controls["Version"].Text = EngineVersionInformation.Version;
			
			
		}

	}
}
