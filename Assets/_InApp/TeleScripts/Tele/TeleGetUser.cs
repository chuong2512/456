using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System.Collections;

public class TeleGetUser : MonoBehaviour
{
    int userID;
    string userName;

    [DllImport("__Internal")]
    private static extern void GetUserInfo();

    [DllImport("__Internal")]
    private static extern void InitTeleWebApp();


    private void Start()
    {       
        InitTeleWebApp();
        GetUserInfo();
    }

    public void CallBackUserData(string userData)
    {            
        JObject jsonData = JObject.Parse(userData);
        int id = (int)jsonData["id"];
        userName = (string)jsonData["username"];      
        userID = id;
        TeleDataManager.Instance.userData.id = userID;
        TeleDataManager.Instance.userData.username = userName;      
    }
}



   