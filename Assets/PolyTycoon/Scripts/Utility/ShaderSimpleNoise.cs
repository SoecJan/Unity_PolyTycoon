using System;
using UnityEngine;

public class ShaderSimpleNoise : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
//        Debug.Log(Mathf.SmoothStep(0f, 0.34f, this.SimpleNoiseValue(new Vector2(transform.position.x, transform.position.z), 0.1f)));
    }

    public float SimpleNoiseValue(Vector2 position, float scale)
    {
        float t = 0.0f;

        float freq = 1f;
        float amp = (float) Math.Pow(0.5f, 3f);
        t += unity_valueNoise(new Vector2(position.x * scale / freq, position.y * scale / freq)) * amp;

        freq = (float) Math.Pow(2.0f, 1f);
        amp = (float) Math.Pow(0.5f, 2f);
        t += unity_valueNoise(new Vector2(position.x * scale / freq, position.y * scale / freq)) * amp;

        freq = (float) Math.Pow(2.0f, 2f);
        amp = (float) Math.Pow(0.5f, 1f);
        t += unity_valueNoise(new Vector2(position.x * scale / freq, position.y * scale / freq)) * amp;

        return t;
    }
    
    float unity_valueNoise (Vector2 uv)
    {
        Vector2 i = Vector2Int.FloorToInt(uv);
        Vector2 f = uv - i;
        Vector2 frac = uv - i;
        f = f * f * f;

        uv = frac - (0.5f * Vector2.one);
        Vector2 c0 = i + new Vector2(0.0f, 0.0f);
        Vector2 c1 = i + new Vector2(1.0f, 0.0f);
        Vector2 c2 = i + new Vector2(0.0f, 1.0f);
        Vector2 c3 = i + new Vector2(1.0f, 1.0f);
        float r0 = unity_noise_randomValue(c0);
        float r1 = unity_noise_randomValue(c1);
        float r2 = unity_noise_randomValue(c2);
        float r3 = unity_noise_randomValue(c3);

        float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
        float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
        float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
        return t;
    }
    
    float unity_noise_randomValue (Vector2 uv)
    {
        float product = (float) (Math.Sin(uv.x * 12.9898f + uv.y * 78.233f) * 43758.5453f);
        return product - (float) Math.Floor(product);
    }

    float unity_noise_interpolate (float a, float b, float t)
    {
        return (1.0f-t)*a + (t*b);
    }
}