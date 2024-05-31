using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Locomotion;
using UnityEngine;

[System.Serializable]
public struct ChangePair{
    public UnitState initState;
    public UnitState changedState;
    public GameObject spawnItem;
}
public abstract class K_Equipment : MonoBehaviour
{
    public enum EquipmentStatus
    {
        Unactive,
        Active,
        Processing,
        Complete
    }
    [SerializeField]
    private Transform SpawnPoint;
     [SerializeField]
    protected List<ChangePair> acceptSets = new();
    [SerializeField]
    private float processingTime = 2;

    [SerializeField]
    protected string ActiveTag;
    [SerializeField]
    protected string ProcessingTag;
    [SerializeField]
    protected string CompleteTag;
     [SerializeField]
    protected EffectController effectController;

    private ChangePair CurrentPair ;
    protected ColorUnit currentUnit; // Store the current ColorUnit interacting with the equipment
    private bool HasSpawnItem= false;
    protected bool isPlayerPresent = false; // Flag to check if player is within the collider

    public EquipmentStatus status { get; protected set; }

    protected virtual void OnTriggerEnter(Collider other)
    {   
        //show other name
        Debug.Log("Enter Collider:"+other.name);
        ColorUnit unit = other.GetComponent<ColorUnit>();
        if (other.CompareTag("ColorUnit"))
        {
            foreach(ChangePair pair in acceptSets){
                if (unit != null && unit.currentState == pair.initState) // Check for specific state if needed
                {
                    currentUnit = unit;
    
                    if (isPlayerPresent && status == EquipmentStatus.Unactive) // Check if player is present and equipment is unactive
                    {
                        status = EquipmentStatus.Active;
                        OnActive();
                    }
                    CurrentPair = pair;
                    break;
                }
            }
        }
        else if (other.CompareTag("Player"))
        {
            isPlayerPresent = true;
            OnPlayerEnter();
            if (currentUnit != null && status == EquipmentStatus.Unactive)
            {
                status = EquipmentStatus.Active;
                OnActive();
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ColorUnit"))
        {
            ColorUnit unit = other.GetComponent<ColorUnit>();
            if (unit != null)
            {   
                // deal with the qualified preprocessing Unit Leave
                foreach(ChangePair pair in acceptSets){
                    if(unit.currentState == pair.changedState){
                        currentUnit = null;
                        HasSpawnItem = false;
                        OnCompleteEnd();
                    }
                }
                
                
            }
        }
        else if (other.CompareTag("Player"))
        {
            isPlayerPresent = false;
            OnPlayerLeave();
            if (status == EquipmentStatus.Active)
            {   
                // handle the put-in state
                if(!HasSpawnItem){
                    if (currentUnit != null)
                    {
                        currentUnit.DestroySelf(); // Destroy ColorUnit when player leaves
                    }
                    status = EquipmentStatus.Processing;
                    StartCoroutine(ProcessingRoutine(processingTime));
                }
                // handle the SpawnItem been taken
                
            }
        }
    }

    protected virtual void OnPlayerEnter()
    {
        Debug.Log("Player Entered");
    }

    protected virtual void OnPlayerLeave()
    {
        Debug.Log("Player Left");
    }

    protected virtual void OnActive()
    {   
        if(currentUnit != null){
            currentUnit.transform.position = SpawnPoint.position;
            currentUnit.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
        else{
            Debug.Log("no current Unit");
        }
        
        Debug.Log("Equipment Active");
    }

    protected virtual IEnumerator ProcessingRoutine(float wait)
    {   
        
        Debug.Log("Processing Start");
        yield return new WaitForSeconds(wait); // Wait for 5 seconds or any other specified time
        OnProcessingEnd();
    }

    protected virtual void OnProcessingEnd()
    {   

        Debug.Log("Processing End");
        status = EquipmentStatus.Complete;
        OnCompleteStart();
    }

    protected virtual void OnCompleteStart()
    {
        Debug.Log("Complete Start");
        if(currentUnit != null){
            currentUnit.DestroySelf();
            currentUnit = null;
        }
        SpawnOutcome();

    }

    protected virtual void OnCompleteEnd()
    {
        Debug.Log("Complete End");
        status = EquipmentStatus.Unactive;
    }

    public void SpawnOutcome( )
    {   
        GameObject toSpawn = CurrentPair.spawnItem;
        if (toSpawn != null)
        {
            GameObject outcome = Instantiate(toSpawn, SpawnPoint.position, Quaternion.identity);
            ColorUnit colorUnit = outcome.GetComponent<ColorUnit>();
            if (colorUnit != null)
            {
                colorUnit.AssignTaste(currentUnit.Taste);
            }
            else
            {
                Debug.LogError("Spawned object does not have a ColorUnit component.");
            }
            HasSpawnItem = true;
        }
        else
        {
            Debug.LogError("No prefab assigned to spawn the outcome.");
        }
    }
}
