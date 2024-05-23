using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ESP32BLEApp : MonoBehaviour
{
    public string DeviceName = "ESP32 BLE Server"; //esp32 device name
    private string ServiceUUID = "4fafc201-1fb5-459e-8fcc-c5c9c331914b";
    private string Characteristic = "beb5483e-36e1-4688-b7f5-ea07361b26a8";

    enum States
    {
        None,
        Scan,
        Connect,
        RequestMTU,
        Subscribe,
        Unsubscribe,
        Disconnect,
        Communication,
    }

    private bool _workingFoundDevice = true;
    private bool _connected = false;
    private float _timeout = 0f;
    private States _state = States.None;
    private bool _foundID = false;
    private string _deviceAddress;

    [SerializeField] private TMP_Text deviceNameText;
    [SerializeField] private TMP_Text deviceIDText;


    [SerializeField] private TMP_Text stateText;

    // // this is our hm10 device
    // private string _hm10;
    // public void OnButton(Button button)
    // {
    //     if (button.name.Contains("Send"))
    //     {
    //         if (string.IsNullOrEmpty(TextToSend.text))
    //         {
    //             BluetoothStatus.text = "Enter text to send...";
    //         }
    //         else
    //         {
    //             SendString(TextToSend.text);
    //         }
    //     }
    //     else if (button.name.Contains("Toggle"))
    //     {
    //         SendByte(0x01);
    //     }
    // }

    void Reset()
    {
        _workingFoundDevice = false;    // used to guard against trying to connect to a second device while still connecting to the first
        _connected = false;
        _timeout = 0f;
        _state = States.None;
        _foundID = false;
        _deviceAddress = null;
        // PanelMiddle.SetActive(false);
    }

    void SetState(States newState, float timeout)
    {
        _state = newState;
        _timeout = timeout;
    }
    void SetStateText(string text)
    {
        if(stateText == null) return;
        stateText.text = text;
    }

    void StartProcess()
    {
        // BluetoothStatus.text = "Initializing...";
        SetStateText("Initializing...");
        deviceNameText.text = "Device Name: " + DeviceName;
        deviceIDText.text = "SERVICE_UUID:"+ ServiceUUID + "\nCHARACTERISTIC_UUID:"+ Characteristic;
        Reset();
        BluetoothLEHardwareInterface.Initialize(true, false, () =>
        {

            SetState(States.Scan, 0.1f);
            SetStateText("Initialized");


        }, (error) =>
        {
            BluetoothLEHardwareInterface.Log("Error: " + error);
        });
    }

    // Use this for initialization
    void Start()
    {
        StartProcess();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeout > 0f)
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0f)
            {
                _timeout = 0f;

                switch (_state)
                {
                    case States.None:
                        break;

                    case States.Scan:
                        // BluetoothStatus.text = "Scanning for ESP32 devices...";
                        SetStateText("Scanning for ESP32 devices...");

                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
                        {

                            // we only want to look at devices that have the name we are looking for
                            // this is the best way to filter out devices
                            if (name.Contains(DeviceName))
                            {
                                _workingFoundDevice = true;

                                // it is always a good idea to stop scanning while you connect to a device
                                // and get things set up
                                BluetoothLEHardwareInterface.StopScan();

                                // add it to the list and set to connect to it
                                _deviceAddress = address;
                                SetState(States.Connect, 0.5f);

                                _workingFoundDevice = false;
                            }

                        }, null, false, false);
                        break;

                    case States.Connect:
                        // set these flags
                        _foundID = false;
                        SetStateText("Connecting to ESP32 ...");

                        // note that the first parameter is the address, not the name. I have not fixed this because
                        // of backwards compatiblity.
                        // also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
                        // the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
                        // large enough that it will be finished enumerating before you try to subscribe or do any other operations.
                        BluetoothLEHardwareInterface.ConnectToPeripheral(_deviceAddress, null, null, (address, serviceUUID, characteristicUUID) =>
                        {

                            if (IsEqual(serviceUUID, ServiceUUID))
                            {
                                // if we have found the characteristic that we are waiting for
                                // set the state. make sure there is enough timeout that if the
                                // device is still enumerating other characteristics it finishes
                                // before we try to subscribe
                                if (IsEqual(characteristicUUID, Characteristic))
                                {
                                    _connected = true;
                                    SetState(States.RequestMTU, 2f);
                                    SetStateText("Connected to ESP32");
                                }
                            }
                        }, (disconnectedAddress) =>
                        {
                            BluetoothLEHardwareInterface.Log("Device disconnected: " + disconnectedAddress);
                            SetStateText("Disconnected");
                        });
                        break;

                    case States.RequestMTU:
                        SetStateText("Requesting MTU");
                        BluetoothLEHardwareInterface.RequestMtu(_deviceAddress, 185, (address, newMTU) =>
                        {
                            SetStateText("ESP32 set to " + newMTU.ToString());
                            SetState(States.Subscribe, 0.1f);
                        });
                        break;

                    case States.Subscribe:
                        SetStateText("Subscribing to ESP32...");

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, ServiceUUID, Characteristic, null, (address, characteristicUUID, bytes) =>
                        {
                            string convertred = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length); 
                            SetStateText("Bytes length: " + bytes.Length + " ,Data: " + convertred);
                        });

                        // set to the none state and the user can start sending and receiving data
                        _state = States.None;
                        break;

                    case States.Unsubscribe:
                        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(_deviceAddress, ServiceUUID, Characteristic, null);
                        SetState(States.Disconnect, 4f);
                        break;

                    case States.Disconnect:
                        if (_connected)
                        {
                            BluetoothLEHardwareInterface.DisconnectPeripheral(_deviceAddress, (address) =>
                            {
                                BluetoothLEHardwareInterface.DeInitialize(() =>
                                {

                                    _connected = false;
                                    _state = States.None;
                                });
                            });
                        }
                        else
                        {
                            BluetoothLEHardwareInterface.DeInitialize(() =>
                            {

                                _state = States.None;
                            });
                        }
                        break;
                }
            }
        }
    }

    string FullUUID(string uuid)
    {
        return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
    }

    // void SendString(string value)
    // {
    //     var data = Encoding.UTF8.GetBytes(value);
    //     // notice that the 6th parameter is false. this is because the HM10 doesn't support withResponse writing to its characteristic.
    //     // some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
    //     // the device
    //     BluetoothLEHardwareInterface.WriteCharacteristic(_hm10, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) =>
    //     {

    //         BluetoothLEHardwareInterface.Log("Write Succeeded");
    //     });
    // }

    // void SendByte(byte value)
    // {
    //     byte[] data = new byte[] { value };
    //     // notice that the 6th parameter is false. this is because the HM10 doesn't support withResponse writing to its characteristic.
    //     // some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
    //     // the device
    //     BluetoothLEHardwareInterface.WriteCharacteristic(_hm10, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) =>
    //     {

    //         BluetoothLEHardwareInterface.Log("Write Succeeded");
    //     });
    // }
}
