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
    SourBitter, // Combination of sour and bitter
    None
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


    public TasteType Mix(TasteType type1, TasteType type2)
    {
        // do mix and update color

        TasteType mixedResult = TasteType.None;

        if (type1 == type2)
        {
            mixedResult = type1; // Same types result in the same type.
        }
        else if (type1 == TasteType.None ){
            mixedResult = type2;
        }
        else if (type2 == TasteType.None){
            mixedResult = type1;
        }
        else
        {
            mixedResult = DetermineMixResult(type1, type2);
        }
        return mixedResult;

    }

    private TasteType DetermineMixResult(TasteType type1, TasteType type2)
    {
        // Define rules for mixing different tastes
        if ((type1 == TasteType.Sweet && type2 == TasteType.Sour) || (type1 == TasteType.Sour && type2 == TasteType.Sweet))
        {
            return TasteType.SweetSour;
        }
        else if ((type1 == TasteType.Sweet && type2 == TasteType.Bitter) || (type1 == TasteType.Bitter && type2 == TasteType.Sweet))
        {
            return TasteType.SweetBitter;
        }
        else if ((type1 == TasteType.Sour && type2 == TasteType.Bitter) || (type1 == TasteType.Bitter && type2 == TasteType.Sour))
        {
            return TasteType.SourBitter;
        }
         else if ((type1 == TasteType.Sour && type2 == TasteType.SweetSour) || (type1 == TasteType.SweetSour && type2 == TasteType.Sour))
        {
            return TasteType.Sour;
        }
         else if ((type1 == TasteType.Sour && type2 == TasteType.SourBitter) || (type1 == TasteType.SourBitter && type2 == TasteType.Sour))
        {
            return TasteType.Sour;
        }
         else if ((type1 == TasteType.Sweet && type2 == TasteType.SweetSour) || (type1 == TasteType.SweetSour && type2 == TasteType.Sweet))
        {
            return TasteType.Sweet;
        }
        else if ((type1 == TasteType.Sweet && type2 == TasteType.SweetBitter) || (type1 == TasteType.SweetBitter && type2 == TasteType.Sweet))
        {
            return TasteType.Sweet;
        }
         else if ((type1 == TasteType.Bitter && type2 == TasteType.SweetBitter) || (type1 == TasteType.SweetBitter && type2 == TasteType.Bitter))
        {
            return TasteType.Bitter;
        }
        else if ((type1 == TasteType.Bitter && type2 == TasteType.SourBitter) || (type1 == TasteType.SourBitter && type2 == TasteType.Bitter))
        {
            return TasteType.Bitter;
        }
          else if ((type1 == TasteType.SourBitter && type2 == TasteType.SweetSour) || (type1 == TasteType.SweetSour && type2 == TasteType.SourBitter))
        {
            return TasteType.Sour;
        }
          else if ((type1 == TasteType.SweetSour && type2 == TasteType.SweetBitter) || (type1 == TasteType.SweetBitter && type2 == TasteType.SweetSour))
        {
            return TasteType.Sweet;
        }
         else if ((type1 == TasteType.SourBitter && type2 == TasteType.SweetBitter) || (type1 == TasteType.SweetBitter && type2 == TasteType.SourBitter))
        {
            return TasteType.Bitter;
        }
        else if ((type1 == TasteType.Sour && type2 == TasteType.SweetBitter) || (type1 == TasteType.SweetBitter && type2 == TasteType.Sour))
        {
            return TasteType.Sour;
        }
        else if ((type1 == TasteType.Sweet && type2 == TasteType.SourBitter) || (type1 == TasteType.SourBitter && type2 == TasteType.Sweet))
        {
            return TasteType.Sweet;
        }
        else if ((type1 == TasteType.Bitter && type2 == TasteType.SweetSour) || (type1 == TasteType.SweetSour && type2 == TasteType.Bitter))
        {
            return TasteType.Bitter;
        }
        
        

        // Add more rules as necessary
        return TasteType.None; // Default case, if no specific mix rules apply
    }
}