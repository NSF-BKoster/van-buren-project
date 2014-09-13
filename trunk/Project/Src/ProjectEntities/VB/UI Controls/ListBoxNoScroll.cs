using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.Renderer;
using Engine.MathEx;
using System.ComponentModel;
using Engine.EntitySystem;
using System.Drawing.Design;
using Engine.Utils;
using ProjectEntities;

namespace ProjectCommon
{
    public class ListBoxNoScroll : Control
    {
        internal int page;
        
        public ListBox.ListBoxItemCollection Items
        {
            get
            {
                if (listBox != null)
                    return listBox.Items;

                return null;
            }
            set
            {
                if (listBox != null)
                {
                    listBox.Items.Clear();
                    foreach (object o in value)
                    {
                        listBox.Items.Add(o);
                        object button = listBox.ItemButton.Clone();
                        (button as Button).Text = "More";
                        listBox.Items.Add(button);
                    }
                }
            }
        }


        private ListBox listBox;
        [Serialize, Browsable(false)]
        public ListBox ListBox
        { 
            get 
            { 
                return listBox; 
            } 
            set 
            {
                if (listBox != null)
                    Controls.Remove(listBox);

                listBox = value;
                Controls.Add(listBox);
            } 
        }

        protected override Control.StandardChildSlotItem[] OnGetStandardChildSlots()
        {
            return new Control.StandardChildSlotItem[] { new Control.StandardChildSlotItem("ListBox", ListBox) };
        }


        
    }
}
