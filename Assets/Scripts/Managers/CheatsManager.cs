using UnityEngine;

public class CheatsManager : MonoBehaviour
{
	public void ExecuteCommand(string command)
	{
		string[] splitcommand = command.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

		string key = string.Empty;
		string value = string.Empty;

		if (splitcommand.Length >= 1) key = splitcommand[0].ToLower();
		if (splitcommand.Length >= 2) value = splitcommand[1].ToLower();

		CircleCollider2D playerCollider = GameManager.Instance.PlayerInstance.GetComponent<CircleCollider2D>();
		ScriptableInt playerHP = GameManager.Instance.PlayerHP;
		ScriptableFloat playerSpeed = GameManager.Instance.PlayerSpeed;

		switch (key)
		{
			#region Player
			case "noclip":
				playerCollider.enabled = !playerCollider.enabled;
				break;
			case "sethp":
				playerHP.value = int.Parse(value);
				break;
			case "resethp":
				playerHP.value = playerHP.startValue;
				break;
			case "addhp":
				playerHP.value += int.Parse(value);
				break;
			case "removehp":
				playerHP.value -= int.Parse(value);
				break;
			case "setmovespeed":
				playerSpeed.value = float.Parse(value);
				break;
			case "resetmovespeed":
				playerSpeed.value = playerSpeed.startValue;
				break;
			case "addmovespeed":
				playerSpeed.value += float.Parse(value);
				break;
			case "removemovespeed":
				playerSpeed.value -= float.Parse(value);
				break;
			#endregion

			default:
				break;
		}

		GameManager.Instance.UiManager.ResetCheatMenu();
		GameManager.Instance.UiManager.SetUIActive(5, false);
	}
}
