using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerEntryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player_name;
    [SerializeField] Button button;

    public Button GetButton() => button;

    public void Initialize(Player player)
    {
        player_name.text = player.player_name;
    }
}
