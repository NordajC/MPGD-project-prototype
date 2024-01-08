using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombat
{
    void onHitReaction(bool heavyAttack, Vector3 hitLocation, HitDirection hitDirection, HitHeight hitHeight, GameObject receivedFrom, float damage);
}