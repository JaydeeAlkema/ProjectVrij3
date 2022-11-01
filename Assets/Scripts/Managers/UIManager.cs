using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

	[SerializeField] private Slider expBarSlider;
	[SerializeField] private Slider hpBarSlider;
	[SerializeField] private TMP_Text pointText;

	[SerializeField] private GameObject[] UIStates;
	// 0 = Hub UI
	// 1 = Dungeon UI
	// 2 = Generation Loading Screen
	// 3 = Pause Screen
	// 4 = Death Screen

	[SerializeField] private Transform[] DevUIComponents;
	// 0 = Melee Upgrades
	// 1 = Cast Upgrades

	void Update()
	{
		SetHP(GameManager.Instance.PlayerHP);
		SetExp(GameManager.Instance.ExpManager.PlayerExp);
		pointText.text = GameManager.Instance.ExpManager.PlayerPoints.ToString();

		if (Input.GetKeyDown(KeyCode.P))
		{
			GameManager.Instance.ExpManager.AddExp(5);
		}
	}

	public void SetupDungeonUI()
	{
		SetExpBar(GameManager.Instance.ExpManager.ExpToNextPoint);
		SetHPBar(GameManager.Instance.PlayerHP);
	}

	public void SetUIActive(int uiStateNumber, bool isActive)
	{
		UIStates[uiStateNumber].SetActive(isActive);
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

	public void AddDevText(int textComponent, string addText)
	{
		DevUIComponents[textComponent].GetComponent<TMP_Text>().text += (addText + ", ");
	}

}
