using UnityEngine;

public class ClanManagerUI : MonoBehaviour
{
    [SerializeField] Transform clan_entry_parent;
    [SerializeField] ClanDetailsUI clan_details;
    [SerializeField] ClanEntryUI clan_entry_prefab;
    [SerializeField] MainPlayerInfoUI main_player_info;
    [SerializeField] GameObject leave_clan_button;

    public MainPlayerInfoUI GetMainPlayerInfoUI() => main_player_info;

    void Start()
    {
        UpdateLeaveClanButtonState();
        clan_details.gameObject.SetActive(false);
    }

    public void AddClanToList(Clan clan_to_add)
    {
        ClanEntryUI ui_entry = Instantiate(clan_entry_prefab);
        ui_entry.Initialize(clan_to_add);
        ui_entry.transform.SetParent(clan_entry_parent);

        ui_entry.GetButton().onClick.AddListener(() =>
        {
            ShowClanDetails(clan_to_add);
        });
    }

    public void RemoveClanFromList(Clan clan_to_remove)
    {
        foreach (Transform child in clan_entry_parent)
        {
            var clan_entry_ui = child.GetComponent<ClanEntryUI>();

            if (clan_entry_ui && clan_entry_ui.clan.uid == clan_to_remove.uid)
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }

    public void ShowClanDetails(Clan clan_to_show)
    {
        // Don't allow to view private clans for nonmembers
        if (clan_to_show.private_clan && !clan_to_show.IsPlayerMember(PlayerManager.Instance.MainPlayer.uid))
        {
            DebugInfoUI.Instance.Message($"Clan is private and you are not a member !");
            return;
        }

        clan_details.gameObject.SetActive(true);
        clan_details.Initialiaze(clan_to_show);
    }

    public void OnLeaveClanPressed()
    {
        var players_clan = PlayerManager.Instance.MainPlayer.GetClan();
        bool clan_will_be_removed = players_clan.members_uids.Count == 1;

        PlayerManager.Instance.MainPlayer.LeaveClan();

        UpdateLeaveClanButtonState();

        if (clan_details.gameObject.activeSelf)
        {
            if (clan_will_be_removed) // Don't show details for non-existant clan
            {
                clan_details.gameObject.SetActive(false);
            }
            else if (players_clan.private_clan) // Hide private clan that you are not part of any longer
            {
                clan_details.gameObject.SetActive(false);
            }
            else
            {
                clan_details.UpdateUIElements();
            }
        }
    }

    public void UpdateLeaveClanButtonState()
    {
        var main_player = PlayerManager.Instance.MainPlayer;
        leave_clan_button.gameObject.SetActive(main_player.IsInClan());
        main_player_info.UpdateUI();
    }
}
