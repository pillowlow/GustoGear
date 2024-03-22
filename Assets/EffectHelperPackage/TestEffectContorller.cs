using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffectContorller : MonoBehaviour
{
     public EffectController controller;
    public KeyCode onKey = KeyCode.O; // Key to turn on the VFX
    public KeyCode offKey = KeyCode.P; // Key to turn off the VFX
    public KeyCode fadeonKey = KeyCode.U; // Key to turn on the VFX
    public KeyCode fadeoffKey = KeyCode.I; // Key to turn off the VFX
    public KeyCode setValueKey = KeyCode.Y;
    public string ID ;
    public float dura;
    // Start is called before the first frame update
    void Start()
    {
        if(controller ==null){
            controller = this.GetComponent<EffectController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(onKey))
        {
            if (controller != null)
                controller.SetOn(ID); // Call the On function
            else
                Debug.LogError("Light reference not set on VFX_SwitchTester.");
        }

        // Check if the 'off' key is pressed
        if (Input.GetKeyDown(offKey))
        {
            if (controller != null)
                controller.SetOff(ID); // Call the Off function
            else
                Debug.LogError("Light reference not set on VFX_SwitchTester.");
        }
        if (Input.GetKeyDown(fadeonKey))
        {
            if (controller != null)
                controller.SetFadeOn(dura,ID); // Call the Off function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }
        if (Input.GetKeyDown(fadeoffKey))
        {
            if (controller != null)
                controller.SetFadeOff(dura,ID); // Call the Off function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }
         if (Input.GetKeyDown(setValueKey))
        {
            if (controller != null)
                controller.SetValueFade(0.5f,dura,ID); // Call the Off function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }
    }
}
