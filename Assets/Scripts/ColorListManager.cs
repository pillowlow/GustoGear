using UnityEngine;
using System.Collections.Generic;


[System.Serializable]

public enum TasteType
{
    Sweet,
    Sour,
    Bitter,
    SweetSour, // Combination of sweet and sour
    SweetBitter, // Combination of sweet and bitter
    SourBitter // Combination of sour and bitter
}

[System.Serializable]
public class ColorMapping
{
    public TasteType tasteType; // Enum for taste types
    public Color color; // The color associated with the taste
}
public class ColorListManager : MonoBehaviour
{
    [SerializeField]
    private List<ColorMapping> colorMappings; // Holds all basic and combined colors.

    public static ColorListManager Instance { get; private set; } // Singleton pattern.

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Get color based on the taste type.
    /// </summary>
    /// <param name="tasteType">The taste type enum to search for.</param>
    /// <returns>The corresponding Color if found, otherwise Color.black.</returns>
    public Color GetColorByTasteType(TasteType tasteType)
    {
        foreach (var mapping in colorMappings)
        {
            if (mapping.tasteType == tasteType)
            {
                return mapping.color;
            }
        }
        Debug.LogWarning("Color not found for taste type: " + tasteType);
        return Color.black; // Return a default color if not found.
    }
}