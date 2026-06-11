using TMPro;
using UnityEngine;

public class ClanDetailsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clan_name;
    [SerializeField] TextMeshProUGUI clan_power;
    [SerializeField] TextMeshProUGUI clan_owner;
    [SerializeField] TextMeshProUGUI clan_member_count;
    [SerializeField] TextMeshProUGUI clan_private_state;

    [SerializeField] Transform player_entry_parent;
    [SerializeField] PlayerEntryUI player_entry_prefab;
    [SerializeField] Transform invite_player_entry_parent;
    [SerializeField] GameObject invite_player_window;
    [SerializeField] GameObject invite_player_button;
    [SerializeField] GameObject join_clan_button;
    [SerializeField] GameObject kick_player_button;

    private Clan clan;

    public void Initialiaze(Clan clan_p)
    {
        clan = clan_p;
        clan_name.text = clan.clan_name;

        UpdateUIElements();
    }

    public void UpdateUIElements()
    {
        UpdateClanPower();
        UpdateClanOwner();
        UpdateInviteButtonState();
        UpdateJoinClanButtonState();
        UpdateMemberList();
        UpdateMemberCount();
        UpdateKickPlayerButtonState();
        UpdatePrivateState();
    }

    public void UpdatePrivateState()
    {
        if (clan.private_clan)
        {
            clan_private_state.text = "Private";
        }
        else
        {
            clan_private_state.text = "Public";
        }
    }

    public void UpdateClanOwner()
    {
        var player = PlayerManager.Instance.GetPlayer(clan.owner_player_uid);

        if (player == null)
        {
            Debug.LogError($"Trying to find nonexistent player with guid: {clan.owner_player_uid}");
            return;
        }

        clan_owner.text = player.player_name;
    }

    public void UpdateInviteButtonState()
    {
        if (clan.private_clan)
        {
            if (clan.owner_player_uid == PlayerManager.Instance.MainPlayer.uid)
            {
                invite_player_button.SetActive(true);
            }
            else
            {
                invite_player_button.SetActive(false);
            }
        }
        else // For non private clan, show invite only if you are member
        {
            if (clan.IsPlayerMember(PlayerManager.Instance.MainPlayer.uid))
            {
                invite_player_button.SetActive(true);
            }
            else
            {
                invite_player_button.SetActive(false);
            }
        }
    }

    public void UpdateJoinClanButtonState()
    {
        // Show join button, if player is not in clan
        if (clan.IsPlayerMember(PlayerManager.Instance.MainPlayer.uid))
        {
            join_clan_button.SetActive(false);
        }
        else
        {
            join_clan_button.SetActive(true);
        }
    }

    public void UpdateKickPlayerButtonState()
    {
        // Show kick button only for owner
        if (clan.owner_player_uid == PlayerManager.Instance.MainPlayer.uid)
        {
            kick_player_button.SetActive(true);
        }
        else
        {
            kick_player_button.SetActive(false);
        }
    }

    public void JoinClan()
    {
        if (clan.IsFull())
        {
            DebugInfoUI.Instance.Message("You can't join ! Clan is full !");
            return;
        }

        var main_player = PlayerManager.Instance.MainPlayer;

        if (!main_player.IsAllowedToJoinClan())
        {
            DebugInfoUI.Instance.Message($"You can't join clan right now ! Wait {main_player.TimeRemainingToJoinClan()} seconds !");
            return;
        }

        // If player is already in a clan, remove them from it
        if (main_player.IsInClan())
        {
            var players_clan = ClanManager.Instance.GetClan(main_player.clan_uid);
            players_clan.RemovePlayer(main_player.uid);
        }

        clan.AddPlayer(main_player.uid);

        UpdateClanPower();
        UpdateJoinClanButtonState();
        UpdateInviteButtonState();
        UpdateMemberList();
        UpdateMemberCount();
        ClanManager.Instance.GetUIManager().UpdateLeaveClanButtonState();
        ClanManager.Instance.GetUIManager().GetMainPlayerInfoUI().UpdateUI();
    }

    public void UpdateClanPower()
    {
        clan_power.text = clan.GetClanPower().ToString();
    }

    public void UpdateMemberCount()
    {
        clan_member_count.text = $"{clan.members_uids.Count}/{Clan.max_players}";
    }

    public void UpdateMemberList()
    {
        foreach (Transform child in player_entry_parent)
        {
            Destroy(child.gameObject);
        }

        foreach (var member_uid in clan.members_uids)
        {
            var player_member = PlayerManager.Instance.GetPlayer(member_uid);

            if (player_member == null)
            {
                Debug.LogError($"Trying to find nonexistent player with guid: {player_member.uid}");
                return;
            }

            PlayerEntryUI ui_entry = Instantiate(player_entry_prefab);
            ui_entry.Initialize(player_member);
            ui_entry.transform.SetParent(player_entry_parent);
        }
    }

    public void OpenInvitePlayerWindow()
    {
        foreach (Transform child in invite_player_entry_parent)
        {
            Destroy(child.gameObject);
        }

        invite_player_window.SetActive(true);

        // TODO: iterating over all players is inefficient - add hot list of Players to each clan
        foreach (var player in PlayerManager.Instance.players)
        {
            if (!clan.IsPlayerMember(player.uid))
            {
                PlayerEntryUI ui_entry = Instantiate(player_entry_prefab);
                ui_entry.Initialize(player);
                ui_entry.transform.SetParent(invite_player_entry_parent);

                ui_entry.GetButton().onClick.AddListener(() =>
                {
                    clan.AddPlayer(player.uid);
                    UpdateMemberList();
                    UpdateClanPower();
                    UpdateMemberCount();
                    invite_player_window.SetActive(false);
                });
            }
        }
    }

    public void OpenKickPlayerWindow()
    {
        foreach (Transform child in invite_player_entry_parent)
        {
            Destroy(child.gameObject);
        }

        invite_player_window.SetActive(true);

        // TODO: iterating over all players is inefficient - add hot list of Players to each clan
        foreach (var player in PlayerManager.Instance.players)
        {
            if (clan.IsPlayerMember(player.uid) && !clan.IsPlayerOwner(player.uid))
            {
                PlayerEntryUI ui_entry = Instantiate(player_entry_prefab);
                ui_entry.Initialize(player);
                ui_entry.transform.SetParent(invite_player_entry_parent);

                ui_entry.GetButton().onClick.AddListener(() =>
                {
                    clan.RemovePlayer(player.uid);
                    UpdateMemberList();
                    UpdateClanPower();
                    UpdateMemberCount();
                    invite_player_window.SetActive(false);
                });
            }
        }
    }
}
