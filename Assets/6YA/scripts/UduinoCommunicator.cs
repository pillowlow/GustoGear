using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
using TMPro;

#if UDUINO_READY
public class UduinoCommunicator : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

    // Start is called before the first frame update
    void Start()
    {
        // 注册数据接收事件
        UduinoManager.Instance.OnDataReceivedEvent.AddListener(OnDataReceived);
    }

    // Update is called once per frame
    void Update()
    {
        // 按需添加，例如读取按键启动读取或发送命令
    }

    // 调用此方法发送数据到 Arduino
    public void UduinoDataSend(string data)
    {
        // 获取当前时间
        System.DateTime currentTime = System.DateTime.Now;
        // 将时间发送到 Uduino 作为测试
        UduinoManager.Instance.sendCommand("timeUpdate", currentTime.ToString("HH:mm:ss"));
        UduinoManager.Instance.sendCommand("dataSend", data);
        Debug.Log("Sent: " + data);

    }

    // 当从 Uduino 收到数据时调用此方法
    private void OnDataReceived(string data, UduinoDevice device)
    {
        messageText.text = device.name + " " + data;
        Debug.Log("Received from " + device.name + ": " + data);
    }
}
#endif
