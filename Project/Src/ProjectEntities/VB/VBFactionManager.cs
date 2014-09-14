// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using Engine;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.Utils;
using System.IO;
using System.Reflection;
using ConversationalAPI;
using NLua;

namespace ProjectEntities
{
	public class VBFactionManagerType : MapGeneralObjectType
	{
        public VBFactionManagerType()
		{
			UniqueEntityInstance = true;
			AllowEmptyName = true;
		}
	}

	public class VBFactionManager : MapGeneralObject
	{
		static VBFactionManager instance;

		[FieldSerialize]
		List<FactionItem> factions = new List<FactionItem>();

		///////////////////////////////////////////
        public class ObjectiveManager
        {
            [FieldSerialize]
            public List<Objective> Objectives = new List<Objective>();

            public void AttemptAddObjective(string missionFile)
            {
                //if its active return
                if (ObjectiveStatus(missionFile) != -1)
                    return;

                //file not valid, return
                if (!File.Exists(string.Format("Lua//Missions//{0}.lua", missionFile)))
                    return;

                Objective newObj = new Objective(missionFile);
                Objectives.Add(newObj);
                Log.Info("Mission '{0}' accepted.", newObj.ObjectiveTitle());
            }

            public int ObjectiveStatus(string typeName)
            {
                Objective obj = Objectives.Find(delegate(Objective t) { return t.missionFile == typeName; });

                if (obj != null)
                    return obj.GetStatus();

                return -1;
            }

            public void SetObjectiveStatus(string typeName, int status)
            {
                Objective obj = Objectives.Find(delegate(Objective t) { return t.missionFile == typeName; });

                if (obj != null)
                    obj.SetStatus(status);
            }

            public class Objective
            {
                Lua luaScript = null;

                [FieldSerialize]
                public string missionFile;

                [FieldSerialize]
                List<int> logEntries = new List<int>();

                public List<int> LogEntries
                {
                    get { return logEntries; }
                    set { logEntries = value; }
                }

                [FieldSerialize]
                int objStatus = 0;

                public delegate void MissionEndDelegate();
                [LogicSystemBrowsable(true)]
                public event Objective.MissionEndDelegate OnMissionEnd;

                public delegate void StatusChangedDelegate(int val);
                [LogicSystemBrowsable(true)]
                public event Objective.StatusChangedDelegate OnStatusChanged;

                public Objective(string missionfile)
                {
                    missionFile = missionfile;
                    logEntries.Add(0);

                    if (!string.IsNullOrEmpty(missionFile))
                    {
                        luaScript = new Lua();
                        luaScript.DoFile(string.Format("Lua//Missions//{0}.lua", missionFile));

                        foreach (MethodInfo m in GetType().GetMethods())
                            luaScript.RegisterFunction(m.Name, this, m);
                    }

                    LuaFunction luaF = luaScript.GetFunction("OnInit");
                    if (luaF != null)
                        luaF.Call(this);
                }

                public void SetMissionFile(string missionfile)
                {
                    missionFile = missionfile;
                    LogEntries.Add(0);
                }

                public void PrintText(object obj)
                {
                    Log.Info(obj.ToString());
                }

                public int DataBaseIndex()
                {
                    return Convert.ToInt32(luaScript.GetFunction("GetDBIndex").Call().GetValue(0));
                }

                public string ObjectiveTitle()
                {
                    return Conversational.Instance.GetBotConversationByID("missions", DataBaseIndex()).Say;
                }

                public int GetStatus()
                {
                    return objStatus;
                }

                public void SetStatus(int status)
                {
                    LogEntries.Add(status);
                    objStatus = status;
                    if (OnStatusChanged != null)
                        OnStatusChanged(status);
                }

                public void EndMission()
                {
                    if (OnMissionEnd != null)
                        OnMissionEnd();

                    //((PlayerCharacter)PlayerIntellect.Instance.ControlledObject).GainExperience(Convert.ToInt32(luaScript.GetFunction("GetExp").Call().GetValue(0)));
                }
            }
        }

		public class FactionItem
		{
			[FieldSerialize]
			VBFactionType factionType;

			[FieldSerialize]
            List<VBFactionType> enemies = new List<VBFactionType>();

            [FieldSerialize]
            ObjectiveManager objManager;

			//

			public VBFactionType FactionType
			{
				get { return factionType; }
				set { factionType = value; }
			}

            public List<VBFactionType> Enemies
			{
				get 
                { return enemies; }
				set { enemies = value; }
			}

            public ObjectiveManager ObjectiveManager
            {
                get { return objManager; }
                set { objManager = value; }
            }

            public VBFactionType GetFaction()
            {
               return (VBFactionType)EntityTypes.Instance.GetByName(factionType.Name);
            }

			public override string ToString()
			{
				if( FactionType == null )
					return "(not initialized)";
				return FactionType.FullName;
			}
		}

		///////////////////////////////////////////

		VBFactionManagerType _type = null; public new VBFactionManagerType Type { get { return _type; } }

		public static VBFactionManager Instance
		{
			get { return instance; }
		}

		public VBFactionManager()
		{
			if( instance != null )
				Log.Fatal( "RTSFactionManager: instance != null" );
			instance = this;
		}

		/// <summary>
		/// Don't modify
		/// </summary>
		[TypeConverter( typeof( CollectionTypeConverter ) )]
		[Editor( "ProjectEntities.Editor.RTSFactionManager_FactionsCollectionEditor, ProjectEntities.Editor", typeof( UITypeEditor ) )]
		public List<FactionItem> Factions
		{
			get { return factions; }
		}

        protected override void OnCreate()
        {
            base.OnCreate();

            //get all factions. if i encountered a new faction, add it
            Map.Instance.GetObjects(Map.Instance.InitialCollisionBounds, MapObjectSceneGraphGroups.UnitGroupMask, delegate(MapObject mapObject)
            {
                Unit u = mapObject as Unit;

                if (u.InitialFaction != null)
                {
                    FactionItem fact = GetFactionItemByType(u.InitialFaction as VBFactionType);

                    if (u != null && u.InitialFaction != null && fact == null)
                    {
                        FactionItem f = new FactionItem();
                        f.FactionType = u.InitialFaction as VBFactionType;
                        f.ObjectiveManager = new ObjectiveManager();
                        factions.Add(f);
                    }
                }
            });
        }

		/// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnPostCreate(Boolean)"/>.</summary>
		protected override void OnPostCreate( bool loaded )
		{
			if( instance == this )//for undo support
				instance = this;
			base.OnPostCreate( loaded );
		}

		/// <summary>Overridden from <see cref="Engine.EntitySystem.Entity.OnDestroy()"/>.</summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if( instance == this )//for undo support
				instance = null;
		}

        public bool AreEnemies(VBFactionType type, VBFactionType type2)
        {
            if (type == null || type2 == null)
                return false;

            if (GetFactionItemByType(type).Enemies.Contains(type2) || type.NaturalEnemies.Contains(type2))
                return true;

            return false;
        }

        public FactionItem GetFactionItemByName(string name)
        {
            FactionType fact = (FactionType)EntityTypes.Instance.GetByName(name);
            if (fact != null)
                return GetFactionItemByType(fact);

            return null;
        }

		public FactionItem GetFactionItemByType( FactionType type )
		{
            if (type as VBFactionType == null)
                return null; 

			foreach( FactionItem item in factions )
				if( item.FactionType == type )
					return item;

			return null;
		}
	}
}
