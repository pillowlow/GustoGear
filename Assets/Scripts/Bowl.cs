
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour
{
    [SerializeField]
    private Collider myCollider; // Consider whether you need this exposed if not used programmatically
    [SerializeField]
    private string targetTag = "ColorUnit"; // Ensure this matches the tag of your ColorUnit objects
    [SerializeField]
    private Material mat;
    [SerializeField]
    private string colorParamName = "_Color"; // Shader color property name
    [SerializeField]
    private string edgeColorPara = "_EdgeColor"; // Ensure shader has this property
    [SerializeField]
    private string waterLevelParamName = "_WaterLevel"; // Shader water level property name
    [SerializeField]
    private Vector2 waterLevelRange = new Vector2(0.0f, 1.0f); // Range for water level normalization

    private int waterLevel = 0;
    private int maxLevel = 2;
    private Color liquidColor = Color.black; // Default color

    void Start()
    {
        ResetBowl();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && waterLevel < maxLevel)
        {
            ColorUnit colorUnit = other.GetComponent<ColorUnit>();
            if (colorUnit != null && colorUnit.currentState == UnitState.Liquid) // State check directly
            {   
                Color incomingColor = ColorListManager.Instance.GetColorByTasteType(colorUnit.taste);
                if (liquidColor == Color.black) // If bowl is initially empty
                {
                    liquidColor = CalculateHDRColor(incomingColor); // Directly assign HDR color
                }
                else
                {
                    MixColors(CalculateHDRColor(incomingColor));
                }

                waterLevel++; // Increment water level
                UpdateMaterialProperties();
                
                if (waterLevel >= maxLevel)
                {
                    Debug.Log("Bowl Full");
                }
            }
        }
    }

    private void MixColors(Color newColor)
    {
        // Mixing the new color with the existing one by averaging
        liquidColor = (liquidColor + newColor) / 2;
    }

    private void UpdateMaterialProperties()
    {
        if (mat != null)
        {
            mat.SetColor(colorParamName, liquidColor); // Set main color
            mat.SetColor(edgeColorPara, CalculateHDRColor(liquidColor)); // Set edge color with HDR
            float normalizedLevel = Mathf.Lerp(waterLevelRange.x, waterLevelRange.y, (float)waterLevel / maxLevel);
            mat.SetFloat(waterLevelParamName, normalizedLevel); // Set water level in shader
        }
    }

    public void ResetBowl()
    {
        waterLevel = 0;
        liquidColor = Color.black;
        UpdateMaterialProperties(); // Update shader properties
        Debug.Log("Bowl reset to initial state.");
    }

    private Color CalculateHDRColor(Color baseColor)
    {
        // Enhancing the color's brightness; adjust the multiplier as needed
        return baseColor * 2.0f;
    }
}
