using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public GameObject TCanvas; // Assign your Timer Canvas in the inspector
    // [SerializeField] private int setDuration = 70;
    [SerializeField] private GameObject spawnPoint;
    public void genTimer(int processDuration = 10)
    {
        GameObject timerInstance = Instantiate(TCanvas, spawnPoint.transform.position, Quaternion.identity);
        Transform timerTransform = timerInstance.transform.Find("Timer");
        if (timerTransform == null)
        {
            Debug.LogError("Timer object not found in the Timer Canvas");
            return;
        }
        Timer timerScript = timerTransform.GetComponent<Timer>();
        if (timerScript != null)
        {
            timerScript.duration = processDuration; // Set the duration as needed
        }
    }

}
