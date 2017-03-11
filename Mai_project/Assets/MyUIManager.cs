using UnityEngine;
using System.Collections;
using System;

public class UserInfo
{
    public string userName = "";
    public string userID = "";
}

public class MyUIManager : MonoBehaviour 
{
    public UILabel loadingLabel;
    public UILabel nameLabel;

    public UIPanel loadingPanel;
    public UIPanel loginPanel;
    public UIPanel gameSelectPanel;
    public UIPanel googleMapPanel;

    public UIEventListener GoogleLoginBtn;
    public UIEventListener FbLoginBtn;
    public UIEventListener exitMapBtn;
    public UIEventListener[] gameSelectBtn;

    public GoogleMap gmp;

    public GameObject BG;
    public UserInfo user = new UserInfo();

	void Start () 
    {
        googleMapPanel.transform.position = new Vector3( googleMapPanel.transform.position.x, googleMapPanel.transform.position.y,1000);
        ButtonEventInitialize();
        //StartCoroutine(LocationConfirm());
        gmp.Refresh();
	}
    void ButtonEventInitialize()
    {
        GoogleLoginBtn.onClick += (GameObject g) =>
        {
            //StartCoroutine(OnLogin());
            OnGoogleLogin();
        };
        exitMapBtn.onClick += (GameObject g) =>
        {
            googleMapPanel.gameObject.SetActive(false);
            gameSelectPanel.gameObject.SetActive(true);
            BG.SetActive(true);
        };
        for(short i=0; i < gameSelectBtn.Length; i++)
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
        for (short i = 0; i < text.Length; i++)
        {
            loadingLabel.text = text[i];
            yield return new WaitForSeconds(0.4f);      
        }
        StartCoroutine(LoadingAnimation());
    }
    public void AfterLogin()
    {  
        StopCoroutine(LoadingAnimation());
        gameSelectPanel.gameObject.SetActive(true); 
        loadingPanel.gameObject.SetActive(false);
    }
    public void BeforeLogin()
    {
        loadingPanel.gameObject.SetActive(true);
        loginPanel.gameObject.SetActive(false);
        StartCoroutine(LoadingAnimation());
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
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

	}
    //google plus call back to unity
    public void OnConnected(string data)
    {
        string[] seperators = new string[] { "/" };
        string[] info = data.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
        user.userName = info[0];
        user.userID = info[1];
        nameLabel.text = "Hi "+user.userName;
        AfterLogin();
    }
    public void OnGoogleLogin()
    {
        BeforeLogin();
        using (AndroidJavaClass unity = new AndroidJavaClass("com.test.tw.test.MyDialog"))
        {
            unity.CallStatic("Login", this.gameObject.name, "OnConnected");
        }
    }
}
