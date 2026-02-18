using Photon.Realtime;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Cheat Command", menuName = "Utilities/Cheat Command")]
    public class CheatCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            PlayerController[] pconts = FindObjectsOfType<PlayerController>();
            ItemsManager itemsManager;
            Player player;

            for (int i = 0; i < pconts.Length; i++)
            {
                if (pconts[i].photonView.IsMine)
                {
                    itemsManager = pconts[i].itemsManager;
                    player = pconts[i].photonView.Owner;
                    for (int j = 0; j < 8; j++)
                    {
                        itemsManager.AddItem(itemsManager.tilesInteraction.allItems[j], 200, null);
                    }
                    pconts[i].audioManager.PlayOneShot("command", false);
                    Debug.Log(player.NickName + " cheats the game!");
                    break;
                }
            }



            return true;
        }
    }
}