using DapperDino.UDCT.Utilities.DeveloperConsole.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DapperDino.UDCT.Utilities.DeveloperConsole
{
    public class DeveloperConsoleBehaviour : MonoBehaviour
    {
    public string prefix = string.Empty;
        [SerializeField] public ConsoleCommand[] commands = new ConsoleCommand[0];




        // private float pausedTimeScale;


        private static DeveloperConsoleBehaviour instance;

        private DeveloperConsole developerConsole;
        private DeveloperConsole DeveloperConsole
        {
            get
            {
                if (developerConsole != null) { return developerConsole; }
                return developerConsole = new DeveloperConsole(prefix, commands);
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
          
        }

        

        public void ProcessCommand(string inputValue) //ONLY commands go here, starting with the prefix
        {
            DeveloperConsole.ProcessCommand(inputValue);

         
        }

        private void Update()
        {
            

          
        }

      
       
    }
}