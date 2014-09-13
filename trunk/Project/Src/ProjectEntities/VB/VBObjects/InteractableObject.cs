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

namespace ProjectEntities
{
    public class InteractableObjectType : DynamicType
    {
    }

	public class InteractableObject : Dynamic
	{
        InteractableObjectType _type = null; public new InteractableObjectType Type { get { return _type; } }

        public delegate void OnInteractDelegate(Dynamic activator);
        public delegate void OnDeathDelegate(MapObject prejudicial);
        [LogicSystemBrowsable(true)]
        public event InteractableObject.OnInteractDelegate OnInteract;
        [LogicSystemBrowsable(true)]
        public event InteractableObject.OnDeathDelegate OnDeath;


        protected override void OnDie(MapObject prejudicial)
        {
            if (OnDeath != null) OnDeath(prejudicial);
            base.OnDie(prejudicial);
        }

        public virtual bool Interact(Dynamic activator)
        {
            if (OnInteract != null)OnInteract(activator);


            return true;
        }
    }
}
