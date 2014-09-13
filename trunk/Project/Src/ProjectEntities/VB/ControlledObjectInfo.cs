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
using System.Reflection;
using ConversationalAPI;
using Engine.Utils;
using Engine.FileSystem;

namespace ProjectEntities
{
    //------------------------------------------------------------------------------------------------------
    //THIS IS A STRIPPED DOWN VERSION OF THE PLAYER CHARACTERS USED TO MOVE THEM ACROSS THE MAPS
    //------------------------------------------------------------------------------------------------------
    public class ControlledObjectInfoType : EntityType
    {}

    public class ControlledObjectInfo : Entity
    {
        public class stat
        {
            [FieldSerialize]
            public string key;

            [FieldSerialize]
            public int value;
        }

        public VBCharacter tmpUnit;

        [FieldSerialize]
        public string name;

        [FieldSerialize]
        public float life;

        [FieldSerialize]
        public List<ProjectEntities.InventoryObject.InventoryObjectItem> inventory;

        [FieldSerialize]
        public List<stat> statistics;

        ControlledObjectInfoType _type = null; public new ControlledObjectInfoType Type { get { return _type; } }

        protected override void OnSave(TextBlock block)
        {
            if (tmpUnit.Health < 1)
                SetForDeletion(false);

            life = tmpUnit.Health;
            inventory = tmpUnit.Inventory;

            if (tmpUnit.PrimaryStatistics != null && tmpUnit.PrimaryStatistics != tmpUnit.Type.defaultStatistics)
            {
                statistics = new List<stat>();

                foreach (var pair in tmpUnit.PrimaryStatistics)
                {
                    stat tmp = new stat();
                    tmp.key = pair.Key;
                    tmp.value = pair.Value;
                    statistics.Add(tmp);
                }
            }

            base.OnSave(block);
        }

        public Dictionary<string, int> ConstructStatistics()
        {
            Dictionary<string, int> tmp = new Dictionary<string, int>();

            foreach (stat s in statistics)
                tmp.Add(s.key, s.value);

            return tmp;
        }

        public void ApplyInfo(VBCharacter avatar)
        {
            tmpUnit = avatar;
            avatar.Health = life;

            if (statistics != null)
                avatar.PrimaryStatistics = ConstructStatistics();

            if (inventory != null) avatar.Inventory = inventory;
        }
    }
}
