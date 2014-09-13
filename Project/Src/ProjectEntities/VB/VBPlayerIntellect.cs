// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine;
using Engine.MathEx;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using ProjectCommon;
using System.IO;

namespace ProjectEntities
{
	/// <summary>
	/// Defines the <see cref="RTSUnitAI"/> entity type.
	/// </summary>
    public class VBPlayerUnitIntellectType : RTSUnitAIType
	{
	}

	public class VBPlayerUnitIntellect : RTSUnitAI
	{
        //in networking mode each client will have different instance. Reference to the his intellect.
        static VBPlayerUnitIntellect instance;

        [FieldSerialize]
        List<Objective> objectiveManager = new List<Objective>();

        enum NetworkMessages
        {
            SetInstanceToClient,
            MainNotActiveUnitToClient,

            ControlKeyPressToServer,
            ControlKeyReleaseToServer,
            TurnToPositionToServer,

            ChangeMainControlledUnitToServer,
            RestoreMainControlledUnitToServer,
        }

		///////////////////////////////////////////
		VBPlayerUnitIntellectType _type = null; public new VBPlayerUnitIntellectType Type { get { return _type; } }

        public static VBPlayerUnitIntellect Instance
        {
            get { return instance; }
        }

        public void ConstructControlledUnits(SpawnPoint sp)
        {
            foreach (ControlledObjectInfo obj in controlledUnits)
            {
                PlayerCharacter unit = (PlayerCharacter)Entities.Instance.Create("VBGirl", Map.Instance);
                unit.Intellect = this;
                unit.AllowSave = false;

                obj.ApplyInfo(unit);
                unit.PostCreate();
                sp.InformUnitSpawned(unit);

                if (ControlledObject == null)
                    ControlledObject = unit;
            }
        }

        public static void SetInstance(VBPlayerUnitIntellect instance)
        {
            if (VBPlayerUnitIntellect.instance != null && instance != null)
                Log.Fatal("PlayerIntellect: SetInstance: Instance already initialized.");

            if (VBPlayerUnitIntellect.instance != null)
            {
                //This entity will accept commands of the player
                if (GameControlsManager.Instance != null)
                {
                    GameControlsManager.Instance.GameControlsEvent -=
                        VBPlayerUnitIntellect.instance.GameControlsManager_GameControlsEvent;
                }
            }

            VBPlayerUnitIntellect.instance = instance;

            if (VBPlayerUnitIntellect.instance != null)
            {
                //This entity will accept commands of the player
                if (GameControlsManager.Instance != null)
                {
                    GameControlsManager.Instance.GameControlsEvent +=
                        VBPlayerUnitIntellect.instance.GameControlsManager_GameControlsEvent;
                }
            }
        }

        public List<Objective> ObjectiveManager
        {
            get { return objectiveManager; }
            set { objectiveManager = value; }
        }

        public int ObjectiveStatus(string typeName)
        {
            Objective obj = ObjectiveManager.Find(delegate(Objective t) { return t.missionFile == typeName; });

            if (obj != null)
                return obj.GetStatus();

            return -1;
        }

        public void SetObjectiveStatus(string typeName, int status)
        {
            Objective obj = ObjectiveManager.Find(delegate(Objective t) { return t.missionFile == typeName; });

            if (obj != null)
                obj.SetStatus(status);
        }

        //OBJECTIVE MANAGER
        public void AttemptAddObjective(string missionFile)
        {
            //if its active return
            if (ObjectiveStatus(missionFile) != -1)
                return;

            //file not valid, return
            if (!File.Exists(string.Format("Lua//Missions//{0}.lua", missionFile)))
                return;

            Objective newObj = (Objective)Entities.Instance.Create("Objective", PlayerIntellect.Instance);
            newObj.SetMissionFile(missionFile);
            newObj.PostCreate();
            ObjectiveManager.Add(newObj);
            Log.Info("Mission '{0}' accepted.", newObj.ObjectiveTitle());
        }

	}
}
