using UnityEngine;
using System.Collections;

public class MyUIManager : MonoBehaviour 
{
    public  UILabel loadingLabel;
    public  UIPanel loadingPanel;
    public UIEventListener LoginBtn;
	// Use this for initialization
	void Start () 
    {
        ButtonEventInitialize();
	}
    void ButtonEventInitialize()
    {
        LoginBtn.onClick += (GameObject g) =>
        {
            print("Login");
        };
    }

    public IEnumerator LoadingAnimation()
    {
        loadingPanel.gameObject.SetActive(true);
        string[] text = new string[] { "Loading", "Loading.", "Loading..", "Loading..." };
        for (int i = 0; i < text.Length; i++)
        {
            loadingLabel.text = text[i];
            yield return new WaitForSeconds(0.5f);      
        }
        loadingPanel.gameObject.SetActive(false);
    }
}
