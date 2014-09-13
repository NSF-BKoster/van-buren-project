//
using Engine;
using Engine.UISystem;
using Engine.Utils;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

namespace ProjectCommon
{
    public class ModuleButton : Button
    {
        [Serialize]
        [Category("Modules")]
        [Editor(typeof(EditorGuiUITypeEditor), typeof(UITypeEditor))]
        public string Module { get; set; }
    }

	public class ModuleWindow : Control
	{
        private Control mod;
        [Serialize, Browsable(false)]
        public Control Module
        {
            get
            {
                return mod;
            }
            set
            {
                if (mod != null)
                    Controls.Remove(mod);

                mod = value;
                Controls.Add(mod);
            }
        }

        protected override Control.StandardChildSlotItem[] OnGetStandardChildSlots()
        {
            return new Control.StandardChildSlotItem[] { new Control.StandardChildSlotItem("Module", Module) };
        }

		protected override void OnAttach()
		{
			base.OnAttach();

            if (mod != null)
                SetMainButtonEvents(this);
		}

        void SetMainButtonEvents(Control c)
        {
            foreach (Control control in c.Controls)
            {
                ModuleButton cbutton = control as ModuleButton;
                if (cbutton != null && !string.IsNullOrEmpty(cbutton.Module))
                {
                    cbutton.Click += delegate(Button sender)
                    {
                        SetModule(cbutton.Module);
                    };
                }
            }
        }

        Control SetModule(string guiPath)
        {
            mod.Controls.Clear();
            Control newControl = ControlDeclarationManager.Instance.CreateControl(guiPath);
            mod.Controls.Add(newControl);

            return newControl;
        }

        protected override bool OnKeyDown(KeyEvent e)
        {
            if (e.Key == EKeys.Escape)
            {
                SetShouldDetach();
                return true;
            }

            return base.OnKeyDown(e);
        }
	}
}
