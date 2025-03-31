namespace Mapbox.Examples
{
    using Mapbox.Unity.Map;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class QuadTreeCameraMovement : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 20)]
        public float _panSpeed = 1.0f;

        //[SerializeField]
        //float _zoomSpeed = 0.25f;  // Commented out - we'll use camera movement instead

        [SerializeField]
        public Camera _referenceCamera;
        [SerializeField]
        public Camera _referenceCameraGame;

        [SerializeField]
        public AbstractMap _mapManager;

        [SerializeField]
        bool _useDegreeMethod;

        [SerializeField] protected bool allowPanning;
        //[SerializeField] protected bool allowZooming;  // Commented out - we'll handle zoom differently
        [SerializeField] protected bool allowTilting;

        //[Range(0, 21)]
        //[SerializeField] protected float minZoomLevel = 0.0f;  // Commented out
        //[Range(0, 21)]
        //[SerializeField] protected float maxZoomLevel = 21.0f;  // Commented out

        [SerializeField] float sensitivityZ = 2f;   // Horizontal sensitivity

        [HideInInspector]
        public bool _dragStartedOnUI = false;

        private Vector3 _origin;
        private Vector3 _mousePosition;
        private Vector3 _mousePositionPrevious;
        private bool _shouldDrag;
        private bool _isInitialized = false;
        private Plane _groundPlane = new Plane(Vector3.up, 0);
        private float rotationZ = 0f;  // Vertical rotation
        private Vector3 defaultRotation;

        // New variables for camera zoom
        [SerializeField] float cameraZoomSpeed = 5f;
        [SerializeField] float minCameraDistance = 10f;
        [SerializeField] float maxCameraDistance = 1000f;

        public static QuadTreeCameraMovement _instance;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            if (null == _referenceCamera)
            {
                _referenceCamera = GetComponent<Camera>();
            }
            _mapManager.OnInitialized += () =>
            {
                _isInitialized = true;
            };
        }

        void Start()
        {
            if (_referenceCameraGame != null)
                _referenceCamera = _referenceCameraGame;

            defaultRotation = _referenceCameraGame.transform.localEulerAngles;
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
            {
                _dragStartedOnUI = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _dragStartedOnUI = false;
            }
        }

        private void LateUpdate()
        {
            if (!_isInitialized) { return; }

            if (!_dragStartedOnUI)
            {
                if (Input.touchSupported && Input.touchCount > 0)
                {
                    HandleTouch();
                }
                else
                {
                    HandleMouseAndKeyBoard();
                }
            }
        }

        void HandleMouseAndKeyBoard()
        {
            // Handle camera zoom instead of map zoom
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDelta != 0f)
            {
                ZoomCamera(scrollDelta);
            }

            //pan keyboard
            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");

            if (allowPanning)
                PanMapUsingKeyBoard(xMove, zMove);

            // If right mouse button is held, rotate/pan the camera
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                if (allowTilting)
                    PanOrRotateCamera(mouseX);
            }

            //pan mouse
            if (allowPanning)
                PanMapUsingTouchOrMouse();
        }

        void HandleTouch()
        {
            switch (Input.touchCount)
            {
                case 1:
                    {
                        if (allowPanning)
                            PanMapUsingTouchOrMouse();

                        Touch touch = Input.GetTouch(0);
                        float touchX = touch.deltaPosition.x;
                        float touchY = touch.deltaPosition.y;

                        if (allowTilting)
                            PanOrRotateCamera(touchX);
                    }
                    break;
                case 2:
                    {
                        // Handle pinch-to-zoom for camera
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                        float zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
                        ZoomCamera(zoomFactor);
                    }
                    break;
                default:
                    break;
            }
        }

        void ZoomCamera(float zoomDelta)
        {
            // Move camera forward/backward to simulate zoom
            float newPosition = _referenceCamera.transform.localPosition.y - zoomDelta * cameraZoomSpeed;
            newPosition = Mathf.Clamp(newPosition, minCameraDistance, maxCameraDistance);
            _referenceCamera.transform.localPosition = new Vector3(
                _referenceCamera.transform.localPosition.x,
                newPosition,
                _referenceCamera.transform.localPosition.z);
        }

        void PanOrRotateCamera(float zInput)
        {
            rotationZ += zInput * sensitivityZ;
            _referenceCameraGame.transform.localEulerAngles = new Vector3(defaultRotation.x, defaultRotation.y, rotationZ);
        }

        public void ZoomMapUsingTouchOrMouse(float zoomFactor)
        {/*
            var zoom = Mathf.Max(minZoomLevel, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, maxZoomLevel));
            if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
            {
                _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
            }
            */
        }


        public void PanMapUsingKeyBoard(float xMove, float zMove)
        {
            if (Math.Abs(xMove) > 0.0f || Math.Abs(zMove) > 0.0f)
            {
                float factor = _panSpeed * (Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom));

                var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + zMove * factor * 2.0f, _mapManager.CenterLatitudeLongitude.y + xMove * factor * 4.0f);

                _mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
            }
        }

        public void PanMapUsingTouchOrMouse()
        {
            if (_useDegreeMethod)
            {
                UseDegreeConversion();
            }
            else
            {
                UseMeterConversion();
            }
        }

        public void PanMapUsingTouchOrMouseEditor(Event e)
        {
            UseMeterConversionEditor(e);
        }

        void UseMeterConversion()
        {
            if (Input.GetMouseButtonUp(1))
            {
                var mousePosScreen = Input.mousePosition;
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                var pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                var latlongDelta = _mapManager.WorldToGeoPosition(pos);
            }

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                _mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                            var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, offset.z * factor));
                            var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

                            _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        void UseMeterConversionEditor(Event e)
        {
            if (_dragStartedOnUI)
            {
                var mousePosScreen = e.mousePosition;
                var newLocVec = new Vector3(mousePosScreen.x, mousePosScreen.y, _referenceCamera.transform.localPosition.y);
                _mousePosition = _referenceCamera.ScreenToWorldPoint(newLocVec);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(newLocVec);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                            var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, -offset.z * factor));
                            var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

                            _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        void UseDegreeConversion()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                _mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            float factor = _panSpeed * Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                            var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + offset.z * factor, _mapManager.CenterLatitudeLongitude.y + offset.x * factor);
                            _mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        private Vector3 getGroundPlaneHitPoint(Ray ray)
        {
            float distance;
            if (!_groundPlane.Raycast(ray, out distance)) { return Vector3.zero; }
            return ray.GetPoint(distance);
        }
    }
}