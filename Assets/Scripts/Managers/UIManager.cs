using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField, BoxGroup("EXP Bar")] private Slider expBarSlider;
	[SerializeField, BoxGroup("EXP Bar")] private Slider expBarSliderDelayed;
	[SerializeField, BoxGroup("EXP Bar")] private float expBarSliderSmoothing = 0.5f;
	[SerializeField, BoxGroup("EXP Bar")] private TMP_Text expAmountText;
	[SerializeField, BoxGroup("EXP Bar")] private TMP_Text pointText;
	[SerializeField, BoxGroup("EXP Bar")] private GameObject getPointVFX;

	[SerializeField, BoxGroup("HP Bar")] private Slider hpBarSlider;
	[SerializeField, BoxGroup("HP Bar")] private Slider hpBarSliderDelayed;
	[SerializeField, BoxGroup("HP Bar")] private float hpBarSliderSmoothing = 0.5f;
	[SerializeField, BoxGroup("HP Bar")] private TMP_Text hpAmountText;

	[SerializeField, BoxGroup("Player On Hit")] private Image playerHitEffect;
	[SerializeField, BoxGroup("Player On Hit")] private float playerHitEffectDuration;

	[SerializeField, BoxGroup("Map")] private Canvas mainCanvas;
	[SerializeField, BoxGroup("Map")] private Canvas shaderCanvas;
	[SerializeField, BoxGroup("Map")] private GameObject map;

	[SerializeField, BoxGroup("Upgrades")] private IconTray meleeUpgradeIcons;
	[SerializeField, BoxGroup("Upgrades")] private IconTray rangedUpgradeIcons;
	[SerializeField, BoxGroup("Upgrades")] private Transform[] DevUIComponents;
	// 0 = Melee Upgrades
	// 1 = Cast Upgrades

	//TODO: Create custom struct that holds UI elements so we don't have to use indeces for enabling/disabling UI, but instead call them by name/type etc.
	[SerializeField, BoxGroup("UI States")] private GameObject[] uiStates;
	// 0 = Hub UI
	// 1 = Dungeon UI
	// 2 = Generation Loading Screen
	// 3 = Pause Screen
	// 4 = Death Screen
	// 5 = Cheat Menu
	// 6 = Empower enemies UI

	private const string playerHitEffectPropertyName = "_Amount";
	private Material playerHitEffectMaterial;

	public GameObject[] UiStates { get => uiStates; private set => uiStates = value; }
	public IconTray MeleeUpgradeIcons { get => meleeUpgradeIcons; set => meleeUpgradeIcons = value; }
	public IconTray RangedUpgradeIcons { get => rangedUpgradeIcons; set => rangedUpgradeIcons = value; }

	[Serializable]
	private class abilityUI
	{
		public Transform abilityIcon;
		public Image cooldownClock;
		public bool onCooldown;
		public float cooldown;
	}

	[SerializeField] private abilityUI[] abilityUIElements;

	private void Start()
	{
		playerHitEffectMaterial = playerHitEffect.material;
		playerHitEffectMaterial.SetFloat(playerHitEffectPropertyName, 0.5f);
		playerHitEffect.gameObject.SetActive(false);
	}

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
		hpBarSliderDelayed.maxValue = maxHP;
		hpBarSliderDelayed.value = maxHP;
	}

	public void SetExpBar(int maxExp)
	{
		expBarSlider.maxValue = maxExp;
		expBarSlider.value = maxExp;
		expBarSliderDelayed.maxValue = maxExp;
		expBarSliderDelayed.value = maxExp;
	}

	public void SetHP(int hp, int maxHP)
	{
		if (hp != hpBarSlider.value)
		{
			hpBarSlider.value = hp;
			hpAmountText.text = hp.ToString() + "/" + maxHP;

			StartCoroutine(SetDelayedHP());
		}
	}

	private IEnumerator SetDelayedHP()
	{
		yield return new WaitForSeconds(1f);

		float startValue = hpBarSliderDelayed.value;
		float endValue = hpBarSlider.value;
		float timeElapsed = 0;
		while (timeElapsed < hpBarSliderSmoothing)
		{
			float smoothedValue = Mathf.Lerp(startValue, endValue, timeElapsed / hpBarSliderSmoothing);
			timeElapsed += Time.deltaTime;

			hpBarSliderDelayed.value = smoothedValue;
			yield return null;
		}

		hpBarSliderDelayed.value = endValue;
	}

	public void SetExp(int exp, int maxExp)
	{
		if (exp != expBarSliderDelayed.value)
		{
			expBarSliderDelayed.value = exp;
			expAmountText.text = exp.ToString() + "/" + maxExp;

			StartCoroutine(SetDelayedExp());
		}
	}

	private IEnumerator SetDelayedExp()
	{
		yield return new WaitForSeconds(1f);

		float startValue = expBarSlider.value;
		float endValue = expBarSliderDelayed.value;
		float timeElapsed = 0;
		while (timeElapsed < expBarSliderSmoothing)
		{
			float smoothedValue = Mathf.Lerp(startValue, endValue, timeElapsed / expBarSliderSmoothing);
			timeElapsed += Time.deltaTime;

			expBarSlider.value = smoothedValue;
			yield return null;
		}

		expBarSlider.value = endValue;
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
		foreach (abilityUI abilityUI in abilityUIElements)
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
					abilityUI.cooldownClock.fillAmount -= 1 / (abilityUI.cooldown / 1000) * Time.deltaTime;
				}
				else
				{
					abilityUI.cooldownClock.fillAmount = 0;
					abilityUI.onCooldown = false;
				}
			}

		}
	}

	public void AssignPlayerCameraToShaderCanvas(Camera camera)
	{
		if (shaderCanvas.worldCamera == null)
		{
			shaderCanvas.worldCamera = camera;
		}
	}	
	
	public void AssignPlayerCameraToMainCanvas(Camera camera)
	{
		if (mainCanvas.worldCamera == null)
		{
			mainCanvas.worldCamera = camera;
		}
	}

	public void ToggleMapOverlay(bool isEnabled)
	{
		if (map.activeSelf != isEnabled)
		{
			map.SetActive(isEnabled);
		}
	}

	public void PlayerHitScreenEffect()
	{
		StopCoroutine(ScreenVFXOnPlayerHit());
		StartCoroutine(ScreenVFXOnPlayerHit());
	}

	public void GetPointParticleEffect()
	{
		getPointVFX.GetComponent<ParticleSystem>().Play();
	}

	IEnumerator ScreenVFXOnPlayerHit()
	{
		playerHitEffect.gameObject.SetActive(true);
		ScriptableInt playerHP = GameManager.Instance.PlayerHP;
		float amount = ((float)playerHP.value / (float)playerHP.startValue);
		playerHitEffectMaterial.SetFloat(playerHitEffectPropertyName, Mathf.Lerp(0.5f, 0.38f, 1 - amount));
		float currentTime = 0;
		while (currentTime < playerHitEffectDuration)
		{
			currentTime += Time.deltaTime;
			Color effectColor = playerHitEffect.color;
			effectColor.a = 1 - Mathf.Clamp01(currentTime / playerHitEffectDuration);
			playerHitEffect.color = effectColor;
			yield return new WaitForEndOfFrame();
		}
		playerHitEffect.gameObject.SetActive(false);
	}
}
