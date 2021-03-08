using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public string Text
    {
        get => text.text;
        set => text.text = value;
    }
}
