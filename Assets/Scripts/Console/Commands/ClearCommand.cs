using Photon.Realtime;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Clear Command", menuName = "Utilities/Clear Command")]
    public class ClearCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            PlayerController[] pconts = FindObjectsOfType<PlayerController>();
            Player player;

            for (int i = 0; i < pconts.Length; i++)
            {
                if (pconts[i].photonView.IsMine)
                {
                   
                        pconts[i].itemsManager .Clear();

                    player = pconts[i].photonView.Owner;
                    pconts[i].audioManager.PlayOneShot("command", false);
                    Debug.Log("Succesful inventory cleanup by " + player.NickName);
                    break;
                }
            }



            return true;
        }
    }
}