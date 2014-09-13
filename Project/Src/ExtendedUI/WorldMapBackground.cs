// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using GameEntities;
using Engine.MapSystem;

namespace Game
{
    /// <summary>
    /// Defines a about us window.
    /// </summary>
    public class WmWindow : EControl
    {
        EControl window;

        EControl pDest;
        EControl pPlayer;

        int travel_speed = 10;

        protected override void OnAttach()
        {
            base.OnAttach();
            window = ControlDeclarationManager.Instance.CreateControl("Gui\\world_map.gui");

            pPlayer = window.Controls["player"];
            //restore position
            pDest = window.Controls["dest"];
             Controls.Add(window);
        }

       /* void AttemptToLoadMap()
        {
            for (int n = 1; ; n++)
            {
            ETextBox curMap = (ETextBox)window.Controls["location" + n.ToString()];

                if (curMap == null)
                break;

                if (window.Controls["player"].GetScreenRectangle().IsIntersectsRect(curMap.GetScreenRectangle()))
                {
                    GameEngineApp.Instance.LoadVBMap("Maps//" + curMap.Name + ".map", "location1");
                    SetShouldDetach();
                    return;
                }
            }

            int i = new Random().Next(1, 9);
            string randomMap = "Wasteland" + i.ToString();
            GameEngineApp.Instance.SetNeedMapLoad("Maps//" + randomMap + ".map");
        }

        protected override bool OnMouseDown(EMouseButtons button)
        {
            //If atop openly any window to not process
            if (Controls.Count != 1)
                return base.OnMouseDown(button);

            if (button == EMouseButtons.Left && window.Controls[ "map_bg" ].GetScreenRectangle().IsContainsPoint(MousePosition) )
            {
                if( window.Controls[ "player" ].GetScreenRectangle().IsContainsPoint( MousePosition ) )
                    AttemptToLoadMap();
                
                pDest.Visible ^= true;
                pDest.Position = new ScaleValue(ScaleType.Screen, MousePosition);
                return true;
            }
            return base.OnMouseDown(button);
        }

        protected override void OnTick(float delta)
        {
            if (pDest.Visible == true)
            {
                Vec2 dir = Vec2.Subtract(pDest.Position.Value, pPlayer.Position.Value);
                dir = dir.GetNormalize();

                float pSpeed = (delta/travel_speed);
                float x = pPlayer.Position.Value.X;
                float y = pPlayer.Position.Value.Y;

                
                if (MapValue(y) != MapValue(pDest.Position.Value.Y))
                    y += dir.Y * pSpeed;
                

                
                if (MapValue(x) != MapValue(pDest.Position.Value.X))
                    x += dir.X * pSpeed;
               

                pPlayer.Position = new ScaleValue(ScaleType.Screen, new Vec2(x, y));

                /*if (center player pos == center dest pos )
                    pDest.Visible = false;*
               }
            base.OnTick(delta);
        }

        int MapValue(float val)
        {
            return (int)(val*1000);
        }

        protected override bool OnKeyDown(KeyEvent e)
        {
            if (base.OnKeyDown(e))
                return true;

            if (e.Key == EKeys.Escape)
            {
                SetShouldDetach();
                return true;
            }
            return false;
        }*/
    }
}
