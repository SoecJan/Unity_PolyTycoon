using System;
using UnityEngine;
using UnityEngine.UI;

public class MapChunk2D : MonoBehaviour
{
    private Image _image;

    private void Start()
    {
        _image = gameObject.AddComponent<Image>();
        RectTransform imageTransform = ((RectTransform) _image.transform);
        imageTransform.anchoredPosition = Vector2.zero;
        imageTransform.localScale = Vector3.one;
    }

    public void OnHeightMapReceive(object heightMapObject)
    {
        HeightMap heightMap = (HeightMap) heightMapObject;
        int width = heightMap.values.GetLength(0) - 1;
        int height = heightMap.values.GetLength(1) - 1;
        Texture2D texture2D = GenerateTexture(heightMap);
        _image.sprite = Sprite.Create(texture2D, Rect.MinMaxRect(0, 0, width, height), Vector2.up, 1);
    }

    private static Texture2D GenerateTexture(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0) - 1;
        int height = heightMap.values.GetLength(1) - 1;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false, false)
        {
            filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp
        };

        for (int x = 0; x < heightMap.values.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < heightMap.values.GetLength(1) - 1; y++)
            {
                float x0 = heightMap.values[x, y];
                float x1 = heightMap.values[x + 1, y];
                float y0 = heightMap.values[x, y + 1];
                float y1 = heightMap.values[x + 1, y + 1];

                float average = (x0 + x1 + y0 + y1) / 4;
                
                if (average >= 0.9f) // Snow
                {
                    texture2D.SetPixel(x, y, new Color(255 / 255f, 255 / 255f, 255 / 255f, 1));
                }
                else if (average >= 0.3f) // Mountain
                {
                    texture2D.SetPixel(x, y, new Color(29 / 255f, 57 / 255f, 30 / 255f, 1));
                }
                else if (average >= 0.2f) // Grass
                {
                    texture2D.SetPixel(x, y, new Color(37 / 255f, 128 / 255f, 48 / 255f, 1));
                }
                else // Water
                {
                    texture2D.SetPixel(x, y, new Color(32 / 255f, 60 / 255f, 192 / 255f, 1));
                }
            }
        }

        texture2D.Apply();
        return texture2D;
    }
}