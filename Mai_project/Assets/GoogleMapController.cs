using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleMapController : MonoBehaviour 
{
	private static GoogleMapController instance;
	public static GoogleMapController Instance
	{
		get
		{
			if(instance == null)
				instance = FindObjectOfType(typeof(GoogleMapController)) as GoogleMapController;
			return instance;
		}
	}
	
	public GoogleMap gmp;
	public Text mytext;

	void Awake()
	{
		print("open");
	  StartCoroutine(LocationConfirm());
	}
	
	
	public IEnumerator  LocationConfirm()
	{
		 // First, check if user has location service enabled
     	if (!Input.location.isEnabledByUser)
		 {
			 print("break");
			 mytext.text = "break";
            yield break;
		 }
		 // Start service before querying location
        Input.location.Start();
		 // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
		 // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
			 mytext.text ="time out";
            yield break;
        }
		 // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			gmp.centerLocation.longitude = Input.location.lastData.longitude;
			gmp.centerLocation.latitude = Input.location.lastData.latitude;
			mytext.text ="有座標  " + Input.location.lastData.latitude;
			gmp.Refresh();
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

	}
}
