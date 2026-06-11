using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Player MainPlayer;
    public List<Player> players = new List<Player>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Player GetPlayer(string uid)
    {
        foreach (var player in players)
        {
            if (player.uid == uid)
            {
                return player;
            }
        }

        return null;
    }

    public List<Player> GetListOfRandomPlayersNotInAnyClan(int count)
    {
        var players_to_return = new List<Player>();

        var player_buffer = GetPlayersNotInAnyClan();
        player_buffer = Utilities.ShufflePlayerList(player_buffer);

        for (int i = 0; i < count; i++)
        {
            if (i < player_buffer.Count)
            {
                players_to_return.Add(player_buffer[i]);
            }
        }

        return players_to_return;
    }

    public List<Player> GetPlayersNotInAnyClan()
    {
        var players_to_return = new List<Player>();

        foreach (var player in players)
        {
            if (String.IsNullOrEmpty(player.clan_uid) && player != MainPlayer)
            {
                players_to_return.Add(player);
            }
        }

        return players_to_return;
    }
}
