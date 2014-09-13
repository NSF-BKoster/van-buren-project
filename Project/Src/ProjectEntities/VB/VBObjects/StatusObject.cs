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

namespace ProjectEntities
{
    public class StatusObjectType : InteractableObjectType
    {
        [FieldSerialize]
        public string disabledMessage;

        [FieldSerialize]
        public string enableMessage;


    }

	/// <summary>
    /// OBJECT WHICH REQUIRES A CERTAIN SKILL OR ITEM IN THE INVENTORY TO UNLOCK
	/// </summary>
    public class StatusObject : InteractableObject
	{
        [FieldSerialize]
        string enableType = null;

        //can be either a skill or an item type.
        public string EnableType
        {
            get { return enableType; }
            set { enableType = value; }
        }

        [FieldSerialize]
        string enableSkill = null;

        //can be either a skill or an item type.
        public string EnableSkill
        {
            get { return enableSkill; }
            set { enableSkill = value; }
        }

        [FieldSerialize]
        bool disabled = false;

        public delegate void OnStatusChangedDelegate(bool status);
        [LogicSystemBrowsable(true)]
        public event StatusObject.OnStatusChangedDelegate OnStatusChanged;

        public virtual bool Disabled
        {
            get { return disabled; }
            
            set 
            { 
                disabled = value;
                if (OnStatusChanged != null) OnStatusChanged(disabled);
            }
        }

        public bool IsDisabled()
        {
            GetLuaScript.Push(disabled);
            return disabled;
        }

        //
        StatusObjectType _type = null; public new StatusObjectType Type { get { return _type; } }

        public override bool Interact(Dynamic activator)
        {
            if (disabled)
            {
                VBCharacter act_char = activator as VBCharacter;
                if (act_char != null)
                    AttemptUnlock(act_char);
            }

            return base.Interact(activator);
        }


        public void AttemptUnlock(VBCharacter activator)
        {
            //try have an item to do it
            if (UnlockByItem(activator))
                return;

            if (UnlockBySkill(activator))
                return;

            //notify locked
            if (!string.IsNullOrEmpty(Type.disabledMessage))
                Log.Info(Type.disabledMessage);
        }

        bool UnlockByItem(VBCharacter activator)
        {
            if (string.IsNullOrEmpty(enableType))
                return false;

            if (activator.GetCurItem != null && activator.GetCurItem.ItemType.Name == enableType)
            {
                Disable();
                return true;
            }

            return false;
        }

        bool UnlockBySkill(VBCharacter activator)
        {
            if (string.IsNullOrEmpty(enableSkill))
                return false;

            string[] commandData = enableSkill.Split('$');
            if (commandData.Length < 2)
                return false;

            //if im a player but my skillqueue isnt this return false
            VBCharacter plr = (activator as VBCharacter);
            if (plr != null && plr.SkillQueue != commandData[0])
                return false;

            int skill = activator.GetCharStat(commandData[0]);
            if (skill != -1 && skill >= Convert.ToInt32(commandData[1]))
                Disable();
            else
                Log.Info("Failed to unlock using {0}. Level {1} required", commandData[0], commandData[1]);

            return true;
        }


        public void Disable()
        {
            Disabled = false;
            if (!string.IsNullOrEmpty(Type.enableMessage)) Log.Info(Type.enableMessage);
        }
    }
}
