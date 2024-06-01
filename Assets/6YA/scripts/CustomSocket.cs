using System.Collections;
using UnityEngine;
using Oculus.Interaction;

public class CustomSocket : MonoBehaviour
{
    [SerializeField]
    private GameObject attachPointOnSocket;
    [SerializeField]
    private GameObject UnitSet;
    private bool isOccupied = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;  // 如果已经被占用，直接返回
        if (other.CompareTag("ColorUnit"))
        {
            if (attachPointOnSocket != null)
            {
                other.transform.position = attachPointOnSocket.transform.position;
                other.transform.parent = attachPointOnSocket.transform;
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                isOccupied = true;
                Debug.Log("Object attached: isKinematic=" + rb.isKinematic + ", useGravity=" + rb.useGravity);
            }
            else
            {
                Debug.LogError("AttachPoint not found");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ColorUnit"))
        {
            other.transform.parent = UnitSet.transform;
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            isOccupied = false;
            Debug.Log("Object detached: isKinematic=" + rb.isKinematic + ", useGravity=" + rb.useGravity);
        }
    }
}
