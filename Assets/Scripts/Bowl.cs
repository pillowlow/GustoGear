
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour
{
    [SerializeField]
    private Collider myCollider;
    [SerializeField]
    private string targetTag = "ColorUnit"; // Ensure this matches the tag of your ColorUnit objects
    [SerializeField]
    private Material mat;
    [SerializeField]
    private string colorParamName = "_Color"; // Shader color property name
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
            if (colorUnit != null)
            {
                if (liquidColor == Color.black)
                {
                    liquidColor = CalculateHDRColor(colorUnit.basicColor); // Assign HDR color directly if current is black
                }
                else
                {
                    MixColors(CalculateHDRColor(colorUnit.basicColor));
                }

                waterLevel += 1; // Increment water level
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
        liquidColor = (liquidColor + newColor) / 2;
    }

    private void UpdateMaterialProperties()
    {
        if (mat != null)
        {
            mat.SetColor(colorParamName, liquidColor);
            float normalizedLevel = Mathf.Lerp(waterLevelRange.x, waterLevelRange.y, (float)waterLevel / maxLevel);
            mat.SetFloat(waterLevelParamName, normalizedLevel);
        }
    }

    public void ResetBowl()
    {
        waterLevel = 0;
        liquidColor = Color.black;
        UpdateMaterialProperties();
        Debug.Log("Bowl reset to initial state.");
    }

    private Color CalculateHDRColor(Color baseColor)
    {
        return baseColor * 2.0f; // Simple example of making the color brighter
    }
}
