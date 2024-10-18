using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using SingleApp;

public class TelePurchase : Singleton<TelePurchase>
{
    [SerializeField] List<CoinPack> coinPacks = new List<CoinPack>();
    [SerializeField] string botToken = "";

    string invoiceLink;
    int coinPack;

    public List<CoinPack> CoinPack => coinPacks;

    public static string GenerateSecureToken(int size)
    {
        byte[] tokenData = new byte[size];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(tokenData);
        }

        return Convert.ToBase64String(tokenData);
    }

    public void BuyIapp(int coinPack)
    {
        BuyProductID(coinPack);
    }

    public void BuyProductID(int coinPack)
    {
        string _inAppName = coinPacks[coinPack].coin + " Coins";
        string _priceLabel = "Coin Pack " + coinPack + 1;
        CreateLink(_inAppName, CreateLabelPrice(_priceLabel, coinPacks[coinPack].price));

        StartCoroutine(GetCoin(coinPacks[coinPack].coin));
    }

    public IEnumerator GetCoin(int coin)
    {
        yield return new WaitForSecondsRealtime(3);
        CGameDataManager.Instance.playerData.AddDiamond(coin);

        /*int money = PlayerPrefs.GetInt(MenuScript.MONEY_KEY);
        money += coin;
        PlayerPrefs.SetInt(MenuScript.MONEY_KEY, money);*/

        //PlayfabManager.Instance.SetUserData();
        //PopUpManager.Instance.ShowMessage("Purchase success");
    }

    public string CreateLabelPrice(string label, int price)
    {
        var a = new PriceLabel { label = "Coins " + label, amount = price };
        List<PriceLabel> items = new List<PriceLabel>(1);
        items.Add(a);

        string labelPrice = JsonConvert.SerializeObject(items);

        Debug.Log(labelPrice);
        return labelPrice;
    }

    public void CreateLink(string inappName, string labelPrice)
    {
        StartCoroutine(CreateInvoiceLink1(inappName, labelPrice));
    }

    IEnumerator CreateInvoiceLink1(string inappName, string labelPrice)
    {
        WWWForm form = new WWWForm();
        form.AddField("title", inappName);
        form.AddField("description", "Buy " + inappName);
        form.AddField("payload", GenerateSecureToken(32));
        form.AddField("provider_token", "");
        form.AddField("currency", "XTR");
        form.AddField("prices", labelPrice);

        string createInvoiceUrl = "https://api.telegram.org/bot" + botToken + "/createInvoiceLink";

        using (UnityWebRequest request = UnityWebRequest.Post(createInvoiceUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("POST Success: " + request.downloadHandler.text);
                //message.text = request.downloadHandler.text;
                JObject jsonObject = JObject.Parse(request.downloadHandler.text);
                invoiceLink = (string)jsonObject["result"];
                ClickOpenInvoice();
            }
            else
            {
                Debug.Log("POST Error: " + request.error);
                //message.text = request.error;
            }
        }
    }

    [DllImport("__Internal")]
    private static extern void OpenInvoice(string mess);

    public void ClickOpenInvoice()
    {
        OpenInvoice(invoiceLink);
    }

    public void ShowLink(string mess)
    {
        Debug.Log(mess);
    }

    public void ShowPurchaseLog(string mess)
    {
        //CoinManager.Instance.AddCoin(5000);
        //PlayfabManager.Instance.SetUserData();
    }
}

class Test
{
    public bool ok;
    public string result;
}

class PriceLabel
{
    public string label { get; set; }
    public int amount { get; set; }
}

[System.Serializable]
public class CoinPack
{
    public int coin;
    public int price;
}