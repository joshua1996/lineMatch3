using UnityEngine;
using System.Collections;

public class monster : MonoBehaviour {

    public GameObject preb;
    public int atk;
    public int def;
    public int hp;
    private float height_y;
    //DrawImage tett;
    
    // Use this for initialization
    void Start () {
        createHealthBar();
        damage();
        height_y=this.GetComponent<RectTransform>().rect.width;

    }
	
	// Update is called once per frame
	void Update () {
    }
    void createHealthBar()
    {
        Vector3 position = new Vector3(this.transform.position.x,height_y-120, 0);
        var tmp = GameObject.Instantiate(preb,Vector3.zero, Quaternion.identity) as GameObject;
        tmp.transform.SetParent(transform);
        tmp.transform.localScale = Vector3.one;
        tmp.transform.localPosition = position;
        tmp.name = "monsterHealth";
    }

   void damage()
    {
        GameObject aaa = GameObject.Find("monsterHealth");

    }
}
