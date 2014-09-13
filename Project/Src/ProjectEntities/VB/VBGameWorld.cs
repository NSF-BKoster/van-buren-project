// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using Engine;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.MathEx;
using Engine.PhysicsSystem;
using Engine.Renderer;
using Engine.Networking;
using ProjectCommon;
using Engine.FileSystem;
using System.IO;

namespace ProjectEntities
{
    public class VBGameWorldType : GameWorldType
	{
	}

    public class VBGameWorld : GameWorld
	{
        public DateTime gameTime = new DateTime(2253, 10, 23,0,0,0);

		static VBGameWorld instance;

		//

		VBGameWorldType _type = null; public new VBGameWorldType Type { get { return _type; } }

		public VBGameWorld()
		{
			instance = this;
		}

		public static new VBGameWorld Instance
		{
			get { return instance; }
		}

        protected override void OnSave(TextBlock block)
        {
            block.SetAttribute("GameTime", gameTime.ToString());
            base.OnSave(block);
        }

        protected override bool OnLoad(TextBlock block)
        {
            gameTime = DateTime.Parse(block.GetAttribute("GameTime"));
            return base.OnLoad(block);
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();

			instance = null;
		}

        //save current map
        public void SaveMap(string filePath, bool currentMap)
        {
            if (MapSaveAble())
            {
                if (currentMap)
                    MapSystemWorld.WorldSave(filePath +" //cursave.world", true);
                else
                {
                    string saveName = string.Format("{0}Maps//{1}.world", filePath, Path.GetFileNameWithoutExtension(Map.Instance.SourceMapVirtualFileName));
                    MapSystemWorld.MapSave(saveName, true);
                }
            }
        }

        public bool MapSaveAble()
        {
            return (Map.Instance != null && !string.IsNullOrEmpty(Map.Instance.Name));
        }
	}
}
