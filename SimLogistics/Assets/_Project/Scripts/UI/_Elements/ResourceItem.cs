using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceItem : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private TMP_Text pricingText;
    [SerializeField] private TMP_Text rateText;

    public Sprite Thumbnail
    {
        get => thumbnail.sprite;
        set => thumbnail.sprite = value;
    }
    public string Label
    {
        get => labelText.text;
        set => labelText.text = value;
    }
    public string Volume
    {
        get => volumeText.text;
        set => volumeText.text = value;
    }
    public string Pricing
    {
        get => pricingText.text;
        set => pricingText.text = value;
    }
    public string Rate
    {
        get => rateText.text;
        set => rateText.text = value;
    }
}
