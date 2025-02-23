using UnityEngine;

namespace LoGaCulture.LUTE
{
    public class TestGlobalVariable : MonoBehaviour
    {
        public BasicFlowEngine flowEngineGlobal;
        public bool flowEngineBool1;
        void Start()
        {

            flowEngineGlobal = GameObject.Find("GlobalVariablesEngine").GetComponent<BasicFlowEngine>();

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
            flowEngineGlobal.SetBooleanVariable("Bool_1", true);
        }

    }
}
