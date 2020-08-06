using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewRouteView : MonoBehaviour
{
    [Header("Navigation")] 
    [SerializeField] private GameObject _visibleObject;
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_InputField _routeNameInputfield;
    [SerializeField] private Button _createButton;
    private System.Action _onHide;

    private void Start()
    {
        _showButton.onClick.AddListener(delegate
        {
            SetVisible(!VisibleObject.activeSelf); 
        });
        _exitButton.onClick.AddListener(delegate
        {
            SetVisible(false);
        });
    }

    public TMP_InputField RouteNameInputfield
    {
        get => _routeNameInputfield;
        set => _routeNameInputfield = value;
    }

    public Button CreateButton
    {
        get => _createButton;
        set => _createButton = value;
    }

    public GameObject VisibleObject
    {
        get => _visibleObject;
        set => _visibleObject = value;
    }

    public Action OnHide
    {
        get => _onHide;
        set => _onHide = value;
    }

    public void Reset()
    {
        RouteNameInputfield.text = "";
    }

    public void SetVisible(bool visible)
    {
        VisibleObject.SetActive(visible);
        if (!visible) OnHide?.Invoke();
    }
}
