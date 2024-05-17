using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class CustomSocket : MonoBehaviour
{
    [SerializeField]
    private GameObject attachPointOnSocket;
    [SerializeField]
    private GameObject UnitSet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ColorUnit"))
        {
            // get object name "AttachPoint" 
            GameObject attachPointOnSocket = GameObject.Find("AttachPoint");
            if (attachPointOnSocket == null)
            {
                Debug.LogError("AttachPoint not found");
                other.transform.position = attachPointOnSocket.transform.position;
                other.transform.parent = attachPointOnSocket.transform;
                other.GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                Debug.Log("AttachPoint found");
                // 將物件移動到新位置並將其作為子對象
                other.transform.position = attachPointOnSocket.transform.position;
                other.transform.parent = attachPointOnSocket.transform;
                other.GetComponent<Rigidbody>().isKinematic = true;
                other.GetComponent<Rigidbody>().useGravity = false;

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ColorUnit"))
        {
            // 解除吸附
            other.transform.parent = UnitSet.transform;
            other.GetComponent<Rigidbody>().isKinematic = false;
            other.GetComponent<Rigidbody>().useGravity =false;

        }
    }
}
