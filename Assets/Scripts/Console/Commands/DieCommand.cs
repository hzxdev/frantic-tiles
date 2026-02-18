using Photon.Realtime;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Die Command", menuName = "Utilities/Die Command")]
    public class DieCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            PlayerController[] pconts = FindObjectsOfType<PlayerController>();
            Player player;

            for (int i = 0; i < pconts.Length; i++)
            {
                if (pconts[i].photonView.IsMine)
                {
                    if (pconts[i].isAlive)
                        pconts[i].Die();
                    
                    player = pconts[i].photonView.Owner;
                    Debug.Log("Succesful suicide by " + player.NickName);
                    break;
                }
            }

            

            return true;
        }
    }
}