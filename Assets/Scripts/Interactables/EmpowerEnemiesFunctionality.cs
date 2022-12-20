using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmpowerEnemiesFunctionality : MonoBehaviour
{
	[SerializeField] private ExpManager expMan;
	[SerializeField] private LevelManager levelMan;

	//[SerializeField] private int currentPointCost; //Increase cost, keep updated

	[SerializeField] private int healOnPointSpend;
	[SerializeField] private TMP_Text currentLevelText;
	[SerializeField] private TMP_Text newLevelText;
	[SerializeField] private List<TMP_Text> currentStatTexts = new List<TMP_Text>();
	[SerializeField] private List<TMP_Text> newStatTexts = new List<TMP_Text>();
	private float currentStat;
	private float increasedStat;

	private void Start()
	{
		expMan = GameManager.Instance.ExpManager;
		levelMan = LevelManager.LevelManagerInstance;
	}

	private void OnEnable()
	{
		if (expMan == null)
		{
			expMan = GameManager.Instance.ExpManager;
		}
	}

	void Update()
	{
		if(expMan != null)
		{
			UpdateEmpowerUI();
		}
	}

	void UpdateEmpowerUI()
	{
		GameManager.Instance.SetPauseState(true);
		currentLevelText.text = levelMan.CurrentLevel.ToString();
		int newLevelNumber = levelMan.CurrentLevel + 1;
		newLevelText.text = newLevelNumber.ToString();
		currentStat = ((levelMan.DificultyModifier - 1f) * 100f) * levelMan.CurrentLevel;
		increasedStat = ((levelMan.DificultyModifier - 1f) * 100f) * newLevelNumber;
		foreach (TMP_Text statText in currentStatTexts)
		{
			statText.text = currentStat.ToString() + "%";
		}
		foreach (TMP_Text statText in newStatTexts)
		{
			statText.text = increasedStat.ToString() + "%";
		}
	}

	public void EmpowerEnemies()
	{
		if (expMan.PlayerPoints >= levelMan.PointToLevel)
		{
			expMan.PlayerPoints -= levelMan.PointToLevel;
			GameManager.Instance.PlayerHP.value += healOnPointSpend;
			Debug.Log("level up, current points: " + expMan.PlayerPoints + " Points to level was: " + levelMan.PointToLevel);
			levelMan.IncreaseLevel();
		}
	}

	public void CloseEmpowerWindow()
	{
		GameManager.Instance.UiManager.SetUIActive(6, false);
	}
}
