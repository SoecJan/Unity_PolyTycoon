using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
	public enum Biome { None, Grain, Fruit, Animal, Oil, FreshWater }

	public static BiomeData[] GenerateBiomeData(BiomeSettings biomeSettings, int width, int height, Vector2 sampleCoord)
	{
		BiomeData[] biomeData = new BiomeData[biomeSettings.Biomes.Length];
		for (int i = 0; i < biomeSettings.Biomes.Length; i++)
		{
			BiomeSetting biomeSetting = biomeSettings.Biomes[i];
			float[,] noiseArray = Noise.GenerateNoiseMap(width, height, biomeSetting.NoiseSettings, sampleCoord);
			biomeData[i] = new BiomeData(biomeSetting.Biome, noiseArray, biomeSetting.BiomeColorMultiplicatorVector3);
		}
		return biomeData;
	}
}

[CreateAssetMenu(fileName = "BiomeData", menuName = "PolyTycoon/BiomeData", order = 1)]
public class BiomeSettings : ScriptableObject
{
	[SerializeField] private BiomeSetting[] _biomes;

	public BiomeSettings(BiomeSetting[] biomes)
	{
		_biomes = biomes;
	}

	public BiomeSetting[] Biomes {
		get {
			return _biomes;
		}

		set {
			_biomes = value;
		}
	}
}

[Serializable]
public struct BiomeSetting
{
	[SerializeField] private BiomeGenerator.Biome _biome;
	[SerializeField] private NoiseSettings _noiseSettings;
	[SerializeField] private Vector3 _biomeColorMultiplicatorVector3;

	public NoiseSettings NoiseSettings {
		get {
			return _noiseSettings;
		}

		set {
			_noiseSettings = value;
		}
	}

	public BiomeGenerator.Biome Biome {
		get {
			return _biome;
		}

		set {
			_biome = value;
		}
	}

	public Vector3 BiomeColorMultiplicatorVector3 {
		get {
			return _biomeColorMultiplicatorVector3;
		}

		set {
			_biomeColorMultiplicatorVector3 = value;
		}
	}
}

public class BiomeData
{
	[SerializeField] private BiomeGenerator.Biome _biome;
	[SerializeField] private float[,] _arrayData;
	[SerializeField] private Material _material;
	[SerializeField] private Vector3 _colorMultiplier;

	public BiomeData(BiomeGenerator.Biome biome, float[,] arrayData, Vector3 colorMultiplier)
	{
		_biome = biome;
		_arrayData = arrayData;
		_colorMultiplier = colorMultiplier;
	}

	

	public BiomeGenerator.Biome Biome {
		get {
			return _biome;
		}

		set {
			_biome = value;
		}
	}

	public float[,] ArrayData {
		get {
			return _arrayData;
		}

		set {
			_arrayData = value;
		}
	}

	public Vector3 ColorMultiplier {
		get {
			return _colorMultiplier;
		}

		set {
			_colorMultiplier = value;
		}
	}

	public Material Material {
		get {
			return _material;
		}

		set {
			_material = value;
		}
	}
}
