using Photon.Pun;
using Photon.Realtime;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Giveself Command", menuName = "Utilities/Giveself Command")]
    public class GiveSelfCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string playerName = string.Join(" ", args);




            


            ModControls[] pconts = FindObjectsOfType<ModControls>();
            ChatManager cm = new ChatManager();

            for (int i = 0; i < pconts.Length; i++)
            {
                if ((!PhotonNetwork.OfflineMode && pconts[i].photonView.IsMine) || PhotonNetwork.OfflineMode) //local mod control ("me")
                {
                    cm = pconts[i].transform.parent.GetComponentInChildren<ChatManager>();

                    if (playerName == string.Empty)
                    {
                        cm.Chat("Usage:\n/giveself [itemId] [amount]. Works offline too.", MessageType.System);
                        return true;
                    }

                    if (args.Length != 2)
                    {
                        cm.Chat("Bad usage! Couldn't perform /give", MessageType.System);
                        return true;
                    }

                    if (int.TryParse(args[0], out int itemId) && int.TryParse(args[1], out int amount))
                    {
                        pconts[i].GiveToSelf(itemId, amount);
                    }



                    break;
                }
            }


            






            return true;
        }
    }
}
