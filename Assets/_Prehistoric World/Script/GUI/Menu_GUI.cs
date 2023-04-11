using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GUI : MonoBehaviour {
    public static Menu_GUI Instance;
	public Text coinText;
	public Text liveText;

	public Transform healthBar;
	public GameObject Key;

	CanvasGroup canvasGroup;

    public GameObject starGroup;
    public Image star1, star2, star3;
    bool isStar1Collected, isStar2Colltected, isStar3Collected;
    bool firstPlay = true;

    private void Awake()
    {
        Instance = this;

    
    }

    private void OnEnable()
    {
        if (firstPlay)
            return;

        if (isStar1Collected)
        {
            star1.color = Color.white;
        }
        if (isStar2Colltected)
        {
            star2.color = Color.white;
        }
        if (isStar3Collected)
        {
            star3.color = Color.white;
        }
    }

    private void Start()
    {
		canvasGroup = GetComponent<CanvasGroup>();

        star1.color = Color.black;
        star2.color = Color.black;
        star3.color = Color.black;

        firstPlay = false;

        if (LevelMapType.Instance.levelType == LEVELTYPE.BossFight)
            starGroup.SetActive(false);
    }

    public Vector2 maskUIPosition(int number)
    {
        switch (number)
        {
            case 1:
                return Camera.main.ScreenToWorldPoint(new Vector3(star1.transform.position.x, star1.transform.position.y, -Camera.main.transform.position.z));

            case 2:
                return Camera.main.ScreenToWorldPoint(new Vector3(star2.transform.position.x, star2.transform.position.y, -Camera.main.transform.position.z));

            case 3:
                return Camera.main.ScreenToWorldPoint(new Vector3(star3.transform.position.x, star3.transform.position.y, -Camera.main.transform.position.z));

            default:
                return Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update () {
		coinText.text = GameManager.Instance.Coin.ToString ("00");
        if (GlobalValue.levelPlaying == -1)
            liveText.text = "MAX";
        else
            liveText.text = "x" + GlobalValue.SavedLives;

		healthBar.localScale = new Vector3 ((float)GameManager.Instance.Player.Health / (float)GameManager.Instance.Player.maxHealth, 1, 1);

		Key.SetActive (GameManager.Instance.isHasKey);

        canvasGroup.alpha = GameManager.Instance.isInDialogue ? 0 : 1;
    }

    public void ScrollCollectAnim(int ID)
    {
        switch (ID)
        {
            case 1:
                star1.color = Color.white;
                isStar1Collected = true;
                break;
            case 2:
                star2.color = Color.white;
                isStar2Colltected = true;
                break;
            case 3:
                star3.color = Color.white;
                isStar3Collected = true;
                break;
            default:
                break;
        }
    }
}
