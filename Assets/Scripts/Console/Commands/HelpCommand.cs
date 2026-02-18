using Photon.Realtime;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Help Command", menuName = "Utilities/Help Command")]
    public class HelpCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            ModControls[] pconts = FindObjectsOfType<ModControls>();
            Player player;

            for (int i = 0; i < pconts.Length; i++)
            {
                if (pconts[i].photonView.IsMine)
                {
                    pconts[i].HelpCommand();


                    break;
                }
            }



            return true;
        }
    }
}
