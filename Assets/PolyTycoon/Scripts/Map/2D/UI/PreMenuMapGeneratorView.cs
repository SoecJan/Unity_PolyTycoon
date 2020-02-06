using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PreMenuMapGeneratorView : MonoBehaviour
{
    private MapPreview[] _mapPreview;
    private MapGenerator2D _mapGenerator2D;
    [SerializeField] private TMP_InputField _seedInputField;
    [SerializeField] private Button _randomButton;

    [SerializeField] private Slider _lacunaritySlider;
    
    // Start is called before the first frame update
    void Start()
    {
        _mapPreview = FindObjectsOfType<MapPreview>();
        _mapGenerator2D = FindObjectOfType<MapGenerator2D>();
        // Seed Input
        _seedInputField.text = _mapGenerator2D.heightMapSettings.noiseSettings.seed.ToString();
        _seedInputField.onValueChanged.AddListener(delegate(string value) {
            try
            {
                SetSeed(int.Parse(value));
            }
            catch (FormatException exception)
            {
                _seedInputField.text = 0.ToString();
            }
        });
        _randomButton.onClick.AddListener(delegate
        {
            _seedInputField.text = Random.Range(0, 999999).ToString();
        });
        
        // Lacunarity Slider
        _lacunaritySlider.value = (_mapGenerator2D.heightMapSettings.noiseSettings.lacunarity - 1) / 0.75f;
        _lacunaritySlider.onValueChanged.AddListener(delegate(float value)
        {
            float mappedValue = 1 + (value * 0.75f);
            _mapGenerator2D.heightMapSettings.noiseSettings.lacunarity = mappedValue;
            _mapGenerator2D.RecalculateMap();
            foreach (MapPreview mapPreview in _mapPreview)
            {
                mapPreview.DrawMapInEditor();
            }
        });
    }

    private void SetSeed(int value)
    {
        _mapGenerator2D.heightMapSettings.noiseSettings.seed = value;
        _mapGenerator2D.RecalculateMap();
        foreach (MapPreview mapPreview in _mapPreview)
        {
            mapPreview.DrawMapInEditor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
