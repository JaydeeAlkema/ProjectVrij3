using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	[SerializeField] private Slider expBarSlider;
	[SerializeField] private Slider hpBarSlider;
	[SerializeField] private TMP_Text pointText;

	//TODO: Create custom struct that holds UI elements so we don't have to user indeces for enabling/disabling UI, but instead call them by name/type etc.
	[SerializeField] private GameObject[] UIStates;
	// 0 = Hub UI
	// 1 = Dungeon UI
	// 2 = Generation Loading Screen
	// 3 = Cheat Menu

	void Start()
	{
		SetExpBar(GameManager.Instance.ExpManager.ExpToNextPoint);
		SetHPBar(GameManager.Instance.PlayerHP);
	}

	void Update()
	{
		SetHP(GameManager.Instance.PlayerHP);
		SetExp(GameManager.Instance.ExpManager.PlayerExp);
		pointText.text = GameManager.Instance.ExpManager.PlayerPoints.ToString();

		if (Input.GetKeyDown(KeyCode.P))
		{
			GameManager.Instance.ExpManager.AddExp(5);
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (UIStates[3].activeInHierarchy)
			{
				DisableUI(3);
				Time.timeScale = 1;
			}
			else
			{
				EnableUI(3);
				Time.timeScale = 0;
			}
		}
	}

	public void EnableUI(int uiStateNumber)
	{
		UIStates[uiStateNumber].SetActive(true);
	}

	public void DisableUI(int uiStateNumber)
	{
		UIStates[uiStateNumber].SetActive(false);
	}

	public void DisableAllUI()
	{
		foreach (GameObject uiState in UIStates)
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

}
