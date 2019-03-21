using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using commands;

public class SceneController : MonoBehaviour, IEventListener {

	public bool DEBUG = true;

	public enum Scenes
	{
		PolyTycoon, MainMenu
	}

	// Use this for initialization
	void Start () {
		EventManager.instance.AddListener(this as IEventListener, "commands.GameStartCommand");
		SceneManager.LoadScene("MainMenue", LoadSceneMode.Additive);
	}

	public bool HandleEvent(IEvent evt)
	{
		if (evt.GetData() is GameStartCommand)
		{
			HandleGameStartCommand((GameStartCommand)evt.GetData());
		}
		
		return false;
	}

	private void HandleGameStartCommand (GameStartCommand gameStartCommand)
	{
		switch (gameStartCommand.GameName)
		{
			case "Tower Defense":
				SceneManager.LoadScene("TowerDefense", LoadSceneMode.Additive);
				break;
			case "BlockShooter":
				SceneManager.LoadScene("BlockShooter", LoadSceneMode.Additive);
				break;
			case "PolyTycoon":
				SceneManager.LoadScene("BlockShooter", LoadSceneMode.Additive);
				break;
			default:
				Debug.LogError("Game not found");
				break;
		}
		SceneManager.UnloadSceneAsync("ConnectUI");
	}

	public void LoadScene(Scenes scene)
	{
		switch (scene)
		{
			case Scenes.PolyTycoon:
				TransitionController transitionController = FindObjectOfType<TransitionController>();
				transitionController.StartTransition();
				AsyncOperation operation = SceneManager.UnloadSceneAsync("MainMenue");
				StartCoroutine(LoadSceneCoroutine(operation, transitionController));
				break;
			case Scenes.MainMenu:
				SceneManager.UnloadSceneAsync("PolyTycoon");
				SceneManager.LoadScene("MainMenue", LoadSceneMode.Additive);
				break;
		}
	}

	private IEnumerator LoadSceneCoroutine(AsyncOperation operation, TransitionController transitionController)
	{
		while (!operation.isDone)
		{
			yield return null;
		}
		SceneManager.LoadScene("PolyTycoon", LoadSceneMode.Additive);
		yield return new WaitForSeconds(1);
		transitionController.EndTransition();
	}
}
