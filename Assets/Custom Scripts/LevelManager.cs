using LoGaCulture.LUTE;
using Mapbox.Unity.Map;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Mapbox.Unity.Map
{
    public class LevelManager : MonoBehaviour
    {
        BasicFlowEngine flowEngine;
        public AbstractMap map;
        string mapIncomplete = "mapbox://styles/nino94/cm80q0guc00yv01qu2rsybx1l";
        string mapComplete = "mapbox://styles/nino94/cm7wjvhbp00m701r124sgghoj";

        [SerializeField]
        protected ImageryLayer _imagery = new ImageryLayer();

        void Start()
        {
            flowEngine = GameObject.Find("Flow Engine").GetComponent<BasicFlowEngine>();
        }

        // Update is called once per frame
        void Update()
        {
            if (flowEngine.GetBooleanVariable("MapCompleted") == true)
            {

                IImageryLayer imageryLayer = map.ImageLayer;

                // Set the style URL using the extension method
                map.ImageLayer.SetLayerSource(mapComplete);
            }
            else
            {
                map.ImageLayer.SetLayerSource(mapIncomplete);
            }
        }
        public void LoadScene_Nino(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
