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
using Engine.FileSystem;
using Engine.UISystem;
using Engine.Renderer;
using System.IO;
using NLua;

namespace ProjectEntities
{
    public class LuaObjectType : MapObjectType
    {
    }

	public class LuaObject : MapObject
	{
        LuaObjectType _type = null; public new LuaObjectType Type { get { return _type; } }

        [FieldSerialize]
        string scriptLogic;

        public virtual string ScriptLogic
        {
            get { return scriptLogic; }
            set { scriptLogic = value; }
        }

        Lua luaScript;

        [Browsable(false)]
        public Lua GetLuaScript
        {
            get { return luaScript; }
            set { luaScript = value; }
        }

        public object GetParentEntity()
        {
            luaScript.Push(this);
            return this;
        }

        /*public void UnlockAchievement(string name)
        {
            EControl prompt = ControlDeclarationManager.Instance.CreateControl("Gui\\pipboy\\achievement.gui");

            if (prompt != null)
            {
                string error;
                TextBlock block = TextBlockUtils.LoadFromRealFile("Data\\Definitions\\achievements.config", out error);
                if (block != null)
                {
                    foreach (TextBlock b in block.Children)
                    {
                        if (b.Name == name)
                        {
                            prompt.Controls["title"].Text = b.GetAttribute("title");
                            prompt.Controls["image"].BackTexture = TextureManager.Instance.Load(b.GetAttribute("image"), Texture.Type.Type2D, 0);
                            ScreenControlManager.Instance.Controls.Add(prompt);
                            ScreenControlManager.Instance.PlaySound("Sounds\\achievement.wav");
                            return;
                        }
                    }
                }
            }
        }*/

        public object GetProperty(object entity, string property)
        {
            object ret = entity.GetType().GetProperty(property).GetValue(entity, null);

            luaScript.Push(ret);
            return ret;
        }

        public void SetProperty(object ent, string property, object value)
        {
            ent.GetType().GetProperty(property).SetValue(ent, value, null);
        }

        public virtual void RegisterLuaFunctions()
        {
            //REGISTER ALL OF THE THINGS
            foreach (MethodInfo m in GetType().GetMethods())
                luaScript.RegisterFunction(m.Name, this, m);
        }

        public void PrintText(object obj)
        {
            Log.Info(obj.ToString());
        }

        protected override void OnPostCreate(bool loaded)
        {
            if (!string.IsNullOrEmpty(scriptLogic))
            {
                luaScript = new Lua();
                RegisterLuaFunctions();

                luaScript.DoFile(scriptLogic);

                LuaFunction luaF = luaScript.GetFunction("OnInit");
                if (luaF != null)
                    luaF.Call(this);
            }

            base.OnPostCreate(loaded);
        }

        protected override void OnDestroy()
        {
            if (luaScript != null)
                luaScript.Dispose();

            base.OnDestroy();
        }

    }
}
