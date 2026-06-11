using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MainPlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player_name;
    [SerializeField] private TextMeshProUGUI player_power;
    [SerializeField] private TextMeshProUGUI player_clan;

    public void UpdateUI()
    {
        var main_player = PlayerManager.Instance.MainPlayer;

        player_name.text = main_player.player_name;
        player_power.text = main_player.power.ToString();

        if (string.IsNullOrEmpty(main_player.clan_uid))
        {
            player_clan.text = "N/A";
        }
        else
        {
            var clan = ClanManager.Instance.GetClan(main_player.clan_uid);

            if (clan == null)
            {
                Debug.LogError($"Serious Error ! Trying to get nonexistent clan ! UID: {main_player.clan_uid}");
            }
            else
            {
                player_clan.text = clan.clan_name;
            }
        }

    }
}
