// Copyright (C) 2006-2010 NeoAxis Group Ltd.
using System;
using Engine.Renderer;
using Engine.UISystem;
using System.ComponentModel;
using Engine.SoundSystem;
using System.Reflection;
using ProjectEntities;

namespace ProjectCommon
{
    public class PlayerPropertyDisplay : TextBox
    {
        [Serialize]
        [Category("PropertyOrStat")]
        public string Property
        { get; set; }

        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(Property) && !string.IsNullOrEmpty(Name))
                Property = Name;

            base.OnAttach();
        }

        protected override void OnTick(float delta)
        {
            MainVBHUD p = Parent as MainVBHUD;
            if (!string.IsNullOrEmpty(Property) && p != null && p.selectedUnit != null)
                Text = GetProperty(p.selectedUnit, Property);

            base.OnTick(delta);
        }

        string GetProperty(object src, string propName)
        {
            PropertyInfo prop = src.GetType().GetProperty(propName);
            if (prop != null)
                return prop.GetValue(src, null).ToString();

            return (src as VBCharacter).GetCharStat(Property).ToString();
        }
    }
}
