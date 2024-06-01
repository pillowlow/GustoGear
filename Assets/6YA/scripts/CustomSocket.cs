using System.Collections;
using UnityEngine;
using Oculus.Interaction;
using TMPro;

public class CustomSocket : MonoBehaviour
{
    [SerializeField] private GameObject attachPointOnSocket;
    [SerializeField] private GameObject UnitSet;
    [SerializeField] private TMP_Text FlavorText;
    [SerializeField] private TMP_Text TextureText;

    private bool isOccupied = false;

    private float coolDownTime = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;  // 如果已经被占用，直接返回
        if (other.CompareTag("ColorUnit"))
        {
            ColorUnit colorUnit = other.GetComponent<ColorUnit>();
            TasteType CurrentTaste = colorUnit.Taste;
            FlavorText.text = CurrentTaste.ToString();

            TextureText.text = colorUnit.currentState.ToString();
            Debug.Log("CurrentTaste: " + CurrentTaste);
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
                // Debug.Log("Object attached: isKinematic=" + rb.isKinematic + ", useGravity=" + rb.useGravity);
            }
            else
            {
                Debug.LogError("AttachPoint not found");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isOccupied && other.CompareTag("ColorUnit"))
        {

            other.transform.parent = UnitSet.transform;
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            //cool down
            FlavorText.text = "null";
            StartCoroutine(CoolDown());
            isOccupied = false;
            // Debug.Log("Object detached: isKinematic=" + rb.isKinematic + ", useGravity=" + rb.useGravity);
        }
    }
    private IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDownTime);
    }
}
