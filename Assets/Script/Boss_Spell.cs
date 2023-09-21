using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Spell : MonoBehaviour
{
    public GameObject bossSpellPrefab;
    public GameObject player;

    [SerializeField] float yOffset = 1.0f;

    public void SpellCast()
    {
        Vector3 spawnPosition = new Vector3(player.transform.position.x, player.transform.position.y + yOffset, player.transform.position.z);
        GameObject spell = Instantiate(bossSpellPrefab, spawnPosition, Quaternion.identity);
    }
}
