using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlayerItem : MonoBehaviour, IListener
{
    public enum PlayerNext { None, Player1, Player2, Player3, Player4, Player5 }
    [Header("SWITCH PLAYER")]
    public PlayerNext switchPlayer;
    [Tooltip(" 0: unlimited time")]
    public float timeUseNewPlayer = 10;
    bool isWorked = false;
    public AudioClip sound;
    [Range(0, 1)]
    public float soundVolume = 0.5f;
    public GameObject Effect;

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            yield break;

        if (other.gameObject.GetComponent<Player>() == null)
            yield break;

        if (isWorked)
            yield break;

        //only allow player jump on to active it
        if (GameManager.Instance.Player.controller.boxcollider.bounds.min.y < transform.position.y)
            yield break;

        isWorked = true;

        SoundManager.PlaySfx(sound, soundVolume);
        
        if (Effect != null)
            Instantiate(Effect, transform.position, transform.rotation);

        int _player = 999;
        switch (switchPlayer)
        {
            case PlayerNext.None:
                break;
            case PlayerNext.Player1:
                _player = 0;
                break;
            case PlayerNext.Player2:
                _player = 1;
                break;
            case PlayerNext.Player3:
                _player = 2;
                break;
            case PlayerNext.Player4:
                _player = 3;
                break;
            case PlayerNext.Player5:
                _player = 4;
                break;
            default:
                break;
        }

        if (_player != 999 && (_player + 1 != GameManager.Instance.Player.ID))
        {
            GameManager.Instance.Player.transform.position = new Vector3(transform.position.x, GameManager.Instance.Player.transform.position.y, GameManager.Instance.Player.transform.position.z);
            //if (CharacterHolder.Instance.isThisCharIdUnlock(_player))
            //{
            CharacterHolder.Instance.WaitTimeBackToMainPlayer(timeUseNewPlayer==0? int.MaxValue: timeUseNewPlayer, GameManager.Instance.Player.ID - 1);
            CharacterHolder.Instance.CharacterPicked = CharacterHolder.Instance.Characters[_player];
            if (GameManager.Instance)
                GameManager.Instance.SwitchPlayerCharacter();

            //BlackScreenUI.instance.Show(0.3f, Color.white);
            //yield return new WaitForSeconds(0.1f);
            //BlackScreenUI.instance.Hide(0.3f);
            //}
        }

        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }

    public void IPlay()
    {
    }

    public void ISuccess()
    {
    }

    public void IPause()
    {
    }

    public void IUnPause()
    {
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
        if (!GameManager.Instance.Player.playerWithPartner && transform.position.x > (GameManager.Instance.currentCheckpoint? GameManager.Instance.currentCheckpoint.position.x: GameManager.Instance.currentPlayerPos.x))
        {
            gameObject.SetActive(true);
            isWorked = false;
        }
    }

    public void IOnStopMovingOn()
    {
    }

    public void IOnStopMovingOff()
    {
    }
}
