using UnityEngine;
using System.Collections;

public static class Noise
{

	public static float[,] GenerateRoundedNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		System.Random prng = new System.Random(settings.seed);
		Vector2[] octaveOffsets = new Vector2[settings.octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < settings.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
			float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= settings.persistance;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < settings.octaves; i++)
				{
					float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
					float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= settings.persistance;
					frequency *= settings.lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = Mathf.Round(noiseHeight);

				float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
				noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
			}
		}
		return noiseMap;
	}
	
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		System.Random prng = new System.Random(settings.seed);
		Vector2[] octaveOffsets = new Vector2[settings.octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < settings.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
			float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= settings.persistance;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < settings.octaves; i++)
				{
					float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
					float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= settings.persistance;
					frequency *= settings.lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;

				float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
				noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
			}
		}
		return noiseMap;
	}
}

