// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Engine.EntitySystem;
using Engine;
using Engine.MathEx;
using Engine.PhysicsSystem;
using Engine.MapSystem;
using Engine.Renderer;
using Engine.Utils;
using Engine.FileSystem;

namespace ProjectEntities
{
    public class VBCharacterType : RTSCharacterType
	{
        [FieldSerialize]
        int apMax;

        public Dictionary<string, int> defaultStatistics;

        public int ActionPtsMax
        {
            get { return apMax; }
            set { apMax = value; }
        }

        [FieldSerialize]
        List<string> meleeAttacks = new List<string>();

        public List<string> MeleeAttacks
        {
            get { return meleeAttacks; }
        }

        [FieldSerialize]
        List<string> defaultInventory = new List<string>();

        public List<string> DefaultInventory
        {
            get { return defaultInventory; }
            set { defaultInventory = value; }
        }

        protected override bool OnLoad(TextBlock block)
        {
            TextBlock stats = block.FindChild("defaultStatistics");
            if (stats != null)
            {
                defaultStatistics = new Dictionary<string, int>();
                foreach (TextBlock.Attribute att in stats.Attributes)
                    defaultStatistics.Add(att.Name, Convert.ToInt32(att.Value));
            }

            return base.OnLoad(block);
        }
	}

    public class VBCharacter : RTSCharacter, GridBasedNavigationSystem.IOverrideObjectBehavior
	{
        //temporary variables
        VBItem activeHeldItem;
        Vec3 lastPosCheck;

        MapObjectAttachedMapObject activeHeldItemAttachedObject;
        Dictionary<string, int> primaryStatistics;

        [FieldSerialize]
        int apts;

        [FieldSerialize]
        [DefaultValue(true)]
        bool primaryActive;

        [FieldSerialize]
        int[] activeItems = new int[3];

        [Browsable(false)]
        public int ActionPts
        {
            get { return apts; }
            set
            {
                if (value > 0)
                {
                    if (value > GetMaxActionPoints)
                        apts = GetMaxActionPoints;
                    else
                        apts = value;
                }
                else
                {
                    apts = 0;
                }
            }
        }

        [Browsable(false)]
        public virtual int GetMaxActionPoints
        {
            get 
            {
                if (Type.ActionPtsMax > 0)
                    return Type.ActionPtsMax;


                return GetCharStat("GetAP");
            }
        }

        public override int GetMaxWeight()
        {
            if (Type.MaxWeight > 0)
                return Type.MaxWeight;

            return GetCharStat("GetCarryWeight");
        }

        public override float MaxHealth()
        {
            if (Type.HealthMax > 0)
                return Type.HealthMax;

            return GetCharStat("GetHP");
        }

        [Browsable(false)]
        public InventoryObjectItem GetCurItem
        {
            get
            {
                if (primaryActive)
                    return GetItem(activeItems[0] - 1);
                else
                    return GetItem(activeItems[1] - 1);
            }
        }

        [Browsable(false)]
        public VBItem ActiveHeldItem
        {
            get { return activeHeldItem; }
        }

        [Browsable(false)]
        public InventoryObjectItem GetItem(int index)
        {
            if (index < 0)
                return null;

            return Inventory[index];
        }

        public Dictionary<string, int> PrimaryStatistics
        {
            get { return primaryStatistics; }
            set { primaryStatistics = value; }
        }

        public Dictionary<string, int> PrimaryOrTypeStatistics
        {
            get
            {
                if (primaryStatistics != null)
                    return primaryStatistics;
                else
                    return Type.defaultStatistics;
            }

        }

        [Browsable(false)]
        public bool PrimaryActive
        {
            get { return primaryActive; }
            set 
            { 
                primaryActive = value;
                UpdateCurrentItem();
            }
        }

        public int[] ActiveItems
        {
            get { return activeItems;  }
            set { activeItems = value; }
        }

        string skillQueue;


        [Browsable(false)]
        public new VBUnitAI Intellect
        {
            get { return (VBUnitAI)base.Intellect; }
        
        }

        [Browsable(false)]
        public string SkillQueue
        {
            get { return skillQueue; }
            set
            {
                skillQueue = value;

                if (!string.IsNullOrEmpty(value))
                    (Intellect as RTSUnitAI).DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.PreUse),false);
            }
        }

        [FieldSerialize]
        VBFactionType initialFaction;

        [LogicSystemBrowsable(true)]
        public new VBFactionType InitialFaction
        {
            get { return initialFaction; }
            set
            {initialFaction = value;}
        }

		VBCharacterType _type = null; public new VBCharacterType Type { get { return _type; } }

        public List<MapObject> ObjectsInPath(MapObject startPos, MapObject endPos)
        {
            List<MapObject> objects = new List<MapObject>();

            Ray ray = new Ray(startPos.Position, endPos.Position - startPos.Position);
            if (ray.Direction != Vec3.Zero)
            {
                RayCastResult[] piercingResult = PhysicsWorld.Instance.RayCastPiercing(ray, (int)ContactGroup.CastOnlyContact);

                foreach (RayCastResult result in piercingResult)
                {
                    MapObject mapObject = MapSystemWorld.GetMapObjectByBody(result.Shape.Body);

                    if (mapObject != null && mapObject != startPos && mapObject != endPos)
                        objects.Add(mapObject);
                }
            }

            return objects;
        }

        public void BaseAttack(Dynamic ent, string skill)
        {
            foreach (MapObject m in ObjectsInPath(this, ent))
                Log.Info("Objects in path {0}", m.Name);

            MultipleActionItem mTmp = activeHeldItem as MultipleActionItem;
            if (mTmp != null && mTmp.Use(ent))
            {
                //remove action points, play sound, apply damage
                Intellect.IncActionPts(-mTmp.GetCurActionMode().ActionPoints);
                ent.DoDamage(this, Position, null, GetNetDamage(ent), false);
                SoundPlay3D(mTmp.GetCurActionMode().PlaySound, .5f, true);

                //TODO: do attack anim
                AnimationTree tree = GetFirstAnimationTree();
                if (tree != null) tree.ActivateTrigger("walkDance");
            }
        }

        public void AttemptAddObjective(string missionFile)
        {
            VBFactionManager.Instance.GetFactionItemByType(InitialFaction).ObjectiveManager.AttemptAddObjective(missionFile);
        }

        public bool CanOpenInventory()
        {
            if (Intellect.OutsideCombatOrActive())
            {
                if (ActionPts > 1)
                {
                    Intellect.IncActionPts(-2);
                }
                else
                {
                    Log.Info("Not enough action points");
                    return false;
                }
            }

            return true;
        }

        public InventoryObjectItem SetBestWeapon()
        {
            foreach (InventoryObjectItem i in Inventory)
            {
                if ((i.ItemType as VBWeaponItemType) != null)
                {
                    SetItem(i, true);

                    //try a reload
                    VBWeaponItem itm = ActiveHeldItem as VBWeaponItem;
                    if (itm != null) itm.TryReload();

                    return i;
                }
            }

            return null;
        }

        void UpdateWeaponInfo()
        {
            ConsumableItem consItm = activeHeldItem as ConsumableItem;

            //update the inv obj entry with the new info from this item
            if (consItm != null)
            {
                //remember the last selected action we had
                GetCurItem.ActionMode = consItm.ActionMode;
                GetCurItem.Juice = consItm.Juice;
            }
        }

        protected override void OnPostCreate(bool loaded)
        {
            UpdateCurrentItem();
            base.OnPostCreate(loaded);
        }

        public void CreateActiveItem(string itmType)
        {
            //shared item creation
            activeHeldItem = (VBItem)Entities.Instance.Create(itmType, Parent);
            activeHeldItem.Owner = this;
            activeHeldItem.PostCreate();
        }

        public virtual void UpdateCurrentItem()
        {
            if (EntitySystemWorld.Instance.WorldSimulationType == WorldSimulationTypes.Editor)
                return;

            //handle my current object if any
            if (activeHeldItem != null)
            {
                if (activeHeldItemAttachedObject != null)
                {
                    Detach(activeHeldItemAttachedObject);
                    activeHeldItemAttachedObject = null;
                }

                activeHeldItem.SetForDeletion(false);
                activeHeldItem = null;

                /* TODO: ONLINE ITEM SUPPORT - TO DO WHEN PORTING
                 * if (EntitySystemWorld.Instance.IsServer())
                    Server_SendSetactiveHeldItemToClients(EntitySystemWorld.Instance.RemoteEntityWorlds);*/
            }

            if (GetCurItem != null)
            {
                CreateActiveItem(GetCurItem.ItemType.Name);

                //activeHeldItem.Server_EnableSynchronizationPositionsToClients = false;

                //transfer the info from the itm to the newly created gun
                ConsumableItem consItm = activeHeldItem as ConsumableItem;
                if (consItm != null)
                {
                    consItm.Juice = GetCurItem.Juice;
                    consItm.ActionMode = GetCurItem.ActionMode;

                    VBWeaponItem wpnItm = consItm as VBWeaponItem;
                    if (wpnItm != null)
                        wpnItm.AmmoTypeLoaded = GetCurItem.AmmoType;
                }

                //TODO: PREVENT THE OBJECT ATTACHMENT BUG
                //CreateactiveHeldItemAttachedObject();
                /* ONLINE ITEM SUPPORT - TO DO WHEN PORTING
                 * if (EntitySystemWorld.Instance.IsServer())
                    Server_SendSetactiveHeldItemToClients(EntitySystemWorld.Instance.RemoteEntityWorlds);*/
            }
            else
            {
                if (Type.MeleeAttacks != null)
                {
                    if (primaryActive)
                        CreateActiveItem(Type.MeleeAttacks[0]);
                    else
                        CreateActiveItem(Type.MeleeAttacks[1]);

                }
            }
        }

        public virtual void SetItem(InventoryObjectItem itm, bool bPrimary)
        {
            UpdateWeaponInfo();

            if (bPrimary)
                activeItems[0] = Inventory.IndexOf(itm)+1;
            else
                activeItems[1] = Inventory.IndexOf(itm)+1;

            UpdateCurrentItem();
        }
        public void ResetSlot(bool bPrimary)
        {
            UpdateWeaponInfo();

            if (bPrimary)
                activeItems[0] = 0;
            else
                activeItems[1] = 0;

            UpdateCurrentItem();
        }

        public void SetArmor(InventoryObjectItem itm)
        {
            activeItems[2] = Inventory.IndexOf(itm);

            DestroyPhysicsModel();
            //TODO: SET NEW ARMOR FUNCTION - SHOULD THIS USE ATTACHMENTS OR NEW PLAYER MODEL??
        }

        public override MapObjectCreateObjectCollection.CreateObjectsResultItem[] DieObjects_Create()
        {
            MapObjectCreateObjectCollection.CreateObjectsResultItem[] result = base.DieObjects_Create();

            //populate the inventory
            foreach (MapObjectCreateObjectCollection.CreateObjectsResultItem item in result)
            {
                MapObjectCreateMapObject createMapObject = item.Source as MapObjectCreateMapObject;
                if (createMapObject != null)
                {
                    foreach (MapObject mapObject in item.CreatedObjects)
                    {
                        Corpse invObj = mapObject as Corpse;
                        if (invObj != null && InitialFaction != null)
                            invObj.Inventory = Inventory;
                    }
                }
            }

            return result;
        }

        public virtual float GetNetDamage(Dynamic target)
        {
            float RD = (ActiveHeldItem as MultipleActionItem).GetDamage(); //= random damage value produced from weapons hit damage range
            float RB = 0; //= ranged bonus (RB=0 unless the player has Bonus Ranged Damage perk)
            float CM = 2; //= critical hit damage multiplier (if no critical hit then CM=2, otherwise assigned value from critical hit table)
            float ADR = 0; // armor damage resistance value
            float ADT = 0; // armor damage threshold value
            float X = 0;
            float Y = 0;
            float RM = 0; // = ammo resistance modifier (only value allowed to be negative or positive in the equation)
            float CD = 75; // = combat difficulty multiplier (Easy=75, Normal=100, Hard=125)


            VBWeaponItem wpn = ActiveHeldItem as VBWeaponItem;
            if (wpn != null && wpn.AmmoTypeLoaded != null)
            {
                RM = wpn.AmmoTypeLoaded.DmgResistance;
                X = wpn.AmmoTypeLoaded.DmgModifier.Minimum; // = ammo dividend
                Y = wpn.AmmoTypeLoaded.DmgModifier.Maximum; // = ammo dividend
            }

            /*/his armor, not mine!
            RTSCharacter opponent = target as RTSCharacter;
            if (opponent != null && opponent.armor != null)
            {
                ADR = opponent.armor.Type.Resistance;
                ADT = opponent.armor.Type.Threshold;
            }*/

            return (((RD + RB) * (X * CM) / Y / 2.0f * CD / 100.0f) - ADT) * ((100.0f - (ADR + RM)) / 100.0f);
        }

        public InventoryObjectItem GetAmmoFor(InventoryObjectItem wpn)
        {
            VBWeaponItemType asWeapon = wpn.ItemType as VBWeaponItemType;
            if (asWeapon == null)
                return null;

            foreach (InventoryObject.InventoryObjectItem itm in Inventory)
            {
                AmmoItemType amType = itm.ItemType as AmmoItemType;
                if (amType != null && asWeapon.UsableAmmoList.Contains(amType))
                    return itm;
            }

            return null;
        }

        //------------------------------------------------------------------------------------------------------
        //STATISTICS MANIPULATION
        //------------------------------------------------------------------------------------------------------
        public int GetCharStat(string s)
        {
            int val = -1;
            PrimaryOrTypeStatistics.TryGetValue(s, out val);

            //its not a default stat, try derived
            if (val < 1)
            {
                if (GetLuaScript != null)
                    val = Convert.ToInt32(GetLuaScript.GetFunction(s).Call(this).GetValue(0));
            }

            return val;
        }

        public bool IncreaseStat(string str, string statneeded)
        {
            int tmp = GetCharStat(statneeded);
            if (tmp < 1) return false;

            SetStat(statneeded, tmp - 1);
            SetStat(str, GetCharStat(str) + 1);
            return true;
        }

        public bool DecreaseStat(string str, string statneeded)
        {
            int tmp = GetCharStat(str);

            if (tmp <= 1)
                return false;

            SetStat(statneeded, GetCharStat(statneeded) + 1);
            SetStat(str, GetCharStat(str) - 1);
            return true;
        }

        public void SetStat(string str, int val)
        {
            if (primaryStatistics == Type.defaultStatistics)
                primaryStatistics = new Dictionary<string, int>(Type.defaultStatistics);

            primaryStatistics[str] = val;
        }

        public virtual void GainExperience(int exp)
        {
            SetStat("experience", PrimaryOrTypeStatistics["experience"]++);

            //TODO: LEVEL PER EXP GAINED INCREMENTATION
            Log.Info("{0} received {1} experience points.", Name, exp);
        }

        public virtual void LevelUp()
        {
            SetStat("level", PrimaryOrTypeStatistics["level"]++);
            //ScreenControlManager.Instance.PlaySound("Sounds//LevelUp.wav");
            Log.Info("{0} went up a level.", Name);
        }

        protected override void OnRenderFrame()
        {
            //update animation tree
            if (EntitySystemWorld.Instance.Simulation && !EntitySystemWorld.Instance.SystemPauseOfSimulation)
            {
                AnimationTree tree = GetFirstAnimationTree();
                if (tree != null)
                    UpdateAnimationTree(tree);
            }

            base.OnRenderFrame();
        }

        protected override void OnTick()
        {
            if (Intellect.InCombatAndActive())
            {
                //REMOVE APS
                if ((lastPosCheck - Position).Normalize() > 0.8)
                {
                    Intellect.IncActionPts(-1);
                    lastPosCheck = Position;
                }
            }

            base.OnTick();
        }

        protected override void OnDie(MapObject prejudicial)
        {
            //TODO: GENDER BASED DEATH SOUNDS
            SoundPlay3D("VB//Sounds//HMXXXXBD.ogg", 0.5f, true);

            //play kill anim
            AnimationTree tree = GetFirstAnimationTree();
            if (tree != null) tree.ActivateTrigger("death");


             base.OnDie(prejudicial);
        }


        public override void UpdateAnimationTree(AnimationTree tree)
        {
            bool move = false;
            //Degree moveAngle = 0;
            float moveSpeed = 0;

            if (MainBodyVelocity.ToVec2().Length() > .1f)
            {
                move = true;
                moveSpeed = (Rotation.GetInverse() * MainBodyVelocity).X;
            }

            tree.SetParameterValue("idle", move ? 0 : 1);
            tree.SetParameterValue("move", move ? 1 : 0);
            tree.SetParameterValue("run", move && IsRunning ? 1 : 0);

            if (!IsRunning && moveSpeed > 1.5f)
                moveSpeed = 1.5f;

            tree.SetParameterValue("moveSpeed", moveSpeed);
        }
	}
}
