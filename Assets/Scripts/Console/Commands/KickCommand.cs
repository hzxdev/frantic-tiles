using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Kick Command", menuName = "Utilities/Kick Command")]
    public class KickCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string playerName = string.Join(" ", args);

            if (playerName == string.Empty)
            {
                Debug.Log("Usage:\n/kick [playerName]");
                return true;
            }
                

            ModControls[] pconts = FindObjectsOfType<ModControls>();


            for (int i = 0; i < pconts.Length; i++)
            {
                if (pconts[i].photonView.IsMine) //local mod control ("me")
                {
                    pconts[i].KickPlayer(playerName);
                    break;
                }
            }



          

            return true;
        }
    }
}