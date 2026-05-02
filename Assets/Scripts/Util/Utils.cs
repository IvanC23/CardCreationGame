using UnityEngine;

public class Utils : MonoBehaviour
{
    public enum Colors
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    public static Colors RandomColor()
    {
        var values = System.Enum.GetValues(typeof(Colors));
        return (Colors)values.GetValue(Random.Range(0, values.Length));
    }

    public static Color ToColor(Colors color)
    {
        return color switch
        {
            Colors.Red => Color.red,
            Colors.Green => Color.green,
            Colors.Blue => Color.blue,
            Colors.Yellow => Color.yellow,
            _ => Color.white
        };
    }

    public static Colors RandomColorExcluding(Colors exclude)
    {
        var values = System.Enum.GetValues(typeof(Colors));
        Colors result;
        do
        {
            result = (Colors)values.GetValue(Random.Range(0, values.Length));
        } while (result == exclude);
        return result;
    }
}
