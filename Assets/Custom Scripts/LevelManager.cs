using LoGaCulture.LUTE;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LoGaCulture.LUTE
{
    public class LevelManager : MonoBehaviour
    {
        public BasicFlowEngine flowEngineGlobal;

        public bool flowEngineBool1;
        SharedData sharedData;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //flowEngine.SetBooleanVariable("Bool_1", false);

            flowEngineGlobal = GameObject.Find("GlobalVariablesEngine").GetComponent<BasicFlowEngine>();


        }

        // Update is called once per frame
        void Update()
        {

            //flowEngine.ExecuteNode("Complete Mighty Stone");


            //flowEngineGlobal.SetBooleanVariable("Bool_1", flowEngineBool1);


        }
    }
}
