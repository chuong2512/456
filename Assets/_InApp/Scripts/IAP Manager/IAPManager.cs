using System;
using System.Collections;
using SingleApp;
using UnityEngine;

public class IAPKey
{
    public const string PACK1 = "add1";
    public const string PACK2 = "add3";
    public const string PACK3 = "add5";
    public const string PACK4 = "add10";

    public const string PACK1_RE = "sub1";
    public const string PACK2_RE = "sub3";
    public const string PACK3_RE = "sub5";
    public const string PACK4_RE = "sub10";
}

public class IAPManager : PersistentSingleton<IAPManager>
{
    public static Action OnPurchaseSuccess;

    private bool _isBuyFromShop;


    public void BuyProductID(string productId)
    {
        
    }


    private void OnPurchaseComplete(string productId)
    {
        OnPurchaseSuccess?.Invoke();
    }

    private void BuyPack()
    {
        //todo: buy pack
    }
}