using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : K_Equipment
{
    // Start is called before the first frame update
    protected override void OnActive()
    {
        base.OnActive(); // Optionally call the base method if you want to keep the base logging
        Debug.Log("Pot is now active and ready for cooking!");
        //CallEffectContorller
        effectController.SetOn(ActiveTag);
        // Additional activation logic here (e.g., start heating up)
    }

    // Override to implement the specific pot processing logic
    protected override IEnumerator ProcessingRoutine(float wait)
    {   
        effectController.SetOff(ActiveTag);
        effectController.SetOn(ProcessingTag);
        Debug.Log("Pot starts cooking...");
        yield return new WaitForSeconds(wait); // Simulate cooking time
        Debug.Log("Cooking is underway...");
        yield return new WaitForSeconds(wait); // Continue cooking
        OnProcessingEnd();
    }

    // Override to handle what happens when processing ends
    protected override void OnProcessingEnd()
    {
        base.OnProcessingEnd(); // Optionally maintain base functionality
        effectController.SetOff(ProcessingTag);
        Debug.Log("Pot has finished cooking. Please serve the dish.");
        // Possibly trigger animation or effects related to completion
    }

    // Override to handle what happens when the cycle is complete
    protected override void OnCompleteStart()
    {   
        
        base.OnCompleteStart(); // Maintain base functionality if needed
        effectController.SetOff(ProcessingTag);
        effectController.SetOn(CompleteTag);
        Debug.Log("Dish is ready to be served from the pot.");
        // Trigger any completion effects here
    }

    // Override to handle what happens when the completion phase ends
    protected override void OnCompleteEnd()
    {
        base.OnCompleteEnd(); // Maintain base functionality if needed
        effectController.SetOff(CompleteTag);
        Debug.Log("Pot is being cleaned and prepared for the next use.");
        // Cleaning up or resetting pot state
    }

}
