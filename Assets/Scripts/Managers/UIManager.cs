using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
	[SerializeField] private Slider expBarSlider;
	[SerializeField] private Slider hpBarSlider;
	[SerializeField] private TMP_Text pointText;
	[SerializeField] private TMP_Text hpAmountText;
	[SerializeField] private TMP_Text expAmountText;

	//TODO: Create custom struct that holds UI elements so we don't have to user indeces for enabling/disabling UI, but instead call them by name/type etc.
	[SerializeField] private GameObject[] uiStates;

	public GameObject[] UiStates { get => uiStates; private set => uiStates = value; }

	// 0 = Hub UI
	// 1 = Dungeon UI
	// 2 = Generation Loading Screen
	// 3 = Pause Screen
	// 4 = Death Screen
	// 5 = Cheat Menu
	// 6 = Empower enemies UI

	[SerializeField] private Transform[] DevUIComponents;
	// 0 = Melee Upgrades
	// 1 = Cast Upgrades

	[Serializable]
	private class abilityUI
	{
		public Transform abilityIcon;
		public Image cooldownClock;
		public bool onCooldown;
		public float cooldown;
	}

	[SerializeField] private abilityUI[] abilityUIElements;

	void Update()
	{
		SetHP(GameManager.Instance.PlayerHP.value, GameManager.Instance.PlayerHP.startValue);
		SetExp(GameManager.Instance.ExpManager.PlayerExp, GameManager.Instance.ExpManager.ExpToNextPoint);
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
		HandleCooldowns();
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

	public void SetHP(int hp, int maxHP)
	{
		hpBarSlider.value = hp;
		hpAmountText.text = hp.ToString() + "/" + maxHP;
	}

	public void SetExp(int exp, int maxExp)
	{
		expBarSlider.value = exp;
		expAmountText.text = exp.ToString() + "/" + maxExp;
	}

	public void AddDevText(int textComponent, string addText)
	{
		DevUIComponents[textComponent].GetComponent<TMP_Text>().text += (addText + ", ");
	}

	public void ResetCheatMenu()
	{
		uiStates[5].GetComponentInChildren<TMP_InputField>().text = string.Empty;
	}

	public void SetAbilityUIValues(int i, Sprite icon)
	{
		abilityUIElements[i].abilityIcon.GetComponent<Image>().sprite = icon;
		Color iconColor = abilityUIElements[i].abilityIcon.GetComponent<Image>().color;
		iconColor.a = 1f;
		abilityUIElements[i].abilityIcon.GetComponent<Image>().color = iconColor;
	}

	public void ResetAbilityUIValues()
	{
		foreach(abilityUI abilityUI in abilityUIElements)
		{
			abilityUI.abilityIcon.GetComponent<Image>().sprite = null;
			Color iconColor = abilityUI.abilityIcon.GetComponent<Image>().color;
			iconColor.a = 0f;
			abilityUI.abilityIcon.GetComponent<Image>().color = iconColor;
		}
	}

	public void CooldownCountDown(IAbility ability, int abilityInBar)
	{
		abilityUIElements[abilityInBar].cooldown = ability.CoolDown;

		if (!ability.CooledDown && !abilityUIElements[abilityInBar].onCooldown)
		{
			abilityUIElements[abilityInBar].cooldownClock.fillAmount = 1;
			abilityUIElements[abilityInBar].onCooldown = true;
		}
	}

	public void HandleCooldowns()
	{
		foreach (abilityUI abilityUI in abilityUIElements)
		{
			if (abilityUI.onCooldown && abilityUI.cooldownClock != null)
			{
				if (abilityUI.cooldownClock.fillAmount > 0)
				{
					abilityUI.cooldownClock.fillAmount -= 1 / (abilityUI.cooldown/1000) * Time.deltaTime;
				}
				else
				{
					abilityUI.cooldownClock.fillAmount = 0;
					abilityUI.onCooldown = false;
				}
			}

		}
	}
}
