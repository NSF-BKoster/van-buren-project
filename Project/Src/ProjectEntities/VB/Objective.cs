// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine.EntitySystem;
using Engine;
using Engine.SoundSystem;
using Engine.MapSystem;
using Engine.MathEx;
using Engine.UISystem;
using LuaInterface;
using System.Reflection;
using ConversationalAPI;

namespace ProjectEntities
{
    public class ObjectiveType : EntityType
    {
    }

    public class Objective : Entity
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

        ObjectiveType _type = null; public new ObjectiveType Type { get { return _type; } }

        public Objective()
        {
        }

        public void SetMissionFile(string missionfile)
        {
            missionFile = missionfile;
            LogEntries.Add(0);
        }

        protected override void OnPostCreate(bool loaded)
        {
            if (!string.IsNullOrEmpty(missionFile))
            {
                luaScript = new Lua();
                luaScript.DoFile(string.Format("Lua//Missions//{0}.lua", missionFile));

                foreach (MethodInfo m in GetType().GetMethods())
                    luaScript.RegisterFunction(m.Name, this, m);
            }

            LuaFunction luaF = luaScript.GetFunction("OnInit");
            if (luaF != null)
                luaF.Call(this, Parent);

            base.OnPostCreate(loaded);
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
