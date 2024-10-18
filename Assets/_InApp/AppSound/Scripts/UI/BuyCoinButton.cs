using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuyCoinButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private TextMeshProUGUI _priceText;

    [SerializeField] private int _index;

    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            SetDataWithIndex();
        }
    }

    private int _coin;

    private void OnValidate()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _button?.onClick.AddListener(OnClickButton);
    }

    private void SetDataWithIndex()
    {
        var data = TelePurchase.Instance.CoinPack[_index];

        _priceText.SetText($"{data.price} STARS");
        _text.SetText($"{data.coin}");
    }

    private void OnClickButton()
    {
        TelePurchase.Instance.BuyProductID(_index);
    }
}