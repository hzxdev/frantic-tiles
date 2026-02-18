using Photon.Pun;
using Photon.Realtime;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Give Command", menuName = "Utilities/Give Command")]
    public class GiveCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string playerName = string.Join(" ", args);

            if (playerName == string.Empty)
            {
                Debug.Log("Usage:\n/give [playerName] [itemId] [amount]");
                return true;
            }

            if (args.Length != 3)
            {
                Debug.Log("Bad usage! Couldn't perform /give");
                return true;
            }

            ModControls[] pconts = FindObjectsOfType<ModControls>();


            for (int i = 0; i < pconts.Length; i++)
            {
                if (pconts[i].photonView.IsMine) //local mod control ("me")
                {
                    if (int.TryParse(args[1], out int itemId) && int.TryParse(args[2], out int amount))
                    {
                        pconts[i].GivePlayer(args[0], itemId, amount);
                    }

                   

                    break;
                }
            }





            return true;
        }
    }
}
