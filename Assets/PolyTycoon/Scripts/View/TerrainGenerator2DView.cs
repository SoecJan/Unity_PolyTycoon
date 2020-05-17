using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TerrainGenerator2DView : MonoBehaviour
{
    private MapPreview[] _mapPreviews;
    private TerrainGenerator2D _terrainGenerator2D;
    
    [SerializeField] private TMP_InputField _seedInputField;
    [SerializeField] private Button _randomButton;
    [SerializeField] private Slider _lacunaritySlider;
    
    // Start is called before the first frame update
    void Start()
    {
        _mapPreviews = FindObjectsOfType<MapPreview>();
        _terrainGenerator2D = new TerrainGenerator2D(transform);
        // Seed Input
        _seedInputField.text = _terrainGenerator2D.MapSettings.noiseSettings.seed.ToString();
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
        _lacunaritySlider.value = (_terrainGenerator2D.MapSettings.noiseSettings.lacunarity - 1) / 0.75f;
        _lacunaritySlider.onValueChanged.AddListener(delegate(float value)
        {
            float mappedValue = 1 + (value * 0.75f);
            _terrainGenerator2D.MapSettings.noiseSettings.lacunarity = mappedValue;
            _terrainGenerator2D.RecalculateChunk();
            foreach (MapPreview mapPreview in _mapPreviews)
            {
                mapPreview.DrawMapInEditor();
            }
        });
    }

    private void SetSeed(int value)
    {
        _terrainGenerator2D.MapSettings.noiseSettings.seed = value;
        _terrainGenerator2D.RecalculateChunk();
        foreach (MapPreview mapPreview in _mapPreviews)
        {
            mapPreview.DrawMapInEditor();
        }
    }
}
