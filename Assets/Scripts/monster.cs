using UnityEngine;
using System.Collections;

public class monster : MonoBehaviour {

    public GameObject preb;
    public int atk;
    public int def;
    public int hp;

    // Use this for initialization
    void Start () {
        Debug.Log("dsgr");
        createHealthBar();
	}
	
	// Update is called once per frame
	void Update () {
	     
	}
    void createHealthBar()
    {
        var tmp = GameObject.Instantiate(preb,new Vector3(this.transform.position.x, this.transform.position.y -60, 0), Quaternion.identity) as GameObject;
        tmp.transform.SetParent(transform);
    }
}
