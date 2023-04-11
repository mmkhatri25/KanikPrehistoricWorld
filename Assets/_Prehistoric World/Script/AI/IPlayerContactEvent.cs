using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CONTACT_POS { Left, Right, Above, Below}
public interface IPlayerContactEvent
{
    void OnPlayerContact(CONTACT_POS contactPos, Vector2 hitPoint);
}
