using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LungePointCollision : MonoBehaviour
{
    [SerializeField] private PlayerBrain ownerPlayer;
    [SerializeField] private LayerMask knightsMask;

    // ---------- Unity messages

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, knightsMask);

        foreach (Collider2D collider in colliders)
        {
            //check if the hit object is a Player and exclude owner Player
            PlayerBrain player = collider.gameObject.GetComponentInParent<PlayerBrain>();
            if (player != null && player != ownerPlayer)
            {
                //give damage to hit player (if player can take damage)
                player.ReceiveHit(ownerPlayer);
            }
        }
    }
}
