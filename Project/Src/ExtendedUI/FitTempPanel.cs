// Copyright (C) 2006-2010 NeoAxis Group Ltd.
using System;
using Engine.MathEx;
using Engine.Renderer;
using Engine.UISystem;
using System.ComponentModel;
using Engine.SoundSystem;

public class EFitTempPanel : ETempPanel
{
    [Serialize]
    public Vec2 PosOffset
    {get; set;}


    protected override void OnRender()
    {
        base.OnRender();
        Size = new ScaleValue(ScaleType.Screen, GetFittedSize());
    }

    Vec2 GetFittedSize()
    {
        Vec2 maxSize = PosOffset;

        foreach (EControl c in Controls)
            maxSize += c.Position.Value + c.Size.Value;

        return maxSize;
    }
}
