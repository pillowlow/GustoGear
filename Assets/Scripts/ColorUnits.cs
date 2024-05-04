using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UnitState
    {
        Normal,
        Liquid,
        Dried,
        Iced
    }
public class ColorUnit : MonoBehaviour
{
    // Public Color variable for basic color
    
    public TasteType taste;

    // Enumeration for different states of the ColorUnit
    

    // Public State variable
    public UnitState currentState;

    // Serialized private strings for material parameter names
    [SerializeField] private string baseColorParamName;
    [SerializeField] private string emissionColorParamName;

    // Material to apply colors to
    public Material targetMaterial;

    void Start()
    {
        ApplyColors();
    }

    // Function to calculate emission color from the base color
    private Color CalculateEmissionColor(Color baseColor)
    {
        Color emissionColor = baseColor * 2; // Simple multiplication to enhance brightness
        emissionColor.a = 1; // Ensure alpha is set to 1 for full opacity
        return emissionColor;
    }

    // Function to apply colors to material parameters
    private void ApplyColors()
    {
        if (targetMaterial != null)
        {   Color color = ColorListManager.Instance.GetColorByTasteType(taste);
            targetMaterial.SetColor(baseColorParamName, color);
            Color emissionColor = CalculateEmissionColor(color);
            targetMaterial.SetColor(emissionColorParamName, emissionColor);
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}