using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionUnlockViewElement : MonoBehaviour
{
    [SerializeField] private Image _image;

    public Image Image
    {
        get => _image;
        set => _image = value;
    }
}
