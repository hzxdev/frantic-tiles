using Photon.Pun;
using Photon.Realtime;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Reconnect Command", menuName = "Utilities/Reconnect Command")]
    public class ReconnectCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            PhotonNetwork.LoadLevel(0);
            return true;
        }
    }
}
