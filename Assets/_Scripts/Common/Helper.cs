using UnityEngine;

public class Helper : MonoBehaviour
{
    public static void SwapEmpty(Tile tile1, Tile tile2)
    {
        var tempEmpty = tile1.Empty;
        tile1.Empty = tile2.Empty;
        tile2.Empty = tempEmpty;
    }
}
