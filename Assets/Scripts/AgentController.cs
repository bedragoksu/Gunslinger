using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Actions _actions;
    private class InfoPlayer
    {
        public string Name;
        public int CurrentBulletNumber;
        public bool IsAlive;
        public int CardNumber;
        public PlayerModel.TypeOfPlayer Role;

        public InfoPlayer(string Name, int CurrentBulletNumber, int CardNumber, PlayerModel.TypeOfPlayer Role = PlayerModel.TypeOfPlayer.Bos)
        {
            this.Name = Name;
            this.CurrentBulletNumber = CurrentBulletNumber;
            //this.IsAlive = IsAlive;
            this.CardNumber = CardNumber;
            this.Role = Role;
        }
    }

    // currentbulletpoint, IsAlive, cardnum, varsa rol, name,acik kartlar
    public void AgentDecideToPlay(GameObject AgentPlayer)
    {
        PlayerModel AgentPlayerModel = AgentPlayer.GetComponent<PlayerModel>();
        Dictionary<int, InfoPlayer> PlayerInfos = CreatePlayerInfos(AgentPlayerModel);

        // decide tree



    }



    private Dictionary<int, InfoPlayer> CreatePlayerInfos(PlayerModel AgentPlayerModel)
    {
        Dictionary<int, InfoPlayer> PlayerInfos = new Dictionary<int, InfoPlayer>();
        foreach (GameObject player in _gameManager._turns)
        {
            PlayerModel playerModel = player.GetComponent<PlayerModel>();
            if (playerModel.PlayerID != AgentPlayerModel.PlayerID && playerModel.IsAlive)
            {
                InfoPlayer info = new (playerModel.PlayerName, playerModel.CurrentBulletPoint, playerModel.openHand.Count,
                                                    (playerModel.PlayerRole == PlayerModel.TypeOfPlayer.Sheriff) ? PlayerModel.TypeOfPlayer.Sheriff : PlayerModel.TypeOfPlayer.Bos);
                PlayerInfos.Add(playerModel.PlayerID, info);
            }
        }

        return PlayerInfos;
    }

    
}
