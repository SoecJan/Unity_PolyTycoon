using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityGrowthBehaviour : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private Vector2Int _size;
    [SerializeField] private Image _image;
    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private int _buildingBlockSize = 4;
    [SerializeField]
    private float _buildingScale = 1f;

    private BuildingManager _buildingManager;
    private List<GameObject> cubes;

    // Start is called before the first frame update
    void Start()
    {
        _buildingManager = new BuildingManager();
        cubes = new List<GameObject>();
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            Generate();
        }
    }

    void Generate()
    {
        float[,] noise = Noise.GenerateNoiseMap(_size.x, _size.y, _noiseSettings, Vector2.zero);
        
        Texture2D texture2D = new Texture2D(_size.x, _size.y);
        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                texture2D.SetPixel(x, y, new Color(noise[x, y], noise[x, y], noise[x, y], 1f) * (_animationCurve.Evaluate(((float)y)/noise.GetLength(1)) * _animationCurve.Evaluate(((float)x)/noise.GetLength(0))));
            }
        }
        texture2D.Apply();
        this._image.sprite = Sprite.Create(texture2D, new Rect(0, 0, _size.x, _size.y), Vector2.zero);

        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();
        
        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                if (x % _buildingBlockSize == 0 || y % _buildingBlockSize == 0) continue;
//                float height = Mathf.RoundToInt((0.5f + (_animationCurve.Evaluate(((float)y)/noise.GetLength(1)) * _animationCurve.Evaluate(((float)x)/noise.GetLength(0)) * _buildingScale * noise[x, y]))*4f)/4f;
//
//                GameObject parent = new GameObject("Procedural Building", ProceduralCityBuilding);
//                ProceduralCityBuilding cityBuilding = parent.GetComponent<ProceduralCityBuilding>();
//                cityBuilding.Height = height;
//                _buildingManager.AddMapPlaceable(cityBuilding);
//                
//                cubes.Add(obj);
            }
        }
    }
}