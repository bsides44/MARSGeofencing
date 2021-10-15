using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Unity.MARS.Conditions;

public class printLatLng_1 : MonoBehaviour
{
    public Text printText;
    public GameObject firstPrizeProxy;
    private string boundsText;
    private float _latitude;
    private float _longitude;
    private bool checkingForLocation = true;
    private bool locationAcquired = false;
    private double latUpperBoundary;
    private double latLowerBoundary;
    private double lngUpperBoundary;
    private double lngLowerBoundary;


    // Adapted from https://nosuchstudio.medium.com/how-to-access-gps-location-in-unity-521f1371a7e3
    IEnumerator LocationCoroutine() {
        checkingForLocation = true;

        // Permission checks
        #if UNITY_EDITOR

        #elif UNITY_ANDROID
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
            }

            if (!UnityEngine.Input.location.isEnabledByUser) {
                Debug.LogFormat("Android and Location not enabled");
                yield break;
            }

        #elif UNITY_IOS
            if (!UnityEngine.Input.location.isEnabledByUser) {
                Debug.LogFormat("IOS and Location not enabled");
                yield break;
            }
        #endif

        // Start location services
        UnityEngine.Input.location.Start(1f, 1f);
                
        // Wait until service initializes
        int maxWait = 15;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }

        // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
        #if UNITY_EDITOR
            int editorMaxWait = 15;
            while (UnityEngine.Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0) {
                yield return new WaitForSecondsRealtime(1);
                editorMaxWait--;
            }
        #endif

        // Location services failures
        if (maxWait < 1) {
            Debug.LogFormat("Timed out");
            yield break;
        }
        if (UnityEngine.Input.location.status != LocationServiceStatus.Running) {
            Debug.LogFormat("Unable to determine device location. Failed with status {0}", UnityEngine.Input.location.status);
            yield break;
        } else {
        // Location service success
        _latitude = UnityEngine.Input.location.lastData.latitude;
        _longitude = UnityEngine.Input.location.lastData.longitude;
        locationAcquired = true;
        }
        checkingForLocation = false;
    }

    private void Start() {
        printText.text = "Getting location...";
        StartCoroutine(LocationCoroutine());
        
        // Acquire Proxy geofence boundaries
        GeoFenceCondition geoFenceCondition = firstPrizeProxy.GetComponent<GeoFenceCondition>();

        latUpperBoundary = geoFenceCondition.BoundingBox.center.latitude + geoFenceCondition.BoundingBox.latitudeExtents;
        latLowerBoundary = geoFenceCondition.BoundingBox.center.latitude - geoFenceCondition.BoundingBox.latitudeExtents;
        lngUpperBoundary = geoFenceCondition.BoundingBox.center.longitude + geoFenceCondition.BoundingBox.longitudeExtents;
        lngLowerBoundary = geoFenceCondition.BoundingBox.center.longitude - geoFenceCondition.BoundingBox.longitudeExtents;
    }

    private void Update() {
        // Update location periodically
        if (!checkingForLocation) {
            StartCoroutine(LocationCoroutine());
        } 

        if (locationAcquired){
            // Make text green if user is in bounds
            if ((_latitude >= latLowerBoundary) && (_latitude <= latUpperBoundary) && (_longitude >= lngLowerBoundary) && (_longitude <= lngUpperBoundary)) {
                printText.GetComponent<Text>().color = Color.green;
                boundsText = "In bounds! ";
            }
            // Make text red if user is out of bounds
            if ((_latitude <= latLowerBoundary) || (_latitude >= latUpperBoundary) || (_longitude <= lngLowerBoundary) || (_longitude >= lngUpperBoundary)) {
                printText.GetComponent<Text>().color = Color.red;
                boundsText = "Out of bounds. ";
            }
            // Print location on screen
            printText.text = boundsText + "Current LatLng: \n" + _latitude + ", " + _longitude;
        }
    }

    private void OnDisable() {
        StopCoroutine(LocationCoroutine());
        UnityEngine.Input.location.Stop();
    }
}