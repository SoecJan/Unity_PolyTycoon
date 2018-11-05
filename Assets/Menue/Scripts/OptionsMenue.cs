using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenue : AbstractUi
{
	[Header("Navigation")]
	[SerializeField] private Button _backButton;

	[Header("Graphics UI")] 
	[SerializeField] private Toggle _anisotrophicFilteringToggle;

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
		SetVisible(false);
		_mainMenueUi.SetVisible(true);
	}

	private void Load()
	{
		// Graphics Settings
		_anisotrophicFilteringToggle.isOn = QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable;
		_shadowsToggle.isOn = QualitySettings.shadows == ShadowQuality.All;
		_shadowQualitySlider.value = QualitySettings.shadowResolution == ShadowResolution.Low ? 0 : QualitySettings.shadowResolution == ShadowResolution.Medium ? 1 : QualitySettings.shadowResolution == ShadowResolution.High ? 2 : QualitySettings.shadowResolution == ShadowResolution.VeryHigh ? 3 : 2;
		_antiAliasingSlider.value = QualitySettings.antiAliasing == 0 ? 0 :
			QualitySettings.antiAliasing == 2 ? 2 :
			QualitySettings.antiAliasing == 4 ? 4 :
			QualitySettings.antiAliasing == 8 ? 8 : 0;
		_textureSizeSlider.value = QualitySettings.masterTextureLimit == 2 ? 0 :
			QualitySettings.masterTextureLimit == 1 ? 1 :
			QualitySettings.masterTextureLimit == 0 ? 2 : 0;

		// Audio Settings
		float masterVolume;
		_audioMixer.GetFloat("MasterVolume", out masterVolume);
		_masterVolumeSlider.value = Mathf.Exp(masterVolume / 20);
		float musicVolume;
		_audioMixer.GetFloat("MusicVolume", out musicVolume);
		_musicVolumeSlider.value = Mathf.Exp(musicVolume / 20);
		float effectVolume;
		_audioMixer.GetFloat("EffectVolume", out effectVolume);
		_effectVolumeSlider.value = Mathf.Exp(effectVolume / 20);
		float uiVolume;
		_audioMixer.GetFloat("UiVolume", out uiVolume);
		_uiVolumeSlider.value = Mathf.Exp(uiVolume / 20);
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
				_antiAliasingText.text = "2x";
				break;
			case 2:
				QualitySettings.antiAliasing = 4;
				_antiAliasingText.text = "4x";
				break;
			case 3:
				QualitySettings.antiAliasing = 8;
				_antiAliasingText.text = "8x";
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
				_shadowQualityText.text = "Low";
				break;
			case 1:
				QualitySettings.shadowResolution = ShadowResolution.Medium;
				_shadowQualityText.text = "Medium";
				break;
			case 2:
				QualitySettings.shadowResolution = ShadowResolution.High;
				_shadowQualityText.text = "High";
				break;
			case 3:
				QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
				_shadowQualityText.text = "Very High";
				break;
			default:
				QualitySettings.shadowResolution = _defaultShadowResolution;
				break;
		}
	}

	private void OnTextureSizeChange(float value)
	{
		switch ((int)value)
		{
			case 0:
				_textureSizeText.text = "Low";
				QualitySettings.masterTextureLimit = 2;
				break;
			case 1:
				_textureSizeText.text = "Medium";
				QualitySettings.masterTextureLimit = 1;
				break;
			case 2:
				_textureSizeText.text = "High";
				QualitySettings.masterTextureLimit = 0;
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
	{}

	protected override void OnVisibilityChange(bool visible)
	{
		if (visible)
		{
			Load();
		}
	}
}