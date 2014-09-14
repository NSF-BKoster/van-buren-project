// Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using Engine;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.Utils;
using Engine.SoundSystem;
using Engine.MathEx;
using System.Reflection;
using ProjectCommon;

namespace ProjectEntities
{
    public class MultipleActionItemType : VBItemType
    {
        [FieldSerialize]
        List<ActionMode> actionModes = new List<ActionMode>();

        public List<ActionMode> ActionModes
        {
            get { return actionModes; }
            set { actionModes = value; }
        }

        [FieldSerialize]
        string boneSlot = "";

        public string BoneSlot
        {
            get { return boneSlot; }
            set { boneSlot = value; }
        }

        public class ActionMode
        {
            [FieldSerialize]
            string useText;

            public string UseText
            {
                get { return useText; }
                set { useText = value; }
            }
			
			[FieldSerialize]
			string playSound;

			[Editor(typeof(EditorSoundUITypeEditor), typeof(UITypeEditor))]
			public string PlaySound
			{
				get { return playSound; }
				set { playSound = value; }
			}

            [FieldSerialize]
			float betweenFireTime;

            [DefaultValue(0.7f)]
            public float BetweenFireTime
			{
                get { return betweenFireTime; }
                set { betweenFireTime = value; }
			}

            [FieldSerialize]
            Range useDistanceRange;

            public Range UseDistanceRange
            {
                get 
                {
                    if (useDistanceRange.Maximum == 0)
                        useDistanceRange.Maximum = 3.5f;

                    return useDistanceRange; 
                }
                set { useDistanceRange = value; }
            }

            [FieldSerialize]
            [DefaultValue(0)]
            int actionPoints;

            [DefaultValue(0)]
            public int ActionPoints
            {
                get { return actionPoints; }
                set { actionPoints = value; }
            }

            [FieldSerialize]
            Range damage;

            [DefaultValue(0)]
            public Range Damage
            {
                get { return damage; }
                set { damage = value; }
            }

            [FieldSerialize]
            string command;

            public string Command
            {
                get { return command; }
                set { command = value; }
            }

            [FieldSerialize]
            List<string> requirements = new List<string>();

            public List<string> Requirements
            {
                get { return requirements; }
            }
        }
    }

    /// <summary>
    /// Items which can be picked up by units. Med-kits, weapons, ammunition.
    /// </summary>
    public class MultipleActionItem : VBItem
    {
        [FieldSerialize]
        [DefaultValue(0)]
        int actionMode;

        public int ActionMode
        {
            get { return actionMode; }
            set { actionMode = value; }
        }

        [FieldSerialize]
        float readyTimeRemaining;

        [Browsable(false)]
        public bool Ready
        {
            get { return readyTimeRemaining == 0; }
        }

        ///////////////////////////////////////////
        MultipleActionItemType _type = null; public new MultipleActionItemType Type { get { return _type; } }

        public virtual bool HasRequirements(MultipleActionItemType.ActionMode act)
        {
            if (Owner != null && act.Requirements != null)
            {
                foreach (string s in act.Requirements)
                {
                    //search player stats for the lavel and value
                    string[] command = s.Split('$');

                    //check if its a property first
                    PropertyInfo isproperty = GetType().GetProperty(command[0]);
                    if (isproperty != null)
                    {
                        return isproperty.GetValue(this, null).ToString() == command[1];
                    }
                    else
                    {
                        if (Owner.GetCharStat(command[0]) < Convert.ToInt32(command[1]))
                            return false;
                    }
                }
            }

            return true;
        }

        public override void ItemClick()
        {
            //run the command by default
            EngineConsole.Instance.ExecuteString(GetCurActionMode().Command);
        }

        protected override void OnTick()
        {
            base.OnTick();

            if (readyTimeRemaining > 0)
            {
                //FIXME: fire times faster or slower depending on skill level
                /*float coef = 1.0f;

                Unit unit = GetParentUnitHavingIntellect();
                if (unit != null && unit.FastAttackInfluence != null)
                    coef *= unit.FastAttackInfluence.Type.Coefficient;*/

                readyTimeRemaining -= TickDelta /* coef*/;
                if (readyTimeRemaining < 0)
                    readyTimeRemaining = 0;
            }
        }

		public override bool Use(Dynamic ent)
        {
			//TODO: check if owner exists, has enough action points and in range
            if (Owner == null)
                return false;

            if (!Ready)
                return false;

            if (Owner.Intellect.InCombatAndActive() && !Owner.Intellect.HasActionPoints(GetCurActionMode().ActionPoints))
            {
                //Log.Info("{0} tried to fire {1} but was unable as he needs {2} action points", Owner.Name,  GetCurActionMode().UseText, GetCurActionMode().ActionPoints);
                return false;
            }

            readyTimeRemaining = GetCurActionMode().BetweenFireTime;
            return true;
        }
		
        public void IncActMode()
        {
           // find next possible mode
           int actcheck = actionMode;

           do
           {
               actcheck++;

               if (actcheck == Type.ActionModes.Count)
                   actcheck = 0;
           }
           while (!HasRequirements(Type.ActionModes[actcheck]));

           actionMode = actcheck;
        }

        public virtual int GetDamage()
        {
            return new Random().Next((int)GetCurActionMode().Damage.Minimum, (int)GetCurActionMode().Damage.Maximum);
        }

        public MultipleActionItemType.ActionMode GetCurActionMode()
        {
            return Type.ActionModes[ActionMode];
        }

        public MultipleActionItemType.ActionMode GetActionMode(string command)
        {
            return Type.ActionModes.Find(act => act.Command == command);
        }
    }
}
