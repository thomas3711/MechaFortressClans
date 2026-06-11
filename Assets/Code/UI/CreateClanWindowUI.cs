using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CreateClanWindowUI : MonoBehaviour
{
    [SerializeField] TMP_InputField input_field;
    [SerializeField] Toggle private_toggle;

    void Update()
    {
    }

    public void CreateClan()
    {
        var main_player = PlayerManager.Instance.MainPlayer;

        if (!main_player.IsAllowedToJoinClan())
        {
            DebugInfoUI.Instance.Message($"You can't join clan right now ! Wait {main_player.TimeRemainingToJoinClan()} seconds !");
            return;
        }

        main_player.LeaveClan();

        var new_clan = new Clan();
        new_clan.clan_name = input_field.text;
        new_clan.private_clan = private_toggle.isOn;
        new_clan.uid = Utilities.GenerateUID();

        new_clan.AddOwner(main_player.uid);

        ClanManager.Instance.AddClan(new_clan);
        ClanManager.Instance.GetUIManager().GetMainPlayerInfoUI().UpdateUI();
        ClanManager.Instance.GetUIManager().UpdateLeaveClanButtonState();

        this.gameObject.SetActive(false);
    }
}
