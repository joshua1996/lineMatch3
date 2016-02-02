using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class restart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button btn = gameObject.GetComponent<Button>();

        btn.onClick.AddListener(click);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void click()
    {
        SceneManager.LoadScene("testUGUI");
    }
}
