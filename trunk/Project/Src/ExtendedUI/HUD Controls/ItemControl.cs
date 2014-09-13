using System;
using System.Collections.Generic;
using System.Text;
using Engine.UISystem;
using System.ComponentModel;
using System.Drawing.Design;
using Engine;
using Engine.Renderer;
using Engine.Utils;

namespace Engine.UISystem
{
    public class ItemControl : Control
    {
        internal bool down;

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

        #region APLabelDeclaration
        public TextBox aptxt = new TextBox();
        [Serialize]
        [Category("AP Label")]
        public string APText
        {
            get { return aptxt.Text; }
            set { aptxt.Text = value; }
        }

        [Serialize]
        [Category("AP Label")]
        public HorizontalAlign APHorizontal
        {
            get { return aptxt.HorizontalAlign; }
            set { aptxt.HorizontalAlign = value; }
        }

        [Serialize]
        [Category("AP Label")]
        public VerticalAlign APVertical
        {
            get { return aptxt.VerticalAlign; }
            set { aptxt.VerticalAlign = value; }
        }

        [Serialize]
        [Category("AP Label")]
        public ScaleValue APPosition
        {
            get { return aptxt.Position; }
            set { aptxt.Position = value; }
        }
        #endregion
        #region ModeLabelDeclaration
        public TextBox modetxt = new TextBox();
        [Serialize]
        [Category("Mode Label")]
        public override string Text
        {
            get { return modetxt.Text; }
            set { modetxt.Text = value; }
        }

        [Serialize]
        [Category("Mode Label")]
        public HorizontalAlign ModeHorizontal
        {
            get { return modetxt.HorizontalAlign; }
            set { modetxt.HorizontalAlign = value; }
        }

        [Serialize]
        [Category("Mode Label")]
        public VerticalAlign ModeVertical
        {
            get { return modetxt.VerticalAlign; }
            set { modetxt.VerticalAlign = value; }
        }

        [Serialize]
        [Category("Mode Label")]
        public ScaleValue ModePosition
        {
            get { return modetxt.Position; }
            set { modetxt.Position = value; }
        }
        #endregion
        #region IconDeclaration
        Control bg = new Control();
        public void SetIcon(string path)
        {
            if (path != null)
                bg.BackTexture = TextureManager.Instance.Load(path, Texture.Type.Type2D, 0);
            else
                bg.BackTexture = null;
        }
        #endregion

        [Serialize]
        [Category("Item Control")]
        public Font LabelsFont
        {
            get { return aptxt.Font; }
            set
            {
                aptxt.Font = modetxt.Font = value;
            }
        }
        protected override void OnAttach()
        {
            base.OnAttach();

            down = false;
            BackTexture = UpTexture;

            modetxt.Position = ModePosition;
            aptxt.Position = APPosition;
            modetxt.AutoSize = aptxt.AutoSize = true;

            bg.HorizontalAlign = HorizontalAlign.Center;
            bg.VerticalAlign = VerticalAlign.Center;

            Controls.Add(bg);
            Controls.Add(aptxt);
            Controls.Add(modetxt);

            MouseDown += new MouseButtonDelegate(EItemControl_MouseDown);
            MouseUp += new MouseButtonDelegate(EItemControl_MouseUp);
        }

       /* protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            if (GetSPPlayer() == null)
                return;

            Item objItem = GetSPPlayer().ActiveHeldItem;
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
                    modetxt.Text = maObjItem.GetCurActionMode().UseText;
                    aptxt.Text = "AP " + maObjItem.GetCurActionMode().ActionPoints.ToString();

                    ConsumableItem consObjItem = maObjItem as ConsumableItem;
                    if (consObjItem != null)
                    {
                        Controls["itemJuice"].Visible = true;
                        (Controls["itemJuice"] as ECombatPanel).UpdateAPTextures(consObjItem.Juice);
                    }
                    else
                        Controls["itemJuice"].Visible = false;
                }
                else
                {
                    modetxt.Text = aptxt.Text = string.Empty;
                }
            }
        }*/

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
            /*
            if (button == EMouseButtons.Left)
                
                
                GameEngineApp.Instance.ControlManager.PlaySound(LeftDownSound);
            else if (button == EMouseButtons.Right)
                ScreenControlManager.Instance.PlaySound(RightDownSound);

            ExecuteSharedEvents();

            /*if (button == EMouseButtons.Left)
            {
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

                GetSPPlayer().ActiveHeldItem.ItemClick();
            }
            else if (button == EMouseButtons.Right)
            {
                MultipleActionItem maTmp = GetSPPlayer().ActiveHeldItem as MultipleActionItem;
                if (maTmp != null)
                    maTmp.IncActMode();
            }*/
        }

        void ExecuteSharedEvents()
        {
            down = true;
            BackTexture = DownTexture;
        }
    }
}
