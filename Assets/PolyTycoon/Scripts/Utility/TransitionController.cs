using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
	[SerializeField] private GameObject _transitionGameObject;

	//private GameSettingManager _gameSettingManager;
	//private TerrainGenerator _terrainGenerator;

	private MoneyController _moneyController;
	private CityManager _cityManager;
	private CompanyInformationView _companyUi;

	public void StartTransition()
	{
		_transitionGameObject.SetActive(true);
	}

	public void EndTransition()
	{
		//_gameSettingManager = FindObjectOfType<GameSettingManager>();
		//_terrainGenerator = FindObjectOfType<TerrainGenerator>();
		//_terrainGenerator.heightMapSettings.noiseSettings.seed = _gameSettingManager.Setting.WorldSeed;
		//switch (_gameSettingManager.Setting.WorldSize)
		//{
		//	case 0:
		//		_terrainGenerator.detailLevels[0].visibleDstThreshold = 10;
		//		TerrainChunk.maxViewDst = 10;
		//		break;
		//	case 1:
		//		_terrainGenerator.detailLevels[0].visibleDstThreshold = 50;
		//		TerrainChunk.maxViewDst = 50;
		//		break;
		//	case 2:
		//		_terrainGenerator.detailLevels[0].visibleDstThreshold = 50;
		//		TerrainChunk.maxViewDst = 50;
		//		_terrainGenerator.viewer = Camera.main.transform;
		//		break;
		//}
		//_terrainGenerator.UpdateVisibleChunks();

		//_moneyController = FindObjectOfType<MoneyController>();
		//_moneyController.PlayerMoney = _gameSettingManager.Setting.StartMoney;

		//_companyUi = FindObjectOfType<CompanyUi>();
		//_companyUi.CompanyName = _gameSettingManager.Setting.CompanyName;
		//_companyUi.CompanyColor = _gameSettingManager.Setting.CompanyColor;
		
		//_cityManager = FindObjectOfType<CityManager>();
		//for (int i = 0; i < _gameSettingManager.Setting.CityCount; i++)
		//{
		//	_cityManager.AddRandomCity();
		//}

		//Debug.Log("Win Condition: " + _gameSettingManager.Setting.WinCondition);
		_transitionGameObject.SetActive(false);
	}
}
