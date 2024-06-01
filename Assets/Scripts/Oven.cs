using System.Collections;
using UnityEngine;

public class Oven : K_Equipment
{
    // Override to handle what happens when the oven becomes active
    protected override void OnActive()
    {
        base.OnActive();
        Debug.Log("The oven warms up eagerly, ready to bake your delicious creation!");
        effectController.SetOn(ActiveTag);
    }

    // Override to implement the specific oven processing logic
    protected override IEnumerator ProcessingRoutine(float wait)
    {
        effectController.SetOff(ActiveTag);
        effectController.SetOn(ProcessingTag);
        // Start the timer
        timeManager.genTimer((int)processingTime + endingTime);
        Debug.Log("The oven's heat wraps around the dough, beginning the transformation...");
        yield return new WaitForSeconds(wait);
        Debug.Log("Golden crusts form as the scent of fresh baking fills the air...");
        yield return new WaitForSeconds(wait);
        OnProcessingEnd();
    }

    // Override to handle what happens when processing ends
    protected override void OnProcessingEnd()
    {
        base.OnProcessingEnd();
        effectController.SetOff(ProcessingTag);
        Debug.Log("Baking complete! The oven dings. Time to enjoy the fruits of your labor.");
        timeManager.destroyTimer();
    }

    // Override to handle what happens when the cycle is complete
    protected override void OnCompleteStart()
    {
        base.OnCompleteStart();
        effectController.SetOn(CompleteTag);
        Debug.Log("The feast is prepared. It's time to indulge in your baked delights!");
    }

    // Override to handle what happens when the completion phase ends
    protected override void OnCompleteEnd()
    {
        base.OnCompleteEnd();
        effectController.SetOff(CompleteTag);
        Debug.Log("The oven cools down, resting after a job well done.");
    }
}
