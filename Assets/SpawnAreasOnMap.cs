using LoGaCulture.LUTE;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreasOnMap : MonoBehaviour
{
    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    Sprite[] _areaSprites; // Array of sprites for each area

    [Geocode]
    public string[] _locationStrings; // Location coordinates

    [SerializeField]
    float _baseHeightOffset = 10f; // Base height above map

    [SerializeField]
    Vector3[] _individualOffsets; // Per-instance position offsets

    [SerializeField]
    LUTELocationInfo[] locInfo;

    [Geocode]
    string[] _locationInfoStrings;

    int[] LocationId;

    // Zoom-based scaling parameters
    [SerializeField]
    private float _radiusInMeters = 10f;
    private const float MIN_SCALE = 0.1f;
    private const float MAX_SCALE = 1000f;

    private List<GameObject> _spriteHolders;
    private Vector2d[] _locations;

    public BasicFlowEngine flowEngineGlobal;

    [SerializeField]
    private float _fadeSpeed = 0.5f; // How fast the fade should happen

    public bool showAreas;
    public float fadeDelay;

    public bool[] firstTimeActivated;
    private SpriteRenderer[] _cachedRenderers = new SpriteRenderer[8];


    void Start()
    {
        flowEngineGlobal = GameObject.Find("Flow Engine").GetComponent<BasicFlowEngine>();

        // Initialize offsets array if empty
        if (_individualOffsets == null || _individualOffsets.Length != _areaSprites.Length)
        {
            _individualOffsets = new Vector3[_areaSprites.Length];
        }

        if (_areaSprites.Length != _locationStrings.Length)
        {
            Debug.LogError("Number of sprites must match number of locations");
            return;
        }

        _locations = new Vector2d[_locationStrings.Length];
        _spriteHolders = new List<GameObject>();

        for (int i = 0; i < _locationStrings.Length; i++)
        {
            try
            {
                _locations[i] = Conversions.StringToLatLon(_locationStrings[i]);
                CreateSpriteOverlay(i);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating area {i}: {e.Message}");
            }
        }

        for (int i = 0; i < 8; i++)
        {
            if (i < _spriteHolders.Count)
                _cachedRenderers[i] = _spriteHolders[i].GetComponent<SpriteRenderer>();
        }
        //StartCoroutine(DelayedShow(fadeDelay));
    }

    public void ShowAres()
    {
        StartCoroutine(DelayedShow(fadeDelay));
    }



    public IEnumerator DelayedShow(float secs)
    {
        yield return new WaitForSeconds(secs);

        showAreas = true;

    }
    void ShowNodeComplete()
    {
        flowEngineGlobal.ExecuteNode("Node Complete");
    }

    private void CreateSpriteOverlay(int index)
    {
        Vector3 worldPos = _map.GeoToWorldPosition(_locations[index], true);

        GameObject spriteHolder = new GameObject($"AreaSprite_{index}", typeof(SpriteRenderer));
        spriteHolder.transform.SetParent(transform);

        // Apply base position + individual offset
        spriteHolder.transform.position = worldPos +
            (Vector3.up * _baseHeightOffset) +
            _individualOffsets[index];

        // Initial scale will be set in Update
        spriteHolder.transform.rotation = Quaternion.Euler(90, 0, 0);

        SpriteRenderer renderer = spriteHolder.GetComponent<SpriteRenderer>();
        renderer.sprite = _areaSprites[index];
        renderer.sortingLayerName = "Overlay";
        renderer.sortingOrder = -20;
        renderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        renderer.material.color = new Color(1, 1, 1, 0f); // 60% opacity

        _spriteHolders.Add(spriteHolder);
    }

    void Update()
    {
        if (_map == null) return;

        // Handle position and scale updates
        for (int i = 0; i < _spriteHolders.Count; i++)
        {
            if (_spriteHolders[i] == null) continue;

            Vector3 worldPos = _map.GeoToWorldPosition(_locations[i], true);
            _spriteHolders[i].transform.position = worldPos +
                (Vector3.up * _baseHeightOffset) +
                _individualOffsets[i];

            UpdateSpriteScale(_spriteHolders[i], worldPos);
        }
        // if (showAreas)
        {
            // Handle fade effect for completed locations
            for (int a = 0; a < locInfo.Length; a++)
            {
                /*
                if (a >= _spriteHolders.Count) continue;

                var renderer = _spriteHolders[a].GetComponent<SpriteRenderer>();
                float targetAlpha = locInfo[a]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;

                // Simple lerp approach
                float currentAlpha = renderer.material.color.a;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);
                renderer.material.color = new Color(1, 1, 1, newAlpha);
                */


            }
            // Handle area [0]
            {
                var renderer = _cachedRenderers[0];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[0]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[0]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea0") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea0", true);
                            flowEngineGlobal.ExecuteNode("Saving");

                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea0"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [1]
            {
                var renderer = _cachedRenderers[1];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[1]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[1]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea1") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea1", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea1"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [2]
            {
                var renderer = _cachedRenderers[2];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[2]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[2]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea2") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea2", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea2"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [3]
            {
                var renderer = _cachedRenderers[3];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[3]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[3]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea3") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea3", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea3"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [4]
            {
                var renderer = _cachedRenderers[4];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[4]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[4]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea4") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea4", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea4"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [5]
            {
                var renderer = _cachedRenderers[5];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[5]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[5]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea5") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea5", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea5"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [6]
            {
                var renderer = _cachedRenderers[6];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[6]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[6]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea6") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea6", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea6"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

            // Handle area [7]
            {
                var renderer = _cachedRenderers[7];
                float currentAlpha = renderer.material.color.a;
                float targetAlpha = locInfo[7]._LocationStatus == LUTELocationInfo.LocationStatus.Completed ? 0.7f : 0f;
                float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, _fadeSpeed * Time.deltaTime);

                if (showAreas)
                {
                    if (locInfo[7]._LocationStatus == LUTELocationInfo.LocationStatus.Completed &&
                        !flowEngineGlobal.GetBooleanVariable("LoadArea7") &&
                        !Mathf.Approximately(currentAlpha, 0.7f))
                    {
                        ShowNodeComplete();
                        renderer.material.color = new Color(1, 1, 1, newAlpha);

                        if (Mathf.Approximately(newAlpha, 0.7f))
                        {
                            flowEngineGlobal.SetBooleanVariable("LoadArea7", true);
                            flowEngineGlobal.ExecuteNode("Saving");
                        }
                    }
                }

                if (flowEngineGlobal.GetBooleanVariable("LoadArea7"))
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
            }

        }
    }

    private void UpdateSpriteScale(GameObject spriteObject, Vector3 centerPosition)
    {
        if (spriteObject == null || _map == null) return;

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
        spriteObject.transform.localScale = new Vector3(scale, scale, scale);
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

    public void SetOffset(int index, Vector3 offset)
    {
        if (index >= 0 && index < _individualOffsets.Length)
        {
            _individualOffsets[index] = offset;
        }
    }


}



