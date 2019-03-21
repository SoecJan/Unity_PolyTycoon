using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleplayerMenu : AbstractUi
{
    [Header("Navigation")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _playButton;
    [Header("Settings")]
    [SerializeField] private Button _settingBackButton;
    [SerializeField] private Button _settingNextButton;
    [SerializeField] private Animator _animator;

    private PlayMenue _playMenu;

    // Use this for initialization
    void Start()
    {
        _playMenu = FindObjectOfType<PlayMenue>();
        _backButton.onClick.AddListener(OnBackClick);
        _playButton.onClick.AddListener(OnPlayClick);
        _settingBackButton.onClick.AddListener(delegate
        {
            _animator.SetTrigger("Back");
        });
        _settingNextButton.onClick.AddListener(delegate
        {
            _animator.SetTrigger("Next");
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

    protected new void OnVisibilityChange(bool visible)
    { }
}
