using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenue : AbstractUi
{
	[Header("Navigation")]
	[SerializeField] private Button _backButton;

	[Header("Graphics UI")] [SerializeField]
	private Toggle _anisotrophicFilteringToggle;

	[SerializeField] private Toggle _shadowsToggle;
	[SerializeField] private ShadowResolution _defaultShadowResolution = ShadowResolution.High;
	[SerializeField] private Slider _shadowQualitySlider;
	[SerializeField] private Text _shadowQualityText;

	[SerializeField] private Slider _antiAliasingSlider;
	[SerializeField] private Text _antiAliasingText;
	
	[SerializeField] private Slider _textureSizeSlider;
	[SerializeField] private Text _textureSizeText;

	[Header("Volume UI")] 
	[SerializeField] private Slider _masterVolumeSlider;
	[SerializeField] private Text _masterVolumeText;
	[SerializeField] private Slider _effectVolumeSlider;
	[SerializeField] private Text _effectVolumeText;
	[SerializeField] private Slider _musicVolumeSlider;
	[SerializeField] private Text _musicVolumeText;
	[SerializeField] private Slider _uiVolumeSlider;
	[SerializeField] private Text _uiVolumeText;
	[SerializeField] private AudioMixer _audioMixer;

	private MainMenue _mainMenueUi;

	// Use this for initialization
	void Start()
	{
		_mainMenueUi = FindObjectOfType<MainMenue>();

		// Navigation setup
		_backButton.onClick.AddListener(OnBackClick);

		// Graphics Ui setup
		_antiAliasingSlider.onValueChanged.AddListener(OnAntialiasingChange);
		_antiAliasingSlider.wholeNumbers = true;
		_antiAliasingSlider.minValue = 0;
		_antiAliasingSlider.maxValue = 3;
		_shadowQualitySlider.onValueChanged.AddListener(OnShadowQualityChange);
		_shadowQualitySlider.wholeNumbers = true;
		_shadowQualitySlider.minValue = 0;
		_shadowQualitySlider.maxValue = 3;
		_textureSizeSlider.onValueChanged.AddListener(OnTextureSizeChange);
		_textureSizeSlider.wholeNumbers = true;
		_textureSizeSlider.minValue = 0;
		_textureSizeSlider.maxValue = 2;

		_anisotrophicFilteringToggle.onValueChanged.AddListener(OnAnisotrophicFilteringChange);
		_shadowsToggle.onValueChanged.AddListener(OnShadowChange);

		// Volume Slider setup
		_masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
		_masterVolumeSlider.minValue = 0.001f;
		_masterVolumeSlider.maxValue = 1f;
		_effectVolumeSlider.onValueChanged.AddListener(OnEffectVolumeChange);
		_effectVolumeSlider.minValue = 0.001f;
		_effectVolumeSlider.maxValue = 1f;
		_musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
		_musicVolumeSlider.minValue = 0.001f;
		_musicVolumeSlider.maxValue = 1f;
		_uiVolumeSlider.onValueChanged.AddListener(OnUiVolumeChange);
		_uiVolumeSlider.minValue = 0.001f;
		_uiVolumeSlider.maxValue = 1f;
	}

	#region Navigation

	private void OnBackClick()
	{
		_mainMenueUi.SetVisible(true);
	}

	#endregion

	#region Graphic Options

	private void OnAnisotrophicFilteringChange(bool value)
	{
		QualitySettings.anisotropicFiltering = value ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
	}

	private void OnAntialiasingChange(float value)
	{
		switch ((int) value)
		{
			case 0:
				QualitySettings.antiAliasing = 0;
				_antiAliasingText.text = "Off";
				break;
			case 1:
				QualitySettings.antiAliasing = 2;
				_antiAliasingText.text = "2x Multisampling";
				break;
			case 2:
				QualitySettings.antiAliasing = 4;
				_antiAliasingText.text = "4x Multisampling";
				break;
			case 3:
				QualitySettings.antiAliasing = 8;
				_antiAliasingText.text = "8x Multisampling";
				break;
		}
	}

	private void OnShadowChange(bool value)
	{
		QualitySettings.shadows = value ? ShadowQuality.All : ShadowQuality.Disable;
	}

	private void OnShadowQualityChange(float value)
	{
		switch ((int)value)
		{
			case 0:
				QualitySettings.shadowResolution = ShadowResolution.Low;
				break;
			case 1:
				QualitySettings.shadowResolution = ShadowResolution.Medium;
				break;
			case 2:
				QualitySettings.shadowResolution = ShadowResolution.High;
				break;
			case 3:
				QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
				break;
			default:
				QualitySettings.shadowResolution = _defaultShadowResolution;
				break;
		}
	}

	private void OnTextureSizeChange(float value)
	{
		QualitySettings.masterTextureLimit = (int)value;
		switch (QualitySettings.masterTextureLimit)
		{
			case 0:
				_textureSizeText.text = "High";
				break;
			case 1:
				_textureSizeText.text = "Medium";
				break;
			case 2:
				_textureSizeText.text = "Low";
				break;
			default:
				_textureSizeText.text = "Default";
				break;
		}
	}
	#endregion

	#region Audio Options
	private void OnMasterVolumeChange(float value)
	{
		_audioMixer.SetFloat("MasterVolume", Mathf.Log(value) * 20);
		_masterVolumeText.text = Mathf.RoundToInt(value*100f).ToString();
	}

	private void OnEffectVolumeChange(float value)
	{
		_audioMixer.SetFloat("EffectVolume", Mathf.Log(value) * 20);
		_effectVolumeText.text = Mathf.RoundToInt(value * 100f).ToString();
	}

	private void OnMusicVolumeChange(float value)
	{
		_audioMixer.SetFloat("MusicVolume", Mathf.Log(value) * 20);
		_musicVolumeText.text = Mathf.RoundToInt(value * 100f).ToString();
	}

	private void OnUiVolumeChange(float value)
	{
		_audioMixer.SetFloat("UiVolume", Mathf.Log(value) * 20);
		_uiVolumeText.text = Mathf.RoundToInt(value * 100f).ToString();
	}
	#endregion

	public override void Reset()
	{
	}

	protected override void OnVisibilityChange(bool visible)
	{
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; // bool
		QualitySettings.antiAliasing = 1; // 1x Multisampling // int
		QualitySettings.shadows = ShadowQuality.All; // bool
		QualitySettings.shadowResolution = ShadowResolution.High; // Low, Medium, High, Very High
		QualitySettings.masterTextureLimit = 0; // 1 = half, 2 = quarter, 3 = eighth
	}
}