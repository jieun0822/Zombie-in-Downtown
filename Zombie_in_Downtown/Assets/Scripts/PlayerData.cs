using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public string name;
    public int score;
    public int stage;

    public int ammo;
    public int coin;
    public int health;
    public int hasGranades;

    public bool[] hasWeapons;

    public PlayerData(GameManager _gameManager)
    {
        name = _gameManager.player.name;
        score = _gameManager.player.score;
        stage = _gameManager.stage;

        ammo = _gameManager.player.ammo;
        coin = _gameManager.player.coin;
        health = _gameManager.player.health;
        hasGranades = _gameManager.player.hasGranades;

        hasWeapons = new bool[3];
        hasWeapons[0] = _gameManager.player.hasWeapons[0];
        hasWeapons[1] = _gameManager.player.hasWeapons[1];
        hasWeapons[2] = _gameManager.player.hasWeapons[2];
    }
}
