/// <summary>
/// The UI Level, check the current level
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public int group = 1;
    public int levelNumber = 1;
    //string levelSceneName;
    public bool isUnlock = false;
    public Text numberTxt;
    public GameObject imageLocked;

    public bool disableStarGroup = false;
    public GameObject starGroup;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public GameObject shiningFX;
    public GameObject Openning;
    // Use this for initialization

    void Start()
    {
        if (isUnlock || (GlobalValue.LevelPass + 1 >= int.Parse(gameObject.name)))
        {
            numberTxt.text = gameObject.name;

            shiningFX.SetActive(GlobalValue.LevelPass + 1 == int.Parse(gameObject.name));
            Openning.SetActive(GlobalValue.LevelPass + 1 == int.Parse(gameObject.name));

            if (GlobalValue.LevelPass + 1 == int.Parse(gameObject.name))
                World_1.Instance.AvailableLevelGroup(this);

            imageLocked.SetActive(false);
            GetComponent<Button>().interactable = true;
        }
        else
        {
            Openning.SetActive(false);
            shiningFX.SetActive(false);
            numberTxt.gameObject.SetActive(false);
            imageLocked.SetActive(true);
            GetComponent<Button>().interactable = false;
            starGroup.SetActive(false);
        }

        CheckStars(GlobalValue.worldPlaying, int.Parse(gameObject.name));
    }

    private void CheckStars(int worldNumber, int levelNumber)
    {
        star1.SetActive(GlobalValue.IsScrollLevelAte(levelNumber, 1));
        star2.SetActive(GlobalValue.IsScrollLevelAte(levelNumber, 2));
        star3.SetActive(GlobalValue.IsScrollLevelAte(levelNumber, 3));

        if (!disableStarGroup)
            starGroup.SetActive(star1.activeInHierarchy || star2.activeInHierarchy || star3.activeInHierarchy);
    }

    public void Play()
    {
        GlobalValue.levelPlaying = levelNumber;
        if (AdsManager.Instance)
            AdsManager.Instance.ShowAdmobBanner(false);

        MainMenuHomeScene.Instance.LoadScene("Playing");
    }

    public void Play(string _levelSceneName = null)
    {
        SoundManager.Click();
        GlobalValue.levelPlaying = int.Parse(gameObject.name);
        MainMenuHomeScene.Instance.LoadScene("Level " + GlobalValue.levelPlaying);
    }
}
