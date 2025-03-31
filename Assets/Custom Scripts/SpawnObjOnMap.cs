namespace Mapbox.Examples
{
    using LoGaCulture.LUTE;
    using Mapbox.Unity.Map;
    using Mapbox.Unity.MeshGeneration.Factories;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using MoreMountains.Feedbacks;
    using System.Collections.Generic;
    using System.Diagnostics.Tracing;
    using UnityEngine;

    public class SpawnObjOnMap : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        GameObject FlowerParticlePrefab;

        [SerializeField]
        LUTELocationInfo[] locInfo;

        [Geocode]
        string[] _locationStrings;

        int[] LocationId;


        [SerializeField]
        float _spawnScale = 100f;

        public float heightOffset = 0f;

        private List<GameObject> _spawnedObjects;
        private Vector2d[] _locations;

        // Define constants for scaling
        private const float MIN_SCALE = 0.1f;
        private const float MAX_SCALE = 1000f;
        // private const float _radiusInMeters = 10f; // Define a default radius in meters

        void Start()
        {
            _locationStrings = new string[locInfo.Length];
            LocationId = new int[locInfo.Length];
            _locations = new Vector2d[_locationStrings.Length];
            _spawnedObjects = new List<GameObject>();


            for (int a = 0; a < locInfo.Length; a++)
            {
                _locationStrings[a] = locInfo[a].Position;
                LocationId[a] = int.Parse(locInfo[a].infoID);

            }

            for (int i = 0; i < _locationStrings.Length; i++)
            {

                // Convert location string to LatLon
                _locations[i] = Conversions.StringToLatLon(_locationStrings[i]);

                // Obtain the world position from the geo coordinates
                Vector3 worldPosition = _map.GeoToWorldPosition(_locations[i], true);

                // Apply an offset to the height (y-axis)
                worldPosition.y += heightOffset;

                // Instantiate the marker prefab (FlowerParticlePrefab)
                var instance = Instantiate(FlowerParticlePrefab);

                // Ensure the instance has the EventPointer component (if needed)
                var eventPointer = instance.GetComponent<EventPointer>();
                if (eventPointer != null)
                {
                    eventPointer.eventPos = _locations[i];
                    // eventPointer.eventID = i + 1;

                    eventPointer.eventID = LocationId[i];
                }

                // Set the instance's position and initial scale
                instance.transform.position = worldPosition; // Use world position
                instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

                // Add the instance to the list of spawned objects
                _spawnedObjects.Add(instance);

            }
        }

        private void Update()
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];

                // Obtain the updated world position from the geo coordinates
                Vector3 worldPosition = _map.GeoToWorldPosition(location, true);

                // Apply the height offset
                worldPosition.y += heightOffset;

                // Update the spawned object's position
                spawnedObject.transform.position = worldPosition; // Use world position

                // Update scale for the FlowerParticlePrefab
                // UpdateRadiusCircleScale(spawnedObject, worldPosition);
            }

            for (int a = 0; a < locInfo.Length; a++)
            {

                if (locInfo[a]._LocationStatus == LUTELocationInfo.LocationStatus.Completed)
                {
                    _spawnedObjects[a].SetActive(true);
                }
            }
        }

        /*
        private void UpdateRadiusCircleScale(GameObject radiusCircle, Vector3 centerPosition)
        {
            if (radiusCircle == null || _map == null) return;

            // Calculate scale based on zoom level
            float zoomLevel = _map.Zoom;
            float metersPerPixel = CalculateMetersPerPixel(zoomLevel, centerPosition);
            float pixelScale = _radiusInMeters / metersPerPixel;

            // Apply scale, ensuring it's within acceptable bounds
            float scale = Mathf.Clamp(pixelScale, MIN_SCALE, MAX_SCALE);

            // Check for NaN or Infinity
            if (float.IsNaN(scale) || float.IsInfinity(scale))
            {
                scale = 1f; // Fallback to a default scale
            }

            // Apply the scale to the object
            radiusCircle.transform.localScale = new Vector3(scale, scale, scale);
        }

        private float CalculateMetersPerPixel(float zoomLevel, Vector3 centerPosition)
        {
            // Convert center position to geo coordinates
            Vector2d centerGeoPosition = _map.WorldToGeoPosition(centerPosition);

            // Calculate meters per pixel at the equator for the current zoom level
            float metersPerPixelAtEquator = 156543.03f / Mathf.Pow(2, zoomLevel);

            // Adjust for the current latitude
            float latitudeRadians = Mathf.Deg2Rad * (float)centerGeoPosition.x;
            float metersPerPixel = metersPerPixelAtEquator * Mathf.Cos(latitudeRadians);

            return metersPerPixel;
        }
        */
    }
}