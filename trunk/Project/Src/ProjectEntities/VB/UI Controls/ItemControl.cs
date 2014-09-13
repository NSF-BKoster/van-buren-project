using System;
using System.Collections.Generic;
using System.Text;
using Engine.UISystem;
using System.ComponentModel;
using System.Drawing.Design;
using Engine;
using Engine.Renderer;
using Engine.Utils;
using ProjectCommon;
using ProjectEntities;

namespace Engine.UISystem
{
    public class ItemControl : Control
    {
        internal bool down;

        private TextBox apLabel;
        [Serialize, Browsable(false)]
        public TextBox APLabel
        {
            get
            {
                return apLabel;
            }
            set
            {
                if (apLabel != null)
                    Controls.Remove(apLabel);

                apLabel = value;
                Controls.Add(apLabel);
            }
        }

        private TextBox modeLabel;
        [Serialize, Browsable(false)]
        public TextBox ModeLabel
        {
            get
            {
                return modeLabel;
            }
            set
            {
                if (apLabel != null)
                    Controls.Remove(modeLabel);

                modeLabel = value;
                Controls.Add(modeLabel);
            }
        }

        private Control icon;
        [Serialize, Browsable(false)]
        public Control Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if (icon != null)
                    Controls.Remove(icon);

                icon = value;
                Controls.Add(icon);
            }
        }

        private BarPanel consBar;
        [Serialize, Browsable(false)]
        public BarPanel ConsBar
        {
            get
            {
                return consBar;
            }
            set
            {
                if (consBar != null)
                    Controls.Remove(consBar);

                consBar = value;
                Controls.Add(consBar);
            }
        }

        private Button itmSwitch;
        [Serialize, Browsable(false)]
        public Button ItemSwitch
        {
            get
            {
                return itmSwitch;
            }
            set
            {
                if (itmSwitch != null)
                    itmSwitch.SetShouldDetach();

                itmSwitch = value;
                Controls.Add(itmSwitch);

                itmSwitch.Click += delegate
                {
                    MainVBHUD p = Parent as MainVBHUD;
                    if (p != null && p.selectedUnit != null)
                    {
                        p.selectedUnit.PrimaryActive ^= true;
                    }
                };
            }
        }

        protected override Control.StandardChildSlotItem[] OnGetStandardChildSlots()
        {
            return new Control.StandardChildSlotItem[] { new Control.StandardChildSlotItem("APLabel", APLabel),
            new Control.StandardChildSlotItem("ModeLabel", ModeLabel),
            new Control.StandardChildSlotItem("Icon", Icon),
            new Control.StandardChildSlotItem("ConsBar", ConsBar),
            new Control.StandardChildSlotItem("ItemSwitch", ItemSwitch),
            };
        }

        [Serialize]
        [Category("Item Control")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public Texture UpTexture { get; set; }

        [Serialize]
        [Category("Item Control")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public Texture DownTexture { get; set; }

        [Serialize]
        [Category("Item Control")]
        [Editor(typeof(EditorSoundUITypeEditor), typeof(UITypeEditor))]
        public string LeftDownSound { get; set; }

        [Serialize]
        [Category("Item Control")]
        [Editor(typeof(EditorSoundUITypeEditor), typeof(UITypeEditor))]
        public string RightDownSound { get; set; }
       
        protected override void OnAttach()
        {
            base.OnAttach();

            down = false;
            BackTexture = UpTexture;

            MouseDown += new MouseButtonDelegate(EItemControl_MouseDown);
            MouseUp += new MouseButtonDelegate(EItemControl_MouseUp);
        }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            MainVBHUD p = Parent as MainVBHUD;
            if (p != null && p.selectedUnit != null)
            {
                VBItem objItem = p.selectedUnit.ActiveHeldItem;
                if (objItem != null)
                {
                    if (objItem.Type.InvIcon != null)
                        if (objItem.Type.ActIcon != null)
                            SetIcon(objItem.Type.ActIcon);
                        else
                            SetIcon(objItem.Type.InvIcon);
                    else
                        SetIcon(null);

                    MultipleActionItem maObjItem = objItem as MultipleActionItem;
                    if (maObjItem != null)
                    {
                        ModeLabel.Text = maObjItem.GetCurActionMode().UseText;
                        APLabel.Text = "AP " + maObjItem.GetCurActionMode().ActionPoints.ToString();

                        ConsumableItem consObjItem = maObjItem as ConsumableItem;
                        if (consObjItem != null)
                        {
                            ConsBar.Visible = true;
                            ConsBar.UpdateAPTextures(consObjItem.Juice);
                        }
                        else
                            ConsBar.Visible = false;
                    }
                    else
                    {
                        ModeLabel.Text = APLabel.Text = string.Empty;
                    }
                }
            }
        }

        public void SetIcon(string path)
        {
            if (path != null && Icon != null)
                Icon.BackTexture = TextureManager.Instance.Load(path, Texture.Type.Type2D, 0);
            else
                Icon.BackTexture = null;
        }

        void EItemControl_MouseUp(Control sender, EMouseButtons button)
        {
            if (down == true)
            {
                down = false;
                BackTexture = UpTexture;
            }
        }
        
        void EItemControl_MouseDown(Control sender, EMouseButtons button)
        {
            if (button == EMouseButtons.Left)
                GetControlManager().PlaySound(LeftDownSound);
            else if (button == EMouseButtons.Right)
                GetControlManager().PlaySound(RightDownSound);

            ExecuteSharedEvents();

             MainVBHUD p = Parent as MainVBHUD;
             if (p == null || p.selectedUnit == null)
                 return;

            if (button == EMouseButtons.Left)
            {
                p.selectedUnit.ActiveHeldItem.ItemClick();
                /*
                if (GetSPPlayer().ActiveHeldItem as TimedExplosiveItem != null)
                {
                    ETimeSpanWindow tmpWindow = (ETimeSpanWindow)ControlDeclarationManager.Instance.CreateControl("Gui\\TimeSpanWindow.gui");
                    tmpWindow.OnDetachEvent += delegate(ETimeSpanWindow tmpSender)
                    {
                        if (tmpWindow.NumValue.TotalSeconds != 0)
                        {
                            (GetSPPlayer().DropItem(GetSPPlayer().GetCurItem) as TimedExplosiveItem).DetonateIn((float)tmpWindow.NumValue.TotalSeconds);
                            Log.Info("You set the timer.");
                        }
                    };
                    ScreenControlManager.Instance.Controls.Add(tmpWindow);
                    return;
                }

                GetSPPlayer().ActiveHeldItem.ItemClick();*/
            }
            else if (button == EMouseButtons.Right)
            {
                MultipleActionItem maTmp = p.selectedUnit.ActiveHeldItem as MultipleActionItem;
                if (maTmp != null)
                    maTmp.IncActMode();
            }
        }

        void ExecuteSharedEvents()
        {
            down = true;
            BackTexture = DownTexture;
        }
    }
}
