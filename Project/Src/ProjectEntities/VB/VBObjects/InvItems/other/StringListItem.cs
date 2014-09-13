 // Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine;

namespace ProjectEntities
{
	public class StringListItemType : MultipleActionItemType
    {
        [FieldSerialize]
        string[] stringList;

        public string[] StringList
        {
            get { return stringList; }
            set { stringList = value; }
        }
    }

    public class StringListItem : MultipleActionItem
    {
        StringListItemType _type = null; public new StringListItemType Type { get { return _type; } }

        public string GetRandomString()
        {
            int index = new Random().Next(0, Type.StringList.Length);
            return Type.StringList[index];
        }

        public override void ItemClick()
        {
           Log.Info(GetRandomString());
        }
    }
}