// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using Engine.EntitySystem;

namespace ProjectEntities
{
    public class VBFactionType : FactionType
	{
        [FieldSerialize]
        List<VBFactionType> naturalEnemies = new List<VBFactionType>();

        public List<VBFactionType> NaturalEnemies
        {
            get { return naturalEnemies; }
            set { naturalEnemies = value; }
        }
	}

    public class VBFaction : Faction
	{
        VBFactionType _type = null; public new VBFactionType Type { get { return _type; } }
	}
}
