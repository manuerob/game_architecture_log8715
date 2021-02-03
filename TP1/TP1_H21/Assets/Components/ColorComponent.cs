using UnityEngine;

public class ColorComponent : ICopiableComponent
{
    public Color Color;

    public object Clone()
    {
        return new ColorComponent { Color = Color };
    }
}

