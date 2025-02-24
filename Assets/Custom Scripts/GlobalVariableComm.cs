using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoGaCulture.LUTE
{
    public class GlobalVariableComm : MonoBehaviour
    {
        public BasicFlowEngine flowEngineGlobal;
        public bool flowEngineBool1;
        string LocationName;

        void Start()
        {

            flowEngineGlobal = GameObject.Find("GlobalVariablesEngine").GetComponent<BasicFlowEngine>();
            LocationName = SceneManager.GetActiveScene().name;
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Completed();
            }
        }


        void Completed()
        {
            flowEngineGlobal.SetBooleanVariable(LocationName, true);
        }

    }
}
