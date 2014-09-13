using System;
//using System.Collections.Generic;
//using System.Text;
using System.ComponentModel;
//using System.Drawing.Design;
//using System.Windows.Forms;
using Engine;
using Engine.UISystem;
//using Engine.MapSystem;
//using Engine.EntitySystem;
using Engine.MathEx;
using Engine.Renderer;
//using Engine.PhysicsSystem;
//using Engine.Utils;
//using GameEntities;

namespace GameEntities
{
    /// <summary>
    /// A text control that scrolls like a ticker.
    /// Note it only scrolls when its text is larger than its size.
    /// </summary>
    public class ETicker : ETextBox
    {

        /// <summary>
        /// Increment speed in milliseconds.
        /// </summary>
        [DefaultValue(100)]
        [Serialize]
        [Category("General")]
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
             
                if (isReady)
                    UpdateTime();
            }
        }
        private int speed;

        [Serialize]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (isReady)
                    UpdateText();
            }
        }
        private string text;

        /// <summary>
        /// The ticker scrolls while this is true.
        /// </summary>
        [DefaultValue(true)]
        [Serialize]
        [Category("General")]
        public bool Scrolling
        {
            get { return scrolling; }
            set
            {
                scrolling = value;
                if (isReady)
                {
                    UpdateText();
                    UpdateTime();
                }
            }
        }
        private bool scrolling;

        private bool isReady;
        private string textDisplay;     // The text that is rendered
        private int displayStartChar;   // Scroll index
        private int displayMaxChars;
        private float nextTime;
        private Rect clip;

        private void UpdateTime()
        {
            if (scrolling)
            {
                nextTime = Time + (float)speed / 1000;
            }
            else
            {
                nextTime = 0;
            }
        }

        private void UpdateText()
        {
            text = base.Text;

            displayStartChar = 0;
            displayMaxChars = 0;
            textDisplay = "";

            if (text == string.Empty) text = "";
            if (text.Length == 0)
            {
                scrolling = false;
                return;
            }

            float textDisplayLength;
            float averageCharWidth;
            // If there is a game running, or just editor
            if (Engine.EngineConsole.Instance != null)
            {
                textDisplayLength = Engine.EngineConsole.Instance.Font.GetTextLength(text);
                averageCharWidth = textDisplayLength / (float)text.Length;
            }
            else
            {
                averageCharWidth = 0.01f;
                textDisplayLength = averageCharWidth * (float)text.Length;
            }

            float tickerLength = this.GetScreenSize().X;
            if (textDisplayLength <= tickerLength)
            {
                scrolling = false;
            }

            // This is similar to the above check.
            displayMaxChars = (int)(tickerLength / averageCharWidth);
            if (displayMaxChars >= text.Length)
            {
                scrolling = false;
            }

            //Log.Info("textDisplayLength {0}, tickerLength {1}, displayMaxChars {2}", textDisplayLength, tickerLength, displayMaxChars);
        }

        private void UpdateClip()
        {
            clip = this.GetScreenRectangle();
            clip.Top -= 0.04f;
            //clip.Left -= 0.01f;
            //clip.Bottom += 0.01f;
            clip.Right += 0.001f;
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            scrolling = true;
            isReady = true;
            UpdateTime();
            UpdateText();
            UpdateClip();
        }

        protected override void OnTick(float delta)
        {
            base.OnTick(delta);

            if (scrolling)
            {
                if (Time >= nextTime)
                {
                    displayStartChar++;
                    if (displayStartChar >= text.Length)
                    {
                        displayStartChar = 0;
                    }
                    UpdateTime();
                }
            }

            // Sets textDisplay from the current index to the end of text
            int displayCharsLeft = displayMaxChars;
            if (displayMaxChars > (text.Length - displayStartChar))
            {
                displayCharsLeft = (text.Length - displayStartChar);
            }
            textDisplay = text.Substring(displayStartChar, displayCharsLeft);

            // Wrap text around. Adds from the start of the text
            if (scrolling)
            {
                int displayCharsRight = displayMaxChars - displayCharsLeft;
                if (displayCharsRight > 0)
                {
                    if (displayCharsRight < text.Length)
                        textDisplay += text.Substring(0, displayCharsRight);
                }
            }
        }

        protected override void OnRenderUI(GuiRenderer renderer)
        {
            //base.OnRenderUI(renderer);
            // The version without clipping works when running in a game. In the editor, UpdateText set averageCharWidth to a guess!
            // With clipping it looks ok in the editor.
            renderer.AddText(textDisplay, this.GetScreenPosition());
            //renderer.AddText(textDisplay, this.GetScreenPosition(), HorizontalAlign.Left, VerticalAlign.Top, this.TextColor, clip);                
            //renderer.AddRectangle(this.GetScreenRectangle(), this.TextColor, clip);
            //Log.Info("GetScreenRectangle {0}, GetScreenPosition {1}, GetScreenSize {2}", this.GetScreenRectangle().ToString(), this.GetScreenPosition().ToString(), this.GetScreenSize().ToString());
        }

        protected override void OnResize()
        {
            base.OnResize();
            if (isReady)
            {
                UpdateText();
                UpdateClip();
            }
        }

    }
}
