using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClanEntryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clan_name;
    [SerializeField] GameObject private_icon;
    [SerializeField] Button button;

    public Button GetButton() => button;

    public Clan clan;

    public void Initialize(Clan clan_p)
    {
        clan = clan_p;
        clan_name.text = clan.clan_name;

        private_icon.SetActive(clan.private_clan);
    }
}
