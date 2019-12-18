using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldUtility : MonoBehaviour
{
    public static System.Action _onInputFieldSelect;
    public static System.Action _onInputFieldDeselect;

    private void Start()
    {
        TMP_InputField inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(delegate { _onInputFieldSelect?.Invoke(); });
        inputField.onDeselect.AddListener(delegate { _onInputFieldDeselect?.Invoke(); });
    }
}
