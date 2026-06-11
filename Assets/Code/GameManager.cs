using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CreateMainPlayer();
        GenerateRandomPlayers();
        GenerateRandomClans();
    }

    private void CreateMainPlayer()
    {
        var main_player = new Player();
        main_player.player_name = "YOU!";
        main_player.power = 1000;
        main_player.uid = Utilities.GenerateUID();
        main_player.clan_uid = "";

        PlayerManager.Instance.MainPlayer = main_player;
        PlayerManager.Instance.players.Add(main_player);

        ClanManager.Instance.GetUIManager().GetMainPlayerInfoUI().UpdateUI();
    }

    private void GenerateRandomPlayers()
    {
        int player_count = 200;

        for (int i = 0; i < player_count; i++)
        {
            var player = new Player();
            player.player_name = $"GeneratedPlayer-{i.ToString()}";
            player.power = Random.Range(1, Player.max_power + 1);
            player.uid = Utilities.GenerateUID();

            PlayerManager.Instance.players.Add(player);
        }
    }

    private void GenerateRandomClans()
    {
        int clan_count = 3;

        for (int i = 0; i < clan_count; i++)
        {
            var clan = new Clan();
            clan.clan_name = $"GeneratedClan-{i.ToString()}";
            clan.uid = Utilities.GenerateUID();

            var players_to_add = PlayerManager.Instance.GetListOfRandomPlayersNotInAnyClan(Random.Range(1, 10));

            if (players_to_add.Count == 0)
            {
                Debug.LogError("GenerateRandomClans: Got 0 players !");
                return;
            }

            clan.owner_player_uid = players_to_add[0].uid;

            foreach (var player_to_add in players_to_add)
            {
                clan.AddPlayer(player_to_add.uid);
            }

            ClanManager.Instance.AddClan(clan);
        }
    }
}
