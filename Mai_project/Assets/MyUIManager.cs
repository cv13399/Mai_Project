using UnityEngine;
using System.Collections;

public class MyUIManager : MonoBehaviour 
{
    public UILabel loadingLabel;

    public UIPanel loadingPanel;
    public UIPanel loginPanel;
    public UIPanel gameSelectPanel;
    public UIPanel googleMapPanel;

    public UIEventListener LoginBtn;
    public UIEventListener exitMapBtn;
    public UIEventListener[] gameSelectBtn;

    public GoogleMap gmp;

    public GameObject BG;

    bool isPassed = true;

	void Start () 
    {
        googleMapPanel.transform.position = new Vector3( googleMapPanel.transform.position.x, googleMapPanel.transform.position.y,1000);
        ButtonEventInitialize();
        //StartCoroutine(LocationConfirm());
        gmp.Refresh();
	}
    void ButtonEventInitialize()
    {
        LoginBtn.onClick += (GameObject g) =>
        {
         StartCoroutine(OnLogin());
        };
        exitMapBtn.onClick += (GameObject g) =>
        {
            googleMapPanel.gameObject.SetActive(false);
            gameSelectPanel.gameObject.SetActive(true);
            BG.SetActive(true);
        };
        for(uint i=0; i < gameSelectBtn.Length; i++)
            gameSelectBtn[i].onClick += (GameObject g) =>
            {
                OnGameSelected(g);
            };
    }
    void OnGameSelected(GameObject mg)
    {
        print("Game selected");
        BG.SetActive(false);
        gameSelectPanel.gameObject.SetActive(false);
        googleMapPanel.gameObject.SetActive(true);
        googleMapPanel.transform.position = new Vector3( googleMapPanel.transform.position.x, googleMapPanel.transform.position.y,0);
    }
    
    public IEnumerator LoadingAnimation()
    {     
        string[] text = new string[] { "Loading", "Loading.", "Loading..", "Loading..." ,"Loading","Loading.","Loading..","Loading..."};
        for (int i = 0; i < text.Length; i++)
        {
            loadingLabel.text = text[i];
            yield return new WaitForSeconds(0.4f);      
        }
        if(!isPassed)
        StartCoroutine(LoadingAnimation());
    }
    public IEnumerator OnLogin()
    {
        print("Login");
        loadingPanel.gameObject.SetActive(true);
        loginPanel.gameObject.SetActive(false);      
        StartCoroutine(LoadingAnimation());
        yield return new WaitForSeconds(1.5f);
        if (isPassed)
        {
            StopCoroutine(LoadingAnimation());
            gameSelectPanel.gameObject.SetActive(true); 
            loadingPanel.gameObject.SetActive(false);
        }
        else
        {
             StopCoroutine(LoadingAnimation());
             StartCoroutine(LoadingAnimation());
        }
        yield return null;
    }

    public IEnumerator  LocationConfirm()
	{
		 // First, check if user has location service enabled
     	if (!Input.location.isEnabledByUser)
		 {
			//isPassed = false;
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
            StartCoroutine(LocationConfirm());
            yield break;
        }
		 // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            StartCoroutine(LocationConfirm());
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			gmp.centerLocation.longitude = Input.location.lastData.longitude;
			gmp.centerLocation.latitude = Input.location.lastData.latitude;
			gmp.Refresh();          
            isPassed = true;
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

	}
}
