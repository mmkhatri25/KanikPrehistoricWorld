/// <summary>
/// Main menu game success.
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_GameSuccess : MonoBehaviour
{
	public GameObject Star1;        //the star object image
	public GameObject Star2;
	public GameObject Star3;
        public Text coinText, finishText;

	public GameObject NextBut;

	public AudioClip soundStar1;
	public AudioClip soundStar2;
	public AudioClip soundStar3;

	IEnumerator Start()
	{
        coinText.text = GameManager.Instance.Coin.ToString ("00");
        print("GlobalValue.LevelPass-"+GlobalValue.LevelPass);
        if(GlobalValue.LevelPass == 1030)
        {
            finishText.text = "congrats you have clear all levels.";
        }

        Star1.SetActive(false);
		Star2.SetActive(false);
		Star3.SetActive(false);
		NextBut.SetActive(GlobalValue.levelPlaying != -1);

		yield return new WaitForSeconds(0.5f);

		if (GlobalValue.bigStar1)
		{
			Star1.SetActive(true);
			SoundManager.PlaySfx(soundStar1);
			yield return new WaitForSeconds(1);
		}

		if (GlobalValue.bigStar2)
		{
			Star2.SetActive(true);
			SoundManager.PlaySfx(soundStar2);
			yield return new WaitForSeconds(1);
		}

		if (GlobalValue.bigStar3)
		{
			Star3.SetActive(true);
			SoundManager.PlaySfx(soundStar3);
			yield return new WaitForSeconds(1f);
		}
	}
}
