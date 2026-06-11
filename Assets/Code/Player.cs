using System;
using UnityEngine;

[Serializable]
public class Player
{
    public string player_name;
    public string uid;

    // last time player joined a clan (-1 means they never did)
    private float _last_clan_join_time = -1;
    private string _clan_uid;

    public string clan_uid
    {
        get
        {
            return _clan_uid;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                // setting a clan, set the timestamp of last clan join
                _last_clan_join_time = Time.realtimeSinceStartup;
            }

            _clan_uid = value;
        }
    }

    public int power;

    public static readonly int max_power = 1000;
    public static readonly float time_between_allowed_clan_join = 10;

    public bool IsInClan()
    {
        return !String.IsNullOrEmpty(clan_uid);
    }

    public bool IsAllowedToJoinClan()
    {
        if (_last_clan_join_time == -1)
        {
            return true;
        }

        if (Time.realtimeSinceStartup >= _last_clan_join_time + time_between_allowed_clan_join)
        {
            return true;
        }

        return false;
    }

    public float TimeRemainingToJoinClan()
    {
        if (_last_clan_join_time == -1)
        {
            return 0;
        }

        var time_since_join = Time.realtimeSinceStartup - _last_clan_join_time;
        return time_between_allowed_clan_join - time_since_join;
    }

    public void LeaveClan()
    {
        if (IsInClan())
        {
            var clan = ClanManager.Instance.GetClan(clan_uid);

            if (clan == null)
            {
                Debug.LogError($"Trying to get nonexistent clan. uid: {clan_uid}");
                return;
            }

            clan.RemovePlayer(uid);
        }
    }

    public Clan GetClan()
    {
        return ClanManager.Instance.GetClan(clan_uid);
    }
}
