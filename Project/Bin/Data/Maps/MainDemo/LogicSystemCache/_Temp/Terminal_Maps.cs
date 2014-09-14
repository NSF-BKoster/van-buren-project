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
	public class Terminal_Maps : Engine.EntitySystem.LogicSystem.LogicEntityObject
	{
		ProjectEntities.Terminal __ownerEntity;
		
		public Terminal_Maps( ProjectEntities.Terminal ownerEntity )
			: base( ownerEntity )
		{
			this.__ownerEntity = ownerEntity;
			ownerEntity.PostCreated += delegate( Engine.EntitySystem.Entity __entity, bool loaded ) { if( Engine.EntitySystem.LogicSystemManager.Instance != null )PostCreated( loaded ); };
		}
		
		public ProjectEntities.Terminal Owner
		{
			get { return __ownerEntity; }
		}
		
		public Engine.UISystem.ListBox listBox;
		
		public void PostCreated( bool loaded )
		{
			listBox = (ListBox)Owner.MainControl.Controls[ "List" ];
			
			//maps listBox
			{
				string selectMapName = "Maps\\VillageDemo\\Map\\Map.map";
				if(!string.IsNullOrEmpty(GameWorld.Instance.NeedChangeMapName) && 
					GameWorld.Instance.NeedChangeMapSpawnPointName == "Teleporter_Maps")
				{
					if(!string.IsNullOrEmpty(GameWorld.Instance.NeedChangeMapPreviousMapName))
						selectMapName = GameWorld.Instance.NeedChangeMapPreviousMapName;
			//		selectMapName = GameWorld.Instance.ShouldChangeMapName;
				}
				
				string[] mapList = VirtualDirectory.GetFiles( "", "*.map", System.IO.SearchOption.AllDirectories );	
				foreach( string name in mapList )
				{
					listBox.Items.Add( name );
					//if( string.Compare( name.Replace( '/', '\\' ), Map.Instance.VirtualFileName.Replace( '/', '\\' ), true ) == 0 )
					if( string.Compare( name.Replace( '/', '\\' ), selectMapName.Replace( '/', '\\' ), true ) == 0 )
						listBox.SelectedIndex = listBox.Items.Count - 1;
				}
			
				listBox.SelectedIndexChange += listBox_SelectedIndexChanged;
				if( listBox.Items.Count != 0 && listBox.SelectedIndex == -1 )
					listBox.SelectedIndex = 0;
				if( listBox.Items.Count != 0 )
					listBox_SelectedIndexChanged( null );
			}
			
		}

		public void listBox_SelectedIndexChanged( object sender )
		{
			Texture texture = null;
			string description = "";
			
			ListBox listBox = (ListBox)Owner.MainControl.Controls[ "List" ];
			if( listBox.SelectedIndex != -1 )
			{
				string mapName = (string)listBox.SelectedItem;
				string mapDirectory = System.IO.Path.GetDirectoryName( mapName );
			
				//get texture
				string textureName = mapDirectory + "\\Description\\Preview";
				string textureFileName = null;
				string[] extensions = new string[] { "dds", "tga", "png", "jpg" };
				foreach( string extension in extensions )
				{
					string fileName = textureName + "." + extension;
					if( VirtualFile.Exists( fileName ) )
					{
						textureFileName = fileName;
						break;
					}
				}
				if( textureFileName != null )
					texture = TextureManager.Instance.Load( textureFileName );
			
				//get description text
				string descriptionFileName = mapDirectory + "\\Description\\Description.config";
				if( VirtualFile.Exists( descriptionFileName ) )
				{
					string error;
					TextBlock block = TextBlockUtils.LoadFromVirtualFile( descriptionFileName, out error );
					if( block != null )
						description = block.GetAttribute( "description" );
				}
				
				ActivateTeleporter(mapName);
			}
			
			Owner.MainControl.Controls[ "Preview" ].BackTexture = texture;
			Owner.MainControl.Controls[ "Description" ].Text = description;
			
		}

		public void ActivateTeleporter( string mapName )
		{
			Teleporter teleporter = (Teleporter)Entities.Instance.GetByName("Teleporter_Maps");
			teleporter.Active = true;
			teleporter.ChangeMapName = mapName;
			
			
		}

	}
}
