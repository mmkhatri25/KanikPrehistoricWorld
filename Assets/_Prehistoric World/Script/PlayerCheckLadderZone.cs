using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckLadderZone : MonoBehaviour
{
    public LayerMask layerAsLadder;
    public float climbSpeed = 3;        //how fast the player climb the ladder?
    [ReadOnly] public bool isInLadderZone = false;
    [ReadOnly]
    public bool isClimbing = false;

    [ReadOnly] public bool isClimbingLadder8Dir = false;
    public GameObject currentLadder { get; set; }
    [ReadOnly] public bool fallingOffFromLadder = false;

    Player player;
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        //RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.1f, Vector2.zero, 0, layerAsLadder);

        RaycastHit2D hit = Physics2D.BoxCast(player.controller.boxcollider.bounds.center, player.controller.boxcollider.bounds.size, 0, Vector2.zero, 0, layerAsLadder);

        if (hit && !fallingOffFromLadder)
        {
            isInLadderZone = true;
            currentLadder = hit.collider.gameObject;
        }
        else
        {
            //isInLadderZone = false;
            isInLadderZone = false;
            currentLadder = null;
            isClimbing = false;
        }
    }

    public bool checkLadderWithPoint(Vector2 point)
    {
        if (Physics2D.CircleCast(point, 0.01f, Vector2.zero, 0, layerAsLadder))
            return true;
        else
            return false;
    }

    public bool isLadderAbove
    {
        get
        {
            //if (Physics2D.CircleCast(transform.position + Vector3.up * 0.5f, 0.1f, Vector2.zero, 0, layerAsLadder))
            //    return true;
            //else return false;

            if (Physics2D.Raycast(new Vector2(player.transform.position.x, player.controller.boxcollider.bounds.max.y), Vector2.up, 0.1f, layerAsLadder))
                return true;
            else
                return false;
        }
    }

    public bool isLadderBelow
    {
        get
        {
            //if (Physics2D.CircleCast(transform.position + Vector3.down * 0.5f, 0.1f, Vector2.zero, 0, layerAsLadder))
            //    return true;
            //else return false;
            if (Physics2D.Raycast(new Vector2(player.transform.position.x, player.controller.boxcollider.bounds.min.y - 0.5f), Vector2.down, 0.1f, layerAsLadder))
                return true;
            else
                return false;
        }
    }
}