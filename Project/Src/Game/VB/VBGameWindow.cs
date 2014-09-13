// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Engine;
using Engine.Renderer;
using Engine.MathEx;
using Engine.SoundSystem;
using Engine.UISystem;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using Engine.FileSystem;
using Engine.Utils;
using System.IO;
using ProjectEntities;
using ProjectCommon;

namespace Game
{
    /// <summary>
    /// Game window
    /// </summary>
    public class VBGameWindow : GameWindow
    {
        public static string CursorPath
        {
            get { return "VB\\GUI\\Cursors\\"; }
        }

        enum CameraType
        {
            Top,
            Free,

            Count,
        }

        public enum State
        {
            Target,
            Walk,
            HUDDefault,
            Wait,
            Use,
            Talk,
            Pickup,
            GUIDefault,
        }

        private State cursorState;
        public State CursorState
        {
            get { return cursorState; }
            set
            {
                cursorState = value;
                GameEngineApp.Instance.ControlManager.DefaultCursor = CursorPath + value.ToString() + ".tga";


            }
        }

        float cameraDistance = 20;
        SphereDir cameraDirection = new SphereDir(2.5f, .90f);
        Vec3 cameraPosition;

        [Config("Camera", "cameraType")]
        static CameraType cameraType;

        //MAIN HUD
        MainVBHUD hudControl;

        //units that the user CAN select
        List<VBCharacter> availableUnits = new List<VBCharacter>();

        // TEMP VARIABLES
        float newAzimuthAngle = 0;
        Dynamic lastObjectChecked = null;
        //

       /* void itmControl_MouseDown(EControl sender, EMouseButtons button)
        {
          
                    case "settimer":
                        ETimeSpanWindow tmpWindow = (ETimeSpanWindow)ControlDeclarationManager.Instance.CreateControl("Gui\\TimeSpanWindow.gui");
                            tmpWindow.OnDetachEvent += delegate(ETimeSpanWindow tmpSender)
                            {
                                if (tmpWindow.NumValue.TotalSeconds != 0)
                                {
                                    (playerCharacter.DropItem(playerCharacter.GetCurItem) as ExplosiveItem).DetonateIn((float)tmpWindow.NumValue.TotalSeconds);
                                    Log.Info("You set the timer.");
                                }
                            };
                            Controls.Add(tmpWindow);
                        break;
        }*/

        public override void OnBeforeWorldSave()
        {
            base.OnBeforeWorldSave();

            //World serialized data
            World.Instance.ClearAllCustomSerializationValues();
            World.Instance.SetCustomSerializationValue("cameraDirection", cameraDirection);
            World.Instance.SetCustomSerializationValue("cameraPosition", cameraPosition);
        }

        protected override void OnAttach()
        {
            base.OnAttach();
            EngineApp.Instance.KeysAndMouseButtonUpAll();

            //Load my game window
            hudControl = ControlDeclarationManager.Instance.CreateControl("VB\\GUI\\HUD\\VBHUD.gui") as MainVBHUD;
            /*

            //set value of item selector
            ESwitch itmSel = ((ESwitch)hudControl.Controls[0].Controls["itmSelector"]);
            itmSel.bValue = playerCharacter.SecondaryItemActive;
            itmSel.MouseDown += new MouseButtonDelegate(itmSel_MouseDown);

            //if its a new game, add my character creation window
            if (playerCharacter.GetCharStat("charpoints") > 0)
                Controls.Add(new NewGameWindow());
            */

            //ADD the control
            Controls.Add(hudControl);
            CursorState = State.HUDDefault;

            EngineConsole.Instance.AddCommand("createWindow", CC_CreateWindow);
            EngineConsole.Instance.AddCommand("addobjective",CC_ObjectiveAdd);
            /*

            FreeCameraEnabled = cameraType == CameraType.Free;

            //commands to remain in the final version
            EngineConsole.Instance.AddCommand("combat", CC_CombatCommands);
            EngineConsole.Instance.AddCommand("createWindow", CC_CreateWindow);
            EngineConsole.Instance.AddCommand("addobjective",CC_ObjectiveAdd);


            //TODO: REMOVAL OF DEBUG COMMANDS
            EngineConsole.Instance.AddCommand("showobjstat",
                ConsoleCommand_GiveItem);

            EngineConsole.Instance.AddCommand("setobjstat",
                ConsoleCommand_GiveItem2);

            EngineConsole.Instance.AddCommand("showstat",
               ConsoleCommand_GiveItem3);

            EngineConsole.Instance.AddCommand("cmbstatus",
                ConsoleCommand_CombatStatus);

            EngineConsole.Instance.AddCommand("loadmap",
                ConsoleCommand_LoadMap);

            EngineConsole.Instance.AddCommand("setstat",
                ConsoleCommand_SetStat);*/

            ResetTime();

            FocusOnPlayer();

            //render scene for loading resources
            EngineApp.Instance.RenderScene();
            EngineApp.Instance.MousePosition = new Vec2(.5f, .5f);
            ResetCam();
            EngineApp.Instance.MouseRelativeMode = false;
        }

        void FocusOnPlayer()
        {
            Map.Instance.GetObjects(Map.Instance.InitialCollisionBounds, MapObjectSceneGraphGroups.UnitGroupMask, delegate(MapObject mapObject)
                {
                    VBCharacter unit = mapObject as VBCharacter;

                    if (unit != null && unit.InitialFaction != null && unit.InitialFaction.Name == "PlayerFaction")
                        availableUnits.Add(unit);
                });

            hudControl.selectedUnit = availableUnits[0];
        }

        protected override bool OnMouseWheel(int delta)
        {
            //If atop openly any window to not process
            if (Controls.Count != 1)
            {
                return base.OnMouseWheel(delta);
            }

            // zoom in/out
            Vec2 distanceRange = new Vec2(20, 100);
            cameraDistance += delta * (distanceRange[1] - distanceRange[0]) / 1500.0f;
            if (cameraDistance < distanceRange[0])
            {
                cameraDistance = distanceRange[0];
            }
            if (cameraDistance > distanceRange[1])
            {
                cameraDistance = distanceRange[1];
            }

            return base.OnMouseWheel(delta);
        }
        

        /*
        void CC_CombatCommands(string arguments)
        {
            if (CombatManager.Instance == null | CombatManager.Instance.ActiveEntity != playerCharacter)
                return;

            switch (arguments)
            {
                case "end":
                    CombatManager.Instance.AttemptEnd();
                    break;

                case "endTurn":
                    playerCharacter.EndTurn();
                    break;

                default:
                    Log.Warning("Unknown command {0}", arguments);
                    break;
            }
        }*/

        void CC_CreateWindow(string arguments)
        {
            if (hudControl == null || hudControl.selectedUnit == null)
                return;

            RTSUnitAI intellect = hudControl.selectedUnit.Intellect as RTSUnitAI;
            if (intellect == null)
                return;

            switch (arguments)
            {
                case "inv":
                    if (hudControl.selectedUnit.CanOpenInventory())
                    {
                        if (intellect.CurrentTask.Entity != null)
                        {
                            if (!(intellect.CurrentTask.Entity as InventoryObject).Disabled)
                                Controls.Add(new ObjectInventoryWindow(hudControl.selectedUnit, intellect.CurrentTask.Entity as InventoryObject));
                            else
                                Log.Info((intellect.CurrentTask.Entity as InventoryObject).Type.disabledMessage);
                        }
                        else
                            Controls.Add(new TESTinventory(hudControl.selectedUnit));
                    }
                    break;
                    
                case "chat":
                    if (hudControl.selectedUnit != null)
                    {
                        VBCharacter partner = (hudControl.selectedUnit.Intellect as RTSUnitAI).CurrentTask.Entity as VBCharacter;

                        ChatWindow tmpchat = new ChatWindow();
                        tmpchat.SetPartners(hudControl.selectedUnit, partner);
                        Controls.Add(tmpchat);
                        tmpchat.LoadConversation(partner.Name, partner);
                        cameraPosition = partner.Position;
                    }
                    break;

                default:
                    Controls.Add(ControlDeclarationManager.Instance.CreateControl(arguments));
                    break;
            }
        }
        
        void CC_ObjectiveAdd(string arguments)
        {
            if (Map.Instance == null ||  hudControl.selectedUnit == null)
                return;

            hudControl.selectedUnit.AttemptAddObjective(arguments);
        }

       /* void ConsoleCommand_GiveItem3(string arguments)
        {
            if (Map.Instance == null)
                return;
            if (PlayerIntellect.Instance == null)
                return;

            RTSCharacter unit = PlayerIntellect.Instance.ControlledObject as RTSCharacter;
            if (unit == null)
                return;

            Log.Info(unit.GetCharStat(arguments).ToString());
        }

        void ConsoleCommand_GiveItem(string arguments)
        {
            if (Map.Instance == null)
                return;
            if (PlayerIntellect.Instance == null)
                return;

            Unit unit = PlayerIntellect.Instance.ControlledObject;
            if (unit == null)
                return;

            Log.Info(PlayerIntellect.Instance.ObjectiveStatus("mission00").ToString());
        }

        void ConsoleCommand_GiveItem2(string arguments)
        {
            if (Map.Instance == null)
                return;
            if (PlayerIntellect.Instance == null)
                return;

            Unit unit = PlayerIntellect.Instance.ControlledObject;
            if (unit == null)
                return;

            PlayerIntellect.Instance.SetObjectiveStatus(arguments[0].ToString(), Convert.ToInt32(arguments[1]));
        }

        void ConsoleCommand_CombatStatus(string arguments)
        {
            if (Map.Instance == null)
                return;
            if (PlayerIntellect.Instance == null)
                return;

            RTSCharacter unit = (RTSCharacter)PlayerIntellect.Instance.ControlledObject;
            if (unit == null)
                return;

            if (CombatManager.Instance != null)
            {
                foreach (Unit u in CombatManager.Instance.GetCombatants())
                {
                    Log.Info("Combatant {0}, ap {1}, apmax {2}, hp {3}, active {4}", u.Name, u.ActionPts, u.Type.ActionPtsMax, u.Life, CombatManager.Instance.ActiveEntity.Name);
                }

                Log.Info("{0} has the turn", unit.Name);
            }
            else
                Log.Info("combat is not enabled");
        }

        void ConsoleCommand_LoadMap(string arguments)
        {
            if (Map.Instance == null)
                return;
            if (PlayerIntellect.Instance == null)
                return;

            RTSCharacter unit = (RTSCharacter)PlayerIntellect.Instance.ControlledObject;
            if (unit == null)
                return;

            GameEngineApp.Instance.SetChangeMap(arguments);
        }

        void ConsoleCommand_SetStat(string arguments)
        {
            if (Map.Instance == null)
                return;
            if (PlayerIntellect.Instance == null)
                return;

            RTSCharacter unit = (RTSCharacter)PlayerIntellect.Instance.ControlledObject;
            if (unit == null)
                return;

            string[] args = arguments.Split(' ');

            unit.SetStat(args[0], Convert.ToInt32(args[1]));
        }*/

        /*
        void itmSel_MouseDown(Control sender, EMouseButtons button)
        {
            ((ESwitch)sender).bValue ^= true;
            playerCharacter.SecondaryItemActive = ((ESwitch)sender).bValue;
        }*/

        protected override void OnDetach()
        {
            base.OnDetach();
            ResetCursor();
        }

        public void ResetCam()
        {
            if (hudControl.selectedUnit != null) cameraPosition = hudControl.selectedUnit.Position;
            newAzimuthAngle = cameraDirection.Horizontal = 0;
        }

        protected override void OnControlAttach(Control control)
        {
            ResetCursor();

            if (control != hudControl)
                hudControl.Enable = false;

            base.OnControlAttach(control);
        }

        protected override void OnControlDetach(Control control)
        {
            UpdateCursor();

            if (control != hudControl)
                hudControl.Enable = true;

            base.OnControlDetach(control);
        }

        public void UpdateCursor()
        {
            GameEngineApp.Instance.ControlManager.DefaultCursor = CursorPath + CursorState + ".tga";
        }

        public void ResetCursor()
        {
            GameEngineApp.Instance.ControlManager.DefaultCursor = CursorPath + "GUIDefault.tga";
        }

        protected override bool OnKeyDown(KeyEvent e)
        {
            //If atop openly any window to not process
            if (Controls.Count != 1)
                return base.OnKeyDown(e);

            if (e.Key == EKeys.I)
            {
                CC_CreateWindow("inv");
                return true;
            }

            if (e.Key == EKeys.P)
            {
                Controls.Add(ControlDeclarationManager.Instance.CreateControl("VB//GUI//HUD//pipBoy.gui"));
                return true;
            }

            if (e.Key == EKeys.Escape)
            {
                Controls.Add(new MenuWindow());
                return true;
            }

            if (e.Key == EKeys.Space)
            {
                ResetCam();
                return true;
            }

            //rtsCameraDirection
            if (e.Key == EKeys.Left)
            {
                if (newAzimuthAngle == cameraDirection.Horizontal)
                    newAzimuthAngle = cameraDirection.Horizontal + MathFunctions.PI / 4;

                return true;
            }

            if (e.Key == EKeys.Right)
            {
                if (newAzimuthAngle == cameraDirection.Horizontal)
                    newAzimuthAngle = cameraDirection.Horizontal - MathFunctions.PI / 4;

                return true;
            }

            //change camera type
            if (e.Key == EKeys.F7)
            {
                cameraType = (CameraType)((int)cameraType + 1);
                if (cameraType == CameraType.Count)
                    cameraType = (CameraType)0;

                FreeCameraEnabled = cameraType == CameraType.Free;

                return true;
            }

            //GameControlsManager
            if (EntitySystemWorld.Instance.Simulation)
            {
                if (GetRealCameraType() != CameraType.Free && !IsCutSceneEnabled())
                {
                    if (GameControlsManager.Instance.DoKeyDown(e))
                        return true;
                }
            }

            return base.OnKeyDown(e);
        }

        protected override bool OnKeyUp(KeyEvent e)
        {
            //If atop openly any window to not process
            if (Controls.Count != 1)
                return base.OnKeyUp(e);

            //GameControlsManager
            GameControlsManager.Instance.DoKeyUp(e);

            return base.OnKeyUp(e);
        }

        bool IsMouseInControlArea()
        {
            if (Controls.Count != 1)
                return false;

            foreach (Control ec in hudControl.Controls)
            {
                if (ec.Visible && ec.GetScreenRectangle().IsContainsPoint(MousePosition))
                    return false;
            }

            return true;
        }
        protected override void OnMouseMove()
		{
			base.OnMouseMove();

            //If atop openly any window to not process
            if (Controls.Count != 1)
                return;

            if (IsMouseInControlArea())
            {
                if (hudControl.selectedUnit != null && (hudControl.selectedUnit.Intellect as RTSUnitAI).CurrentTask.Type == RTSUnitAI.Task.Types.PreUse)
                {
                    CursorState = State.Target;
                    return;
                }

                if (CursorState > State.Walk)
                {
                    InteractableObject curObj = GetObject() as InteractableObject;
                    if (curObj != null && curObj != hudControl.selectedUnit)
                    {
                        if (curObj != lastObjectChecked)
                        {
                            Log.Info("You see " + curObj.Type.Name);
                            lastObjectChecked = curObj;
                        }

                        if (curObj as VBCharacter != null)
                            CursorState = State.Talk;
                        else
                            CursorState = State.Use;
                    }
                    else
                        CursorState = State.HUDDefault;
                }

                UpdateCursor();
            }
            else
                ResetCursor();

            //GameControlsManager
            if (EntitySystemWorld.Instance.Simulation && EngineApp.Instance.MouseRelativeMode)
            {
                if (GetRealCameraType() != CameraType.Free && !IsCutSceneEnabled())
                {
                    Vec2 mouseOffset = MousePosition;

                    GameControlsManager.Instance.DoMouseMoveRelative(mouseOffset);
                }
            }
        }

        protected override bool OnMouseDoubleClick(EMouseButtons button)
        {
            if (IsMouseInControlArea() && button == EMouseButtons.Left && hudControl.selectedUnit != null )
            {
                RTSUnitAI intellect = hudControl.selectedUnit.Intellect as RTSUnitAI;

                if (intellect.CurrentTask.Type == RTSUnitAI.Task.Types.BreakableMove || intellect.CurrentTask.Type ==  RTSUnitAI.Task.Types.Move)
                {
                    hudControl.selectedUnit.IsRunning = true;
                    return true;
                }
            }


            return base.OnMouseDoubleClick(button);
        }

        protected override bool OnMouseDown(EMouseButtons button)
        {
            //If atop openly any window to not process
            if (Controls.Count != 1)
                return base.OnMouseDown(button);

            if (IsMouseInControlArea())
            {
                if (button == EMouseButtons.Left)
                {
                    if (hudControl.selectedUnit != null)
                    {
                        Dynamic curObj = GetObject();
                        RTSUnitAI intellect = hudControl.selectedUnit.Intellect as RTSUnitAI;

                        switch (CursorState)
                        {
                            case State.Target:
                                if (curObj != null)
                                {
                                    CursorState = State.HUDDefault;
                                    intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Attack, curObj), false);
                                }
                                break;

                            case State.Use:
                                if (curObj as Unit == null)
                                {
                                    //inventory item?
                                    intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Use, curObj), false);



                                    /*
                                        //PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Loot);
                                        intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Stop), false);
                                    else if (curObj as VBItem != null)
                                        intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.PickUp, curObj), false);
                                    else
                                        intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Stop), false);
                                        //PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Interact);
                                    */
                                }
                                break;

                            case State.HUDDefault:
                                if (curObj != null)
                                {
                                    /*PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Interact);
                                    RTSCharacter ch = curObj as RTSCharacter;
                                    if (ch != null)
                                    {
                                        string msg = "{0} has {1} hit points and is armed with {2}";
                                        Log.Info(msg + ".", ch.GetName(), (int)ch.Life, ch.ActiveHeldItem.Type.Name);
                                    }
                                   */
                                }
                                break;

                            case State.Walk:
                                intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Move, MapPos()), false);
                                //PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Move);
                                break;

                            default:
                                if (curObj != null /*&& curObj != PlayerIntellect.Instance.ControlledObject*/)
                                {
                                    VBCharacter tmpEnt = curObj as VBCharacter;
                                    if (tmpEnt != null)
                                    {
                                        intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Talk, tmpEnt), false);
                                        /*
                                        switch (tmpEnt.ConvType)
                                        {
                                            case RTSCharacter.ConversationType.BubbleChat:
                                                tmpEnt.BubbleChat("I NEVER HAD MY ONE CHILD!");
                                                break;
                                            case RTSCharacter.ConversationType.Conversation:
                                                PlayerIntellect.Instance.SetTask(tmpEnt, PlayerIntellect.TaskType.Chat);
                                                break;

                                            default:
                                                break;
                                        }*/
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        Dynamic obj = GetObject();
                        if (obj != null && obj as VBCharacter != null)
                            hudControl.selectedUnit = obj as VBCharacter;
                    }
                }
                else
                if (button == EMouseButtons.Right)
                {
                    Dynamic obj = GetObject();
                    if (obj != null && cursorState > State.Walk)
                    {
                        hudControl.Controls.Add(new ObjectMenu(obj));
                    }
                    else
                        ToggleCursor();
                }
            }

            return base.OnMouseDown(button);
        }

        public virtual void ToggleCursor()
        {
            //stop any task in progress
            if (cursorState != State.Walk && hudControl.selectedUnit != null)
            {
                RTSUnitAI intellect = hudControl.selectedUnit.Intellect as RTSUnitAI;
                intellect.DoTask(new RTSUnitAI.Task(RTSUnitAI.Task.Types.Stop), false);
            }

            switch (CursorState)
            {
                case State.Target:
                    CursorState = State.Walk;
                    break;
                case State.HUDDefault:
                    if (CombatManager.Instance != null)
                        CursorState = State.Target;
                    else
                        CursorState = State.Walk;
                    break;
                case State.Walk:
                    CursorState = State.HUDDefault;
                    break;


                default:
                    break;
            }
        }

        RTSCharacter GetCharacter()
        {
            RTSCharacter tmp = GetUnit() as RTSCharacter;
            if (tmp != null)
                return tmp;

            return null;
        }

        Unit GetUnit()
        {
            Unit tmp = GetObject() as Unit;
            if (tmp != null)
                return tmp;

            return null;
        }

        Dynamic GetObject()
        {
            Vec3 mouseMapPos = Vec3.Zero;

            //get pick information
            Ray ray = RendererWorld.Instance.DefaultCamera.GetCameraToViewportRay(EngineApp.Instance.MousePosition);
            if (!float.IsNaN(ray.Direction.X))
            {
                RayCastResult result = PhysicsWorld.Instance.RayCast(ray, (int)ContactGroup.CastOnlyContact);
                if (result.Shape != null)
                    return MapSystemWorld.GetMapObjectByBody(result.Shape.Body) as Dynamic;
            }
            return null;
        }

        Vec3 MapPos()
        {
            Vec3 mouseMapPos = Vec3.Zero;

            //get pick information
            Ray ray = RendererWorld.Instance.DefaultCamera.GetCameraToViewportRay(EngineApp.Instance.MousePosition);
            if (!float.IsNaN(ray.Direction.X))
            {
                RayCastResult result = PhysicsWorld.Instance.RayCast(ray, (int)ContactGroup.CastOnlyContact);
                return result.Position;
            }

            return Vec3.Zero;
        }

        protected override bool OnMouseUp(EMouseButtons button)
        {
            //If atop openly any window to not process
            if (Controls.Count != 1)
                return base.OnMouseUp(button);

            //GameControlsManager
            GameControlsManager.Instance.DoMouseUp(button);

            if (IsMouseInControlArea() && button == EMouseButtons.Left)
            {
                    Dynamic curObj = GetObject();

                    switch (CursorState)
                    {
                        case State.Target:
                            if (curObj != null)
                            {
                                CursorState = State.HUDDefault;
                                //PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.InteractWithItem);
                            }
                            break;

                        case State.Use:
                            if (curObj as Unit == null)
                            {
                                /*
                                if (curObj as InventoryObject != null)
                                    PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Loot);
                                else if (curObj as Item != null)
                                    PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Take);
                                else
                                    PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Interact);
                                 * */
                            }
                            break;

                        case State.HUDDefault:
                            if (curObj != null)
                            {
                                /*PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Interact);
                                RTSCharacter ch = curObj as RTSCharacter;
                                if (ch != null)
                                {
                                    string msg = "{0} has {1} hit points and is armed with {2}";
                                    Log.Info(msg + ".", ch.GetName(), (int)ch.Life, ch.ActiveHeldItem.Type.Name);
                                }
                               */
                            }
                            break;

                        case State.Walk:
                            //PlayerIntellect.Instance.SetTask(curObj, PlayerIntellect.TaskType.Move);
                            break;

                        default:
                            if (curObj != null && curObj != hudControl.selectedUnit)
                            {
                                VBCharacter tmpEnt = curObj as VBCharacter;
                                if (tmpEnt != null)
                                {
                                    /*
                                    switch (tmpEnt.ConvType)
                                    {
                                        case RTSCharacter.ConversationType.BubbleChat:
                                            tmpEnt.BubbleChat("I NEVER HAD MY ONE CHILD!");
                                            break;
                                        case RTSCharacter.ConversationType.Conversation:
                                            PlayerIntellect.Instance.SetTask(tmpEnt, PlayerIntellect.TaskType.Chat);
                                            break;

                                        default:
                                            break;
                                    }*/
                                }
                            }
                            break;
                    }
            }

            return base.OnMouseUp(button);
        }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            if (hudControl.Enable && GetRealCameraType() == CameraType.Top && !EngineConsole.Instance.Active)
            {
                //change cameraPosition
                if (Time > 2)
                {
                    //rotate vector
                    if (newAzimuthAngle != cameraDirection.Horizontal)
                    {
                        /*if (newAzimuthAngle > cameraDirection.Horizontal)
                            cameraDirection.Horizontal += angleToUpdate;
                        else
                            cameraDirection.Horizontal -= angleToUpdate;*/

                        //cameraDirection.Horizontal = delta * (cameraDirection.Horizontal - newAzimuthAngle);

                        cameraDirection.Horizontal = newAzimuthAngle;
                    }

                    //move
                    Vec2 vector = Vec2.Zero;
                    if (MousePosition.X < .005f)
                    {
                        vector.X--;
                    }
                    if (MousePosition.X > 1.0f - .005f)
                    {
                        vector.X++;
                    }
                    if (MousePosition.Y < .005f)
                    {
                        vector.Y++;
                    }
                    if (MousePosition.Y > 1.0f - .005f)
                    {
                        vector.Y--;
                    }

                    if (vector != Vec2.Zero)
                    {
                        //rotate vector
                        float angle = MathFunctions.ATan(-vector.Y, vector.X) + cameraDirection.Horizontal;
                        vector = new Vec2(MathFunctions.Sin(angle), MathFunctions.Cos(angle));
                        Vec2 result = cameraPosition.ToVec2() + vector * delta * 30;

                        if (Map.Instance.InitialCollisionBounds.IsContainsPoint(new Vec3(result.X, result.Y, 0)))
                            cameraPosition = new Vec3(result.X, result.Y, 0);
                    }
                }
            }

            //GameControlsManager
            if (EntitySystemWorld.Instance.Simulation)
            {
                if (GetRealCameraType() != CameraType.Free && !IsCutSceneEnabled())
                    GameControlsManager.Instance.DoTick(delta);
            }
        }

        CameraType GetRealCameraType()
        {
            return cameraType;
        }

        bool IsCutSceneEnabled()
        {
            return CutSceneManager.Instance != null && CutSceneManager.Instance.CutSceneEnable;
        }

        protected override void OnGetCameraTransform(out Vec3 position, out Vec3 forward, out Vec3 up, ref Degree cameraFov)
        {
            //To use data about orientation the camera if the cut scene is switched on
            if (IsCutSceneEnabled())
                if (CutSceneManager.Instance.GetCamera(out position, out forward, out up, out cameraFov))
                    return;

            Vec3 offset;
            {
                Quat rot = new Angles(0, 0, MathFunctions.RadToDeg(cameraDirection.Horizontal)).ToQuat();
                rot *= new Angles(0, MathFunctions.RadToDeg(cameraDirection.Vertical), 0).ToQuat();
                offset = rot * new Vec3(1, 0, 0);
                offset *= cameraDistance;
            }

            Vec3 lookAt = new Vec3(cameraPosition.X, cameraPosition.Y, 0);
            forward = -offset;
            position = lookAt + offset;
            up = new Vec3(0, 0, 1);
            cameraFov = 80f;
        }
    }
}

