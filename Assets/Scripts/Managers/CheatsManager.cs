using UnityEngine;

public class CheatsManager : MonoBehaviour
{
	public void ExecuteCommand(string command)
	{
		string key = command.Split(' ')[0].ToLower();
		string value = command.Split(' ')[1];

		switch (key)
		{
			case "sethp":
				GameManager.Instance.PlayerHP.value = int.Parse(value);
				break;

			default:
				break;
		}

		GameManager.Instance.UiManager.ResetCheatMenu();
		GameManager.Instance.UiManager.SetUIActive(5, false);
	}
}
