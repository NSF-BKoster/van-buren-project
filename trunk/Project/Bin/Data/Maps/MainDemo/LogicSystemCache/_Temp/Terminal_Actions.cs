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
	public class Terminal_Actions : Engine.EntitySystem.LogicSystem.LogicEntityObject
	{
		ProjectEntities.Terminal __ownerEntity;
		
		public Terminal_Actions( ProjectEntities.Terminal ownerEntity )
			: base( ownerEntity )
		{
			this.__ownerEntity = ownerEntity;
			ownerEntity.PostCreated += delegate( Engine.EntitySystem.Entity __entity, bool loaded ) { if( Engine.EntitySystem.LogicSystemManager.Instance != null )PostCreated( loaded ); };
		}
		
		public ProjectEntities.Terminal Owner
		{
			get { return __ownerEntity; }
		}
		
		
		public void PostCreated( bool loaded )
		{
			Button button;
			
			//TeleportZombie button
			button = Owner.MainControl.Controls[ "TeleportZombie" ] as Button;
			if( button != null )
			{
				button.Click += delegate( Button sender )
				{
					Teleporter teleporter = (Teleporter)Entities.Instance.GetByName( "Teleporter_InsideMap1" );
					if( teleporter != null )
					{
						MapObject unit = (MapObject)Entities.Instance.Create( "Zombie", Map.Instance );
						unit.Position = new Vec3( 1000, 1000, 0 );
						unit.PostCreate();			
						teleporter.ReceiveObject( unit, null );
					}
				};
			}
			
			//TeleportBug button
			button = Owner.MainControl.Controls[ "TeleportBug" ] as Button;
			if( button != null )
			{
				button.Click += delegate( Button sender )
				{
					Teleporter teleporter = (Teleporter)Entities.Instance.GetByName( "Teleporter_InsideMap1" );
					if( teleporter != null )
					{
						MapObject unit = (MapObject)Entities.Instance.Create( "Bug", Map.Instance );
						unit.Position = new Vec3( 1000, 1000, 0 );
						unit.PostCreate();			
						teleporter.ReceiveObject( unit, null );
					}
				};
			}
		}

	}
}
