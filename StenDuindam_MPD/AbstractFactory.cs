using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public abstract class AbstractFactory {
  
    public static AbstractFactory GetInstance() {
    SerialPort serialPort = new SerialPort("COM3", 9600);

        //Check if in editor, return windows implementation if this is the case
        if (Application.isEditor)
            return new WindowsImplementation();
        switch (Application.platform) {         
#if !DISABLE_SYSTEM
            case RuntimePlatform.WindowsPlayer:
                //Check if there is an arduino, if that is the case run the Arduino implementation
                if (serialPort != null) {
                    return new ArduinoImplementation();
                    }
                //Otherwise continue with the normal WindowsImplementation
                else {
                    return new WindowsImplementation();
                    }
            case RuntimePlatform.IPhonePlayer:
                return new IPhoneImplementation();
            case RuntimePlatform.Android:
                return new AndroidImplementation();
#endif
            default:
                return new DummyImplementation();
        }
    }
    //Two functions to change the game and input per platform
    public abstract void LoadBoardPreset();
    public abstract void CheckInput();
}

//ARDUINO
    public class ArduinoImplementation : AbstractFactory {       
        public override void LoadBoardPreset() {
        //Create Arduino Component
        if (Manager.Instance.arduino == null) {
            Manager.Instance.AddArduinoComponent();
            Manager.Instance.arduino = Manager.Instance.gameObject.GetComponent<ArduinoRead>();
            }
        //board presets
        Manager.Instance.xWidth = 5;
        Manager.Instance.yLength = 5;
        Manager.Instance.timeToSpawn = 1;
        //camera setting
        Camera.main.transform.position = new Vector3(2, 3.5f, -5);
        Camera.main.GetComponent<Camera>().orthographic = false;
        }
        
        public override void CheckInput() {
        //Is handled by ArduinoRead
        }
    }

//WINDOWS
public class WindowsImplementation : AbstractFactory {
    public override void LoadBoardPreset() {
        //board presets
        Manager.Instance.xWidth = 5;
        Manager.Instance.yLength = 5;
        Manager.Instance.timeToSpawn = 1;
        //camera setting
        Camera.main.transform.position = new Vector3(2, 3.5f, -5);
        Camera.main.GetComponent<Camera>().orthographic = false;
        }

    public override void CheckInput() {
        //Input key
        if (Input.GetMouseButton(0)) {
            Vector3 MouseLocation = Input.mousePosition;
            Vector3 sourcePos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            MouseLocation.z = -sourcePos.z;
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(MouseLocation);

            //Change gamestate to tutorial if the game has not yet started
            if (Manager.Instance.inStartGame) {
                Manager.Instance.inTutorial = true;
                //Set the blockspawner to tutorial mode
                Manager.Instance.blockSpawner.inTutorial = Manager.Instance.inTutorial;
                Manager.Instance.StartGame();
                return;
                }

            //Determine direction of raycast
            Vector3 direction = targetPos - sourcePos;

            //Make the actual raycast
            Debug.DrawRay(sourcePos, direction, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(sourcePos, direction, out hit, 200f)) {
                if (hit.transform.GetComponent<BaseClass>()) {
                    hit.transform.GetComponent<BaseClass>().Hit();
                    }
                if (hit.transform.GetComponent<NeighbourScript>()) {
                    Debug.Log("HIT!");
                    hit.transform.GetComponent<NeighbourScript>().HitTheNeighbour();
                    }
                if (hit.transform.tag == ("Cat")) {
                    Manager.Instance.audioManager.Sfx(blockType.cat);
                    }
                }
            }
        }
    }

//IOS
public class IPhoneImplementation : AbstractFactory {
    public override void LoadBoardPreset() {
        //board presets
        Manager.Instance.xWidth = 3;
        Manager.Instance.yLength = 5;
        Manager.Instance.timeToSpawn = 1.25f;
        //camera setting
        Camera.main.transform.position = new Vector3(2, 3.5f, -5);
        Camera.main.GetComponent<Camera>().orthographic = false;
        }

    public override void CheckInput() {
        //Input Touch
        if (Input.touchCount > 0) {
            Vector3 touchLocation = Input.GetTouch(0).position;
            Vector3 sourcePos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            touchLocation.z = -sourcePos.z;
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(touchLocation);

            //Change gamestate to tutorial if the game has not yet started
            if (Manager.Instance.inStartGame) {
                Manager.Instance.inTutorial = true;
                //Set the blockspawner to tutorial mode
                Manager.Instance.blockSpawner.inTutorial = Manager.Instance.inTutorial;
                Manager.Instance.StartGame();
                return;
                }

            //Determine direction of raycast
            Vector3 direction = targetPos - sourcePos;

            //Make the actual raycast
            Debug.DrawRay(sourcePos, direction, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(sourcePos, direction, out hit, 200f)) {
                if (hit.transform.GetComponent<BaseClass>()) {
                    hit.transform.GetComponent<BaseClass>().Hit();
                    }
                if (hit.transform.GetComponent<NeighbourScript>()) {
                    Debug.Log("HIT!");
                    hit.transform.GetComponent<NeighbourScript>().HitTheNeighbour();
                    }
                if (hit.transform.tag == ("Cat")) {
                    Manager.Instance.audioManager.Sfx(blockType.cat);
                    }
                }
            }
        } }

        //ANDROID
        public class AndroidImplementation : AbstractFactory {
            public override void LoadBoardPreset() {
                //do an Android thing
                //board presets
                Manager.Instance.xWidth = 3;
                Manager.Instance.yLength = 5;
                Manager.Instance.timeToSpawn = 1.25f;
                //camera setting
                Camera.main.transform.position = new Vector3(2, 3.5f, -5);
                Camera.main.GetComponent<Camera>().orthographic = false;
                }

            public override void CheckInput() {
                //Input Touch
                if (Input.touchCount > 0) {
                    Vector3 touchLocation = Input.GetTouch(0).position;
                    Vector3 sourcePos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
                    touchLocation.z = -sourcePos.z;
                    Vector3 targetPos = Camera.main.ScreenToWorldPoint(touchLocation);

                    //Change gamestate to tutorial if the game has not yet started
                    if (Manager.Instance.inStartGame) {
                        Manager.Instance.inTutorial = true;
                        //Set the blockspawner to tutorial mode
                        Manager.Instance.blockSpawner.inTutorial = Manager.Instance.inTutorial;
                        Manager.Instance.StartGame();
                        return;
                        }

                    //Determine direction of raycast
                    Vector3 direction = targetPos - sourcePos;

                    //Make the actual raycast
                    Debug.DrawRay(sourcePos, direction, Color.red);
                    RaycastHit hit;
                    if (Physics.Raycast(sourcePos, direction, out hit, 200f)) {
                        if (hit.transform.GetComponent<BaseClass>()) {
                            hit.transform.GetComponent<BaseClass>().Hit();
                            }
                        if (hit.transform.GetComponent<NeighbourScript>()) {
                            Debug.Log("HIT!");
                            hit.transform.GetComponent<NeighbourScript>().HitTheNeighbour();
                            }
                        if (hit.transform.tag == ("Cat")) {
                            Manager.Instance.audioManager.Sfx(blockType.cat);
                            }
                        }
                    }
                }
            }

        //DUMMY IMPLEMENTATION
        public class DummyImplementation : AbstractFactory {
            //Quit the game is this is ever loaded, because no controls will work.
            public override void LoadBoardPreset() {
                Application.Quit();
                }
            public override void CheckInput() { }
            } 
     

