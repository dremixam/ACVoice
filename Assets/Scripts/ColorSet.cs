using UnityEngine;

class ColorSet
{
    Color _TextColor;
    Color _BackgroundColor;

    public ColorSet(Color textColor, Color backgroundColor)
    {
        _TextColor = textColor;
        _BackgroundColor = backgroundColor;
    }

    public Color BackgroundColor { get => _BackgroundColor; set => _BackgroundColor = value; }
    public Color TextColor { get => _TextColor; set => _TextColor = value; }

    public static ColorSet GetColorSet(string name)
    {
        Random.InitState(name.GetHashCode());

        float hue;
        float saturation;
        float value;
        Color textColor;
        Color backgroundColor;
        do
        {
            hue = Random.value;
        } while (hue > 0.15f && hue < 0.5f);

        saturation = Mathf.Lerp(0.3f, 0.7f, Random.value);

        if (Random.value > 0.5f)
        {
            value = Mathf.Lerp(0.1f, 0.4f, Random.value);
            textColor = Color.white;
        }
        else
        {
            value = Mathf.Lerp(0.6f, 0.9f, Random.value);
            textColor = Color.HSVToRGB(0.0f, 0.6f, 0.3f);
        }
        backgroundColor = Color.HSVToRGB(hue, saturation, value);

        return new ColorSet(textColor, backgroundColor);
    }
}

