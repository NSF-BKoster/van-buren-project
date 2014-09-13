// Copyright (C) 2006-2010 NeoAxis Group Ltd.
using System;
using Engine.MathEx;
using Engine.Renderer;
using Engine.UISystem;
using System.ComponentModel;
using Engine.SoundSystem;

    public class EBubbleChat : TextBox
    {
        [Serialize]
        [Category("TempPanel")]
        public float UpdateDelay
        { get; set; }

        internal float NextUpdate;
        public Dynamic entity;


        protected override void OnAttach()
        {
            base.OnAttach();

            if (UpdateDelay != 0)
                NextUpdate = Time + UpdateDelay;
        }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            if (NextUpdate != 0 && Time > NextUpdate)
                SetShouldDetach();

            if (entity != null)
                Position = new ScaleValue(ScaleType.Screen, RendererWorld.Instance.DefaultCamera.ProjectToScreenCoordinates(entity.Position));
        }
    }
