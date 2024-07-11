using System;
using System.Globalization;

[Serializable]
public struct Image : IComparable<Image>
{
    public Image(string name, float dominantHue, float averageHue)
    {
        Name = name;
        DominantHue = dominantHue;
        AverageHue = averageHue;
    }
    
    public Image(string name, string dominantHue, string averageHue)
    {
        Name = name;
   
        if (!float.TryParse(dominantHue, NumberStyles.Float, CultureInfo.InvariantCulture, out DominantHue))
        {
            DominantHue = -1;
        }
    
        if (!float.TryParse(averageHue, NumberStyles.Float, CultureInfo.InvariantCulture, out AverageHue))
        {
            AverageHue = -1;
        }
    }

    public string Name;
    public float DominantHue;
    public float AverageHue;

    public int CompareTo(Image other)
    {
        var dominantHueComparison = DominantHue.CompareTo(other.DominantHue);
        if (dominantHueComparison != 0) return dominantHueComparison;
        return AverageHue.CompareTo(other.AverageHue);
    }
}