using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoRead : MonoBehaviour {

    SerialPort serialPort = new SerialPort("COM3", 9600);

    private bool timeOut = false;
    private bool timeOutBusy = false;
    [SerializeField]
    private float timeOutTime = 0.1f; //time between sending read coordinates
    [SerializeField]
    private bool isInput = false;

    private void Start() {
        if (serialPort != null) {
            serialPort.Open();
            serialPort.ReadTimeout = 1;
            }
        }

    public bool getInput() {
        return isInput;
        }
    public bool setInput(bool set) {
        isInput = set;
        return isInput;
        }

    //call functions
    private void Update() {
        ReadSerial();

        if (timeOut && !timeOutBusy) {
            StartCoroutine("TimeOut");
            timeOutBusy = true;
            }
        }

    //timeout time after successfully reading a coordinate
    private IEnumerator TimeOut() {
        yield return new WaitForSeconds(timeOutTime);
        timeOut = false;
        timeOutBusy = false;
        }

    //try to read from serialport, check value for errors, send correct values to ReadCoordinates
    private void ReadSerial() {
        try {
            string message = serialPort.ReadLine();
            try {
                int coords = 0;
                coords = int.Parse(message);
                if (message.Length > 1 && !timeOutBusy) {
                    ReadCoordinates(coords);
                    timeOut = true;
                    }
                else {
                    //Debug.Log("unsuccessful read: " + message); //timeOut, or no x value
                    }
                }
            catch {
                //Debug.Log("unsuccessful read: " + message); //when no int
                }
            }
        catch {
            //Debug.Log("Can't find port");
            }
        }

    //splits int to x and y
    private void ReadCoordinates(int value) {
        isInput = true;
        Debug.Log(isInput);
        value = value % 100; //makes sure the value is only two digits
        int x = Mathf.FloorToInt(value / 10);
        int y = value % 10;
        //this is where you can use your x and y values for anything
        Debug.Log("successfully read coordinates: X:" + x + ", Y:" + y);
        if (Manager.Instance.inStartGame) {
            Manager.Instance.inTutorial = true;
            Manager.Instance.blockSpawner.inTutorial = true;
            Manager.Instance.StartGame();
            Manager.Instance.inStartGame = false;
            }
        else if (Manager.Instance.requestInput) {
            Manager.Instance.requestInput = false;
            }
        else if (Manager.Instance.isTheNeighbourActive) {
            Manager.Instance.theNeighbour.GetComponent<NeighbourScript>().HitTheNeighbour();
            }
        else if (Manager.Instance.cat1.activeSelf) {
            Manager.Instance.audioManager.Sfx(blockType.cat);
            }
        else {
            ArduinoInput(x, y);
            }

        isInput = false;
        }


    public void ArduinoInput(int x, int y) {
        //Break blocks if the X&Y connect with the X&Y of a block
        if (Manager.Instance.activeBlocks.Count >= 1) {
            for (int i = 0; i < Manager.Instance.activeBlocks.Count; i++) {
                if (x == Mathf.RoundToInt(Manager.Instance.activeBlocks[i].transform.position.x)) {
                    if (y + 1 == Mathf.RoundToInt(Manager.Instance.activeBlocks[i].transform.position.y)) {
                        Manager.Instance.activeBlocks[i].gameObject.GetComponent<BaseClass>().Hit();
                        Camera.main.GetComponent<Screenshake>().StartShake(10);
                        }
                    }
                }
            }
        }
    }
            
