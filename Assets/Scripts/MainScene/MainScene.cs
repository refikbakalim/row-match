using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainScene : MonoBehaviour
{
	public GameObject LevelsPopup;
	public GameObject LevelsButton;
	public GameObject HighscorePopup;

	[SerializeField] private TMPro.TextMeshProUGUI highScoreText;

	private const float TweenDuration = 0.1f;

	public GameObject CelebrationParticle;

	public async void openLevelsPopup()
	{
		if (!LevelsPopup) return;

		// Make levels popup appear
		LevelsPopup.SetActive(true);

		// Then animate the popup
		var sequence = DOTween.Sequence();
		sequence.Append(LevelsPopup.transform.DOScale(0.98f, TweenDuration));
		sequence.Append(LevelsPopup.transform.DOScale(1f, TweenDuration));
		await sequence.Play().AsyncWaitForCompletion();
	}

	public void closeLevelsPopup()
	{
		if (!LevelsPopup) return;

		LevelsPopup.SetActive(false);
	}

	public async void onLevelsButtonClicked()
	{
		// Animate levels button, then open levels popup
		var Sequence = DOTween.Sequence();
		Sequence.Append(LevelsButton.transform.DOScale(new Vector3(0.95f, 1.98f, 0.95f), TweenDuration));
		Sequence.Append(LevelsButton.transform.DOScale(new Vector3(1f, 2f, 1f), TweenDuration));
		await Sequence.Play().AsyncWaitForCompletion();

		openLevelsPopup();
	}


	void Start()
	{
		if (StaticData.returningFromGame) // If we are returning back from a level
		{
			if (StaticData.hasNewHighscore) openHighscorePopup(); // If we have a new highscore, open highscore popup
			else openLevelsPopup();
		}
	}

	private async void openHighscorePopup()
	{
		if (!HighscorePopup) return;

		// Set highscore text then, Make highscore popup appear with scale 0
		HighscorePopup.transform.GetChild(0).GetChild(1).transform.localScale = new Vector3(0f, 0f, 0f);
		highScoreText.SetText(StaticData.highscore.ToString());
		HighscorePopup.SetActive(true);

		CelebrationParticle.SetActive(true); // Play the celebration particle

		// scale up the popup, wait 3.5 seconds, then scale down the popup
		var popupSequence = DOTween.Sequence();
		popupSequence.Append(HighscorePopup.transform.GetChild(0).GetChild(1).transform.DOScale(1f, 0.6f));
		popupSequence.AppendInterval(3.5f);
		popupSequence.Append(HighscorePopup.transform.GetChild(0).GetChild(1).transform.DOScale(0f, 0.6f));
		popupSequence.OnComplete(openLevelsPopup);
		await popupSequence.Play().AsyncWaitForCompletion();

		// disable the celebration particle and highscore popup
		CelebrationParticle.SetActive(false);
		HighscorePopup.SetActive(false);

	}
}
