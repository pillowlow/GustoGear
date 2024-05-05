using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Normal,
    Liquid,
    Dried,
    Iced,
    None
}

public class ColorUnit : MonoBehaviour
{
    // Public Color variable for basic color (you can remove or integrate this as needed)
    [SerializeField]
    private TasteType _taste;

    // Property for TasteType with change detection
    public TasteType Taste
    {
        get => _taste;
        set
        {
            if (_taste != value)
            {
                _taste = value;
                OnTasteChanged();
            }
        }
    }

    // Enumeration for different states of the ColorUnit
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
    private void OnValidate()
    {
        if (Application.isPlaying) // Ensure that changes are only applied during play mode
        {
            Taste = _taste; // This will trigger the setter and the OnTasteChanged method
        }
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
        {
            Color color = ColorListManager.Instance.GetColorByTasteType(Taste);
            targetMaterial.SetColor(baseColorParamName, color);
            Color emissionColor = CalculateEmissionColor(color);
            targetMaterial.SetColor(emissionColorParamName, emissionColor);
        }
    }

    // Called when 'Taste' changes
    private void OnTasteChanged()
    {
        Debug.Log("Taste changed to: " + Taste);
        ApplyColors();  // Reapply colors when taste changes
    }

    public void AssignTaste(TasteType assign)
    {
        Taste = assign;  // Use the property to trigger change detection
        ApplyColors();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
