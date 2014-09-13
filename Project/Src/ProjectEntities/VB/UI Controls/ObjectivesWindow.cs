// Copyright (C) 2006-2010 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using ConversationalAPI;
using System.IO;
using Engine.FileSystem;
using ProjectEntities;
using System.ComponentModel;

namespace ProjectCommon
{
	/// <summary>
	/// Defines a about us window.
	/// </summary>
	public class ObjectivesWindow : Control
	{
        private ListBox objList;
        [Serialize, Browsable(false)]
        public ListBox ObjList
        {
            get
            {
                return objList;
            }
            set
            {
                if (objList != null)
                    Controls.Remove(objList);

                objList = value;
                Controls.Add(objList);
            }
        }

        private ListBox objUpdates;
        [Serialize, Browsable(false)]
        public ListBox ObjUpdates
        {
            get
            {
                return objUpdates;
            }
            set
            {
                if (objUpdates != null)
                    Controls.Remove(objUpdates);

                objUpdates = value;
                Controls.Add(objUpdates);
            }
        }

        protected override Control.StandardChildSlotItem[] OnGetStandardChildSlots()
        {
            return new Control.StandardChildSlotItem[] { new Control.StandardChildSlotItem("ObjList", ObjList),
            new Control.StandardChildSlotItem("ObjUpdates", ObjUpdates),
            };
        }

        internal ProjectEntities.VBFactionManager.ObjectiveManager obj; 

        protected override void OnAttach()
        {
            base.OnAttach();

            obj = VBFactionManager.Instance.GetFactionItemByName("PlayerFaction").ObjectiveManager;


            if (obj != null && objList != null && objUpdates != null)
            {
                foreach (ProjectEntities.VBFactionManager.ObjectiveManager.Objective o in obj.Objectives)
                    objList.Items.Add(o.ObjectiveTitle());

                objList.SelectedIndexChange += new ListBox.SelectedIndexChangeDelegate(objList_SelectedIndexChange);
            }
        }

        void objList_SelectedIndexChange(ListBox sender)
        {
            objUpdates.Items.Clear();
            ProjectEntities.VBFactionManager.ObjectiveManager.Objective o = obj.Objectives[objList.SelectedIndex];

            ConversationalItem ci = Conversational.Instance.GetBotConversationByID("missions", o.DataBaseIndex());
            foreach (int index in o.LogEntries)
                objUpdates.Items.Add(ci.ConversationalResponseItems.ResponseItems[index].Response);
        }
	}
}
