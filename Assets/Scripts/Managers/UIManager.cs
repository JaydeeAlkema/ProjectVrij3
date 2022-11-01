using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private Slider expBarSlider;
	[SerializeField] private Slider hpBarSlider;
	[SerializeField] private TMP_Text pointText;

	//TODO: Create custom struct that holds UI elements so we don't have to user indeces for enabling/disabling UI, but instead call them by name/type etc.
	[SerializeField] private GameObject[] uiStates;

	public GameObject[] UiStates { get => uiStates; private set => uiStates = value; }

	// 0 = Hub UI
	// 1 = Dungeon UI
	// 2 = Generation Loading Screen
	// 3 = Pause Screen
	// 4 = Death Screen
	// 5 = Cheat Menu

	void Update()
	{
		SetHP(GameManager.Instance.PlayerHP.value);
		SetExp(GameManager.Instance.ExpManager.PlayerExp);
		pointText.text = GameManager.Instance.ExpManager.PlayerPoints.ToString();

		if (Input.GetKeyDown(KeyCode.P) && uiStates[5].activeInHierarchy == false)
		{
			GameManager.Instance.ExpManager.AddExp(5);
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (uiStates[5].activeInHierarchy)
			{
				SetUIActive(5, false);
			}
			else
			{
				SetUIActive(5, true);
			}
		}
	}

	public void SetupDungeonUI()
	{
		SetExpBar(GameManager.Instance.ExpManager.ExpToNextPoint);
		SetHPBar(GameManager.Instance.PlayerHP.startValue);
	}

	public void SetUIActive(int uiStateNumber, bool isActive)
	{
		uiStates[uiStateNumber].SetActive(isActive);
	}

	public void DisableAllUI()
	{
		foreach (GameObject uiState in uiStates)
		{
			uiState.SetActive(false);
		}
	}

	public void SetHPBar(int maxHP)
	{
		hpBarSlider.maxValue = maxHP;
		hpBarSlider.value = maxHP;
	}

	public void SetExpBar(int maxExp)
	{
		expBarSlider.maxValue = maxExp;
		expBarSlider.value = maxExp;
	}

	public void SetHP(int hp)
	{
		hpBarSlider.value = hp;
	}

	public void SetExp(int exp)
	{
		expBarSlider.value = exp;
	}

	public void ResetCheatMenu()
	{
		uiStates[5].GetComponentInChildren<TMP_InputField>().text = string.Empty;
	}
}
