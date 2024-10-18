using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

public class TeleDataManager : MonoBehaviour
{
    public static TeleDataManager Instance;

    public UserData userData;

    public static Action<string> OnGetUserID;
    public static UnityEvent OnSaveUserData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        Login();
    }

    IEnumerator SendRequest(string url, string data, Action<string> completeAction, Action onFailAction)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                onFailAction?.Invoke();
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                completeAction?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public void Login()
    {
        Debug.Log("Login");

        var _userID = new { id = userData.id };


        string data = JsonConvert.SerializeObject(_userID);
        Debug.Log(data);
        StartCoroutine(SendRequest(ServerUrl.LOGIN, data, OnLoginSucess, OnLoginFail));
    }

    public void SignUp()
    {
        Debug.Log("SignUp");
        userData.data.swap = 5;
        userData.data.rotate = 5;

        string data = JsonConvert.SerializeObject(userData);
        StartCoroutine(SendRequest(ServerUrl.SIGN_UP, data, OnSignUpSucess, OnSignUpFail));
    }

    public void AddCoin(int value)
    {
        var addCoin = new { id = userData.id, amount = value };
        string data = JsonConvert.SerializeObject(addCoin);
        StartCoroutine(SendRequest(ServerUrl.DEPOSIT, data, null, null));
    }

    public void SpendCoin(int value)
    {
        var spendCoin = new { id = userData.id, amount = value };
        string data = JsonConvert.SerializeObject(spendCoin);
        StartCoroutine(SendRequest(ServerUrl.SPEND, data, null, null));
    }

    void OnLoginSucess(string data)
    {
        Debug.Log("LoginSucess");
        JObject jsonData = JObject.Parse(data);
        string userDataJson = jsonData["data"].ToString();
        userData = JsonConvert.DeserializeObject<UserData>(userDataJson);
        SceneManager.LoadScene(1);
    }

    void OnLoginFail()
    {
        Debug.Log("LoginFail");
        SignUp();
    }


    void OnSignUpSucess(string data)
    {
        Debug.Log("SignUp Sucess");
        SceneManager.LoadScene(1);
    }

    void OnSignUpFail()
    {
        Debug.Log("SignUp Fail");
    }
}

[System.Serializable]
public class GameData
{
    public int swap;
    public int rotate;
}

[System.Serializable]
public class UserData
{
    public int id;
    public string username;
    public string avatar;
    public int balance;
    public GameData data;
}