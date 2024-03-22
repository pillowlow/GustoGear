using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLightController : MonoBehaviour
{
    // Start is called before the first frame update
     public LightController LightCnt;
    public KeyCode onKey = KeyCode.O; // Key to turn on the VFX
    public KeyCode offKey = KeyCode.P; // Key to turn off the VFX
    public KeyCode fadeonKey = KeyCode.U; // Key to turn on the VFX
    public KeyCode fadeoffKey = KeyCode.I; // Key to turn off the VFX
    public string ID ;
    public float dura;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(onKey))
        {
            if (LightCnt != null)
                LightCnt.SetTrigger(ID,1); // Call the On function
            else
                Debug.LogError("Light reference not set on VFX_SwitchTester.");
        }

        // Check if the 'off' key is pressed
        if (Input.GetKeyDown(offKey))
        {
            if (LightCnt != null)
                LightCnt.SetTrigger(ID,0); // Call the Off function
            else
                Debug.LogError("Light reference not set on VFX_SwitchTester.");
        }
        if (Input.GetKeyDown(fadeonKey))
        {
            if (LightCnt != null)
                LightCnt.SetTriggerFade(ID,1,dura); // Call the Off function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }
        if (Input.GetKeyDown(fadeoffKey))
        {
            if (LightCnt != null)
                LightCnt.SetTriggerFade(ID,0,dura); // Call the Off function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }
    }
}
