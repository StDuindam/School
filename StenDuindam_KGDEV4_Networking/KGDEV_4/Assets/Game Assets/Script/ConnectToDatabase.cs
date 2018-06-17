using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData {
    public string playerName;
    public string playerScore;
    public string region;
    public string playerId;
    }

public delegate void JsonReply(string json);

public class ConnectToDatabase : MonoBehaviour {

    [Header("Login")]
    public Text username;
    public Text password;

    [Header("Register")]
    public Text registerUsername;
    public Text registerPassword;
    public Text registerNickname;
    public Dropdown registerRegion;

    //Linked player
    public bool loginSuccessful = false;

    //VARIABLES
    const string GET_SCORES = "getScores";
    const string POST_SCROES = "postScores";
    const string LOGIN = "login";
    const string REGISTER = "register";
    const string NAME = "name";
    const string SCORE = "add";
    const string SESSION_ID = "SessionID";
    const string USER_ID = "id";

    //Variables from the login system
     public string userID = "0";
    public string sessionID = "12345";
    public string playerName = "someName";
    public int scoreForPlayer = 0;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
	}
	

    public void Login() {
        StartCoroutine(ServerFunc(LOGIN, HandleLogin, "login",username.text.ToString(),"pass", password.text.ToString()));
        Debug.Log(username.text.ToString() + " " + password.text.ToString());
        }

    public void AddScoreToDB(int _score) {
        StartCoroutine(ServerFunc(SCORE, ParseScores, "score", _score.ToString()));
        }

    public void Register() {
        StartCoroutine(ServerFunc(REGISTER, RegisterConfirmation, "registerlogin", registerUsername.text.ToString(), "registerpassword", registerPassword.text.ToString(), "registernickname", registerNickname.text.ToString(), "registerregion",registerRegion.value.ToString()));
        Debug.Log(registerUsername.text.ToString() + " " + registerPassword.text.ToString() + " " + registerNickname.text.ToString() + " " + registerRegion.value.ToString());
        }

	// Update is called once per frame
    IEnumerator ServerFunc(string request, JsonReply callback, params string[] postArgs) {

        //First we create a form to add information to
        WWWForm form = new WWWForm();
        form.AddField("request", request);
        form.AddField("SessionID", sessionID);
        form.AddField("game", 6);
        form.AddField("id", userID);

        //This is a handy way you can add arguments (in pairs of 2, "&name=value") to the webrequest
        for (int i = 0; i < postArgs.Length; i += 2) {
            form.AddField(postArgs[i], postArgs[i + 1]);
            }

        WWW www = new WWW("http://sten.onwebsiteenter.com/unitymanager.php", form);
     
        //Wait for reply
        yield return www;

        if (www.error != null) {
            Debug.Log(www.error); }
        else {
            Debug.Log(www.text.ToString());
            callback(www.text);

            }
        yield return null;
        }
    
     void ParseScores(string scoresJSON) {
        Debug.Log("parsed score");
        }

    void RegisterConfirmation(string passJSON) {
        Debug.Log("Account created!");
        JObject recievedJObject = JObject.Parse(passJSON);
        Debug.Log(recievedJObject);
        }

    void HandleLogin(string passJSON) {
        //Debug.Log(passJSON);
        JObject recievedJObject = JObject.Parse(passJSON);
        sessionID = recievedJObject[SESSION_ID].Value<string>();
        string nickName = recievedJObject["Nickname"].Value<string>();
        string region = recievedJObject["Region"].Value<string>();
        userID = recievedJObject["id"].Value<string>();
        
        Debug.Log("Got sessionID: " + sessionID + nickName);
        if (sessionID != null) { loginSuccessful = true; SceneManager.LoadScene(1); };
        PlayerData pl = new PlayerData();
        playerName = nickName;

        pl.playerId = userID;
        pl.playerName = nickName;
        pl.region = region;
        }
}