using TMPro;
using UnityEngine;

/// <summary>
/// Subscribes to all InputFields in a given Scene and emits a boolean event on selection.
/// This is used to check if any InputField is activated or not.
/// </summary>
public class InputFieldSelectionUtility
{
    public static System.Action<bool> OnSelectionChange;

    public InputFieldSelectionUtility()
    {
        SubscribeToAllInputFields();
    }

    private void SubscribeToAllInputFields()
    {
        TMP_InputField[] tmpInputFields = Object.FindObjectsOfType<TMP_InputField>();
        foreach (TMP_InputField tmpInputField in tmpInputFields)
        {
            tmpInputField.onSelect.RemoveAllListeners();
            tmpInputField.onSelect.AddListener(delegate { OnSelectionChange?.Invoke(true); });
            
            tmpInputField.onDeselect.RemoveAllListeners();
            tmpInputField.onDeselect.AddListener(delegate { OnSelectionChange?.Invoke(false); });
        }
    }
}
