using UnityEngine;
using System.Collections.Generic;
using System;

public class Clan
{
    public string clan_name;
    public string uid;
    public bool private_clan = false;
    public List<string> members_uids = new List<string>();
    public string owner_player_uid;

    public static readonly int max_players = 10;

    public bool AddPlayer(string player_uid)
    {
        var player = PlayerManager.Instance.GetPlayer(player_uid);

        if (members_uids.Count >= max_players)
        {
            DebugInfoUI.Instance.Message($"{clan_name} - is full ! can't add player: {player.player_name}");
            return false;
        }

        members_uids.Add(player_uid);
        player.clan_uid = uid;
        return true;
    }

    public void AddOwner(string player_uid)
    {
        if (AddPlayer(player_uid))
        {
            owner_player_uid = player_uid;
        }
    }

    public void RemovePlayer(string uid)
    {
        if (!members_uids.Contains(uid))
        {
            Debug.LogError($"Trying to remove player from clan, that is not in the clan. uid: {uid}");
            return;
        }

        var player = PlayerManager.Instance.GetPlayer(uid);
        player.clan_uid = "";
        members_uids.Remove(uid);

        // Removed player is the owner, find a replacement
        if (owner_player_uid == uid)
        {
            if (members_uids.Count > 0)
            {
                // Just get the first remaining player
                owner_player_uid = members_uids[0];
            }
            else
            {
                // There are no more players, delete the clan
                DebugInfoUI.Instance.Message($"Clan: {clan_name} has 0 player left. Removing the clan !");
                ClanManager.Instance.RemoveClan(this);
            }
        }
    }


    public int GetClanPower()
    {
        int totalPower = 0;

        foreach (var member in members_uids)
        {
            var player_member = PlayerManager.Instance.GetPlayer(member);

            if (player_member == null)
            {
                Debug.LogError($"Trying to find nonexistent player with guid: {player_member.uid}");
                return -1;
            }

            totalPower += player_member.power;
        }

        return totalPower;
    }

    public int GetClanPowerWeighted()
    {
        /*
        Weighted Power Distribution (The "Elite Bonus")
        Instead of a linear sum, apply a weight to each player based on their individual power level. 
        This creates a "premium" on high-ranking members.
        */

        float totalPower = 0;

        foreach (var member in members_uids)
        {
            var player_member = PlayerManager.Instance.GetPlayer(member);

            if (player_member == null)
            {
                Debug.LogError($"Trying to find nonexistent player with guid: {player_member.uid}");
                return -1;
            }

            // 1. Calculate the base power (clamped at 1000)
            float basePower = Mathf.Clamp(player_member.power, 0, Player.max_power);

            // 2. Determine Weight: 
            // Players under 500 power = 1.0x weight
            // Players 500-800 power = 1.25x weight
            // Players 800-1000 power = 1.5x weight
            float weight = 1.0f;

            if (basePower >= 800)
                weight = 1.5f;
            else if (basePower >= 500)
                weight = 1.25f;

            totalPower += (basePower * weight);
        }

        return Mathf.RoundToInt(totalPower);
    }

    public bool IsPlayerMember(string uid)
    {
        foreach (var member_uid in members_uids)
        {
            if (member_uid == uid)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPlayerOwner(string uid)
    {
        return owner_player_uid == uid;
    }

    public bool IsFull()
    {
        return members_uids.Count >= max_players;
    }
}
