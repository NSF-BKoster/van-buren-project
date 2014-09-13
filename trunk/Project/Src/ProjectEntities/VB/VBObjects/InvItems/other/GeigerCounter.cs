 // Copyright (C) 2006-2009 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine.UISystem;
using Engine.SoundSystem;
using Engine;

namespace ProjectEntities
{
    public class GeigerCounterType : VBItemType
    {
       
    }

    public class GeigerCounter : VBItem
    {
        [FieldSerialize]
        bool enabled = true;

        float m_flRange = -1;
        float m_flLastRangeUpdate;
        float m_flNextUpdate;

        GeigerCounterType _type = null; public new GeigerCounterType Type { get { return _type; } }

        public void UpdateSourceRange(float m_flrange)
        {
            m_flRange = m_flrange;
            m_flLastRangeUpdate = LastTickTime;
        }

        protected override void OnTick()
        {
            base.OnTick();

            if (enabled && m_flRange != -1 && LastTickTime > m_flNextUpdate) 
            {
               Sound sound = SoundWorld.Instance.SoundCreate("Sounds//items/geiger" + new Random().Next(1, 4).ToString() + ".wav", 0);
               if (sound != null)
                    SoundWorld.Instance.SoundPlay(sound, EngineApp.Instance.DefaultSoundChannelGroup, 0.5f);

               m_flNextUpdate = LastTickTime + sound.Length + (m_flRange / 10);

               if (LastTickTime - m_flLastRangeUpdate > 1)
                   m_flRange = -1;
            }

        }

        public override void ItemClick()
        {
            enabled ^= true;
            Log.Info("Geiger active: {0}", enabled);
        }
    }
}