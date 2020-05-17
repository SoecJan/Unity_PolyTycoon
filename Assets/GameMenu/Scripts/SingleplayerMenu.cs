using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleplayerMenu : AbstractUi
{
    [Header("Navigation")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _playButton;

    [SerializeField] private Slider _mapSizeSlider;
    [SerializeField] private TMP_Text _mapSizeText;
    [SerializeField] private TMP_InputField _budgetInputField;
    [SerializeField] private Image _playerColorInput;
    [SerializeField] private Button _playerColorChangeButton;

    private PlayMenue _playMenu;

    // Use this for initialization
    void Start()
    {
        _playMenu = FindObjectOfType<PlayMenue>();
        _backButton.onClick.AddListener(OnBackClick);
        _playButton.onClick.AddListener(OnPlayClick);
        _mapSizeText.text = "Map size: " + ((_mapSizeSlider.value < _mapSizeSlider.maxValue) ? _mapSizeSlider.value.ToString() : "Infinite");
        _mapSizeSlider.onValueChanged.AddListener(delegate(float value)
        {
            _mapSizeText.text = "Map size: " + ((value < _mapSizeSlider.maxValue) ? value.ToString() : "Infinite");
        });
        _budgetInputField.onValueChanged.AddListener(delegate(string value) { Debug.Log(value); });
        
        _playerColorInput.color = Color.blue;
        _playerColorChangeButton.onClick.AddListener(delegate
        {
            if (_playerColorInput.color.Equals(Color.blue))
            {
                _playerColorInput.color = Color.red;
            }
            else if (_playerColorInput.color.Equals(Color.red))
            {
                _playerColorInput.color = Color.green;
            }
            else if (_playerColorInput.color.Equals(Color.green))
            {
                _playerColorInput.color = Color.cyan;
            }
            else if (_playerColorInput.color.Equals(Color.cyan))
            {
                _playerColorInput.color = Color.blue;
            }
        });
    }

    private void OnBackClick()
    {
        SetVisible(false);
        _playMenu.SetVisible(true);
    }

    private void OnPlayClick()
    {
        Debug.Log("Play!");
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while (!operation.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public new void Reset()
    { }
}
