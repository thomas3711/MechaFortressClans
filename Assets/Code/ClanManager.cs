using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ClanManager : MonoBehaviour
{
    public static ClanManager Instance { get; private set; }

    [SerializeField] ClanManagerUI ui_manager;

    public ClanManagerUI GetUIManager() => ui_manager;

    public List<Clan> clans = new List<Clan>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        /*clans.Clear();
        clans = Database.Instance.GetAllClans();

        Debug.Log("ClanManager Initialize. Loaded clans: " + clans.Count);

        foreach (var clan in clans)
        {
            ui_manager.AddClanToList(clan);
        }*/
    }

    public Clan GetClan(string uid)
    {
        foreach (var clan in clans)
        {
            if (clan.uid == uid)
            {
                return clan;
            }
        }

        return null;
    }

    public void AddClan(Clan clan_to_add)
    {
        clans.Add(clan_to_add);
        ui_manager.AddClanToList(clan_to_add);

        // TODO: Save to database
    }

    public void RemoveClan(Clan clan_to_remove)
    {
        if (!clans.Contains(clan_to_remove))
        {
            Debug.LogError($"Trying to remove non-existent clan. uid: {clan_to_remove.uid}");
            return;
        }

        clans.Remove(clan_to_remove);
        ui_manager.RemoveClanFromList(clan_to_remove);

        // TODO: Remove from database
    }

    public void Save()
    {

    }
}
