using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class DrawImage : MonoBehaviour
{

    public static GameObject HoverObj = null;//鼠标覆盖的GameObject
    private GameObject tempObj = null;//临时GameObject  用于下一个HoverObj的检测
    public Sprite[] mySprite;
    public Text text;
    private GameObject[,] myGameobject;//用于储存所有图片的数组
    private GameObject obj;//用于存储当前点击的图片
    private List<GameObject> list;//用于存储该被消除的图片
    private List<GameObject> checkList;//用于存储该被消除的提示图片
    private int times = 0;//当前已经寻找到的相同图片的个数
    private bool temp = false;//用于开启鼠标点击后的判断
    private bool stop = true;//处理动画逻辑时，暂停鼠标点击的判断功能
    private bool create = true;
    private float score = 0;//玩家分数
    private int number = 3;//最低消除的个数

    public GameObject imagePrefab = null;
    private float Screen_w = 0;//屏幕的宽
    private float Screen_h = 0;//屏幕的高
    private float pix_w = 120;//图片的宽
    private float pix_h = 120;//图片的高
    private float border_x = 0;//x轴边距
    private float border_y = 0;//y轴边距
    private List<GameObject> recycle = new List<GameObject>();

    public Text countDown;
    public float countDownTime;
    private bool countDownStart;
    private bool canTouch=true;

    public GameObject heroesPrefab = null;
    private GameObject[] heroesObject;
    public Sprite[] heroesSprite;

    private GameObject[] monsterObject;
    public GameObject monsterPrefab = null;
    public Sprite[] monsterSprite;
    private GameObject findMonster;
    private bool isCollect;
    private List<string> getBlockName = new List<string>();
    private List<int> getBlockNum = new List<int>();
    private bool isAttack;
    private bool touchHeroes;
    private bool touchMonster;
    private GameObject pickHeroes;
    public Material clickHeroesMaterial;
    private bool isClickHeroesEffect;
    private float shineLocation=0, shineWidth=0;
    private GameObject pickMonster;
    public GameObject targetMonster;

    void Start()
    {
        list = new List<GameObject>();
        DOTween.Init();
        CreateImage();
        createHeroes();
        createMonster();
        touchHeroes = true;
        clickHeroesMaterial.SetFloat("_ShineWidth", 0);
        clickHeroesMaterial.SetFloat("_ShineLocation", 0);
    }
    
    void Update()
    {
        if (isClickHeroesEffect )
        {
            clickHeroesEffect();
        }

        if (touchHeroes == true) //选择英雄攻击
        {
            if (Input.GetMouseButtonDown(0))
            {
                pickHeroes = EventSystem.current.currentSelectedGameObject;
                if (pickHeroes!=null && pickHeroes.tag == "heroes")
                {
                    pickHeroes.GetComponent<Image>().material = clickHeroesMaterial;
                    isClickHeroesEffect = true;
                    touchHeroes = false;
                    touchMonster = true;
                }
            }
        }

        if (touchMonster)
        {
            if (Input.GetMouseButtonDown(0))
            {
                 pickMonster = EventSystem.current.currentSelectedGameObject;
                  if (pickMonster.tag == "monster")
                    {
                    var targetObject =GameObject. Instantiate(targetMonster, Vector3.zero, Quaternion.identity)as GameObject;
                    targetObject.transform.SetParent(pickMonster.transform);
                    targetObject.transform.localScale = Vector3.one;
                    targetObject.transform.localPosition = new Vector2(0,0);
                    //  targetObject.GetComponent<RectTransform>().
                    touchMonster = false;
                    }
            }
        }

        
            if (countDownStart) //点击后进行倒数
            {
                if (countDownTime > 1)
                {
                    GameObject.Find("time").GetComponent<RectTransform>().localPosition = new Vector2(0, GameObject.Find("time").GetComponent<RectTransform>().localPosition.y);
                    countDownTime -= Time.deltaTime;
                    countDown.text = Mathf.Floor(countDownTime).ToString();
                }
                else
                {
                GameObject.Find("time").GetComponent<RectTransform>().localPosition = new Vector2(630, GameObject.Find("time").GetComponent<RectTransform>().localPosition.y);
                countDown.text = "";
                    canTouch = false;
                    //b.turn();
                    //if(isCollect== false )collectBall();
                    isAttack = true;
                    attackMonster();
                }

            }
        if (touchHeroes == false && touchMonster == false)
        {
            if (stop)
            {
                if (canTouch)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        obj = EventSystem.current.currentSelectedGameObject;
                        if (obj != null && obj.tag == "ball") //点击是不是珠子
                        {
                            countDownStart = true;
                            times++;
                            temp = true;
                            tempObj = obj;
                            list.Add(obj);
                        }
                    }
                }
                else //时间到了后将连接的3珠子删掉
                {
                    temp = false;
                    State(temp);
                    if (times >= number)
                    {
                        isCollect = true;
                        stop = false;
                        collectBall();
                        Remove();
                        text.text = string.Format("Scores : {0:n1}", ChangeScore(times));
                    }
                    else
                    {
                        for (int i = 0; i < recycle.Count; i++)
                        {
                            //GameObject.Destroy(recycle[i]);
                        }
                        times = 0;
                        list.Clear();
                        tempObj = null;
                    }
                }
                if (temp)
                {
                    if (HoverObj != tempObj)
                    {
                        if (Check())
                        {
                            times++;
                            tempObj = HoverObj;
                            list.Add(HoverObj);
                            create = true;
                            State(temp);
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        temp = false;
                        State(temp);
                        if (times >= number)
                        {
                            isCollect = true;
                            stop = false;
                            collectBall();
                            Remove();
                            text.text = string.Format("Scores : {0:n1}", ChangeScore(times));
                        }
                        else
                        {
                            for (int i = 0; i < recycle.Count; i++)
                            {

                                GameObject.Destroy(recycle[i]);//删除未达成3珠子删除的箭头
                            }
                            times = 0;
                            list.Clear();
                            tempObj = null;
                        }

                    }
                    State(temp);
                }
            }
        
        }

        //JudgementDie(number);
    }

    //void JudgementDie(int number)
    //{
    //    for (int i = 0; i < myGameobject.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < myGameobject.GetLength(1); j++)
    //        {
    //            if (i - 1 >= 0 && j - 1 >= 0)
    //            {
    //                checkList.Add(myGameobject[i, j]);
    //                if (Judgement(i - 1, j - 1, myGameobject[i, j]))
    //                {
    //                    number--;
    //                    if (number > 1)
    //                    {
    //                        JudgementDie(number--);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //}

    //bool Judgement(int x,int y,GameObject obj)
    //{
    //    for (int i = 0; i < checkList.Count; i++)
    //    {
    //        if (myGameobject[x, y].name == obj.name&&myGameobject[x,y]!= checkList[i])
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    float ChangeScore(int times)
    {
        score += (100 + (times - 2) * 50) * times;
        return score;
    }


    void State(bool flag) //创建箭头
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (flag)
            {
                Color mycolor = new Color();
                ColorUtility.TryParseHtmlString("#FF976B", out mycolor);
                list[i].GetComponent<Image>().color = mycolor; //new Color32(255,102,102,255); //点击后的颜色
                if (i == list.Count-1&& i > 0&&create)
                {
                    GameObject tmp = GameObject.Instantiate(imagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    tmp.transform.SetParent(list[i].transform.parent);
                    tmp.transform.localScale = Vector3.one;

                    tmp.transform.position = list[i].transform.position-(list[i].transform.position - list[i - 1].transform.position)/2;

                    int x_1 = 0;
                    int x_2 = 0;
                    int y_1 = 0;
                    int y_2 = 0;
                    for (int x = 0; x < myGameobject.GetLength(0); x++)
                    {
                        for (int y = 0; y < myGameobject.GetLength(1); y++)
                        {
                            if (list[i].Equals(myGameobject[x, y]))
                            {
                                x_1 = x;
                                y_1 = y;
                            }
                            if (list[i - 1].Equals(myGameobject[x, y]))
                            {
                                x_2 = x;
                                y_2 = y;
                            }
                        }
                    }

                    tmp.transform.Rotate(0, 0, Roate(y_1 - y_2, x_2 - x_1));
                    tmp.transform.name = "Image" + i;
                    recycle.Add(tmp);
                    create = false;
                }
            }
            else
            {
                list[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    int Roate(int temp1,int temp2)
    {
        if (temp1 ==1 && temp2 ==1)
        {
            return 45;
        }
        if(temp1 == 0 && temp2 == 1)
        {
            return 90;
        }
        if (temp1 == -1 && temp2 == 1)
        {
            return 135;
        }
        if (temp1 == -1 && temp2 == 0)
        {
            return 180;
        }
        if (temp1 == -1 && temp2 == -1)
        {
            return 225;
        }
        if (temp1 == 0 && temp2 == -1)
        {
            return 270;
        }
        if (temp1 == 1 && temp2 == -1)
        {
            return 315;
        }
        return 0;
    }

    void Remove() //能连接后进行删除
    {
        Tweener tweener;
        int count = 0;
        for (int i = 0; i < recycle.Count; i++)//删除箭头
        {
           
            GameObject.Destroy(recycle[i]);
           
        }
        for (int i = 0; i < list.Count; i++)
        {
            tweener = list[i].GetComponent<Image>().rectTransform.DOScale(Vector3.zero, 0.2f);//删除的时间(background)
            tweener.SetUpdate(true);

            tweener.SetEase(Ease.OutSine);

            list[i].GetComponent<Image>().sprite = mySprite[8];//删除的珠子暂时变成其他的

            tweener.onComplete = delegate()
            {
                count++;
                if (count == list.Count)
                {
                    test();
                }
            };
        }
    }

    bool Limit(Image image1, Image image2) // 2 sprite 
    {
        int x1 = 0;
        int y1 = 0;
        int x2 = 0;
        int y2 = 0;
        for (int i = 0; i < myGameobject.GetLength(0); i++)
        {
            for (int j = 0; j < myGameobject.GetLength(1); j++)
            {
                if (image1.Equals((myGameobject[i, j].GetComponent<Image>())))
                {
                    x1 = i;
                    y1 = j;
                }
                if (image2.Equals((myGameobject[i, j].GetComponent<Image>())))
                {
                    x2 = i;
                    y2 = j;
                }
            }
        }
        if (Mathf.Abs(x1 - x2) <= 1 && Mathf.Abs(y1 - y2) <= 1)
        {
            return true;
        }
        return false; 
    }

    bool Check()
    {
        GameObject g = HoverObj;
        if (!Limit(g.GetComponent<Image>(), tempObj.GetComponent<Image>())) // 连接后再指向范围外
        {
            Debug.Log(g.name + " and " + obj.name);
            return false;
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name.Equals(g.name))
            {
                if (i >= 0 && i == list.Count - 2)
                {
                    list[i+1].GetComponent<Image>().color = Color.white;
                    var image = recycle[recycle.Count - 1];
                    recycle.Remove(image);
                    GameObject.Destroy(image);//撤销
                    Debug.Log("destroy 299");
                    list.Remove(list[i + 1]);
                    times--;
                    tempObj = HoverObj;
                    State(temp);
                }
                return false;
            }
            else
            {
                
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (!g.GetComponent<Image>().sprite.name.Equals(list[i].GetComponent<Image>().sprite.name))
            {
                return false;
            }
        }
        obj = g;
        return true;
    }


    void test() //对珠子的删除和变换颜色， 位置
    {
        int count = 0;
        int temp = 0;
        bool flag = false;
        for (int i = 0; i < myGameobject.GetLength(1); i++) //0++<6  y
        {
            for (int j = myGameobject.GetLength(0) - 1; j >= 0; j--) //j--=4>0 x
            {
                if (myGameobject[j, i].GetComponent<Image>().sprite.name.StartsWith("Bg"))//4,0
                {
                    flag = true;
                    if (j >= 1)
                    {
                        count++;
                        GameObject obj1 = myGameobject[j, i];//被消除的A对象
                        GameObject obj2 = myGameobject[j - 1, i];//A对象上面的一个B对象 3,6
                        Vector3 position1 = obj1.transform.position;//B对象的位置
                        Vector3 position2 = obj2.transform.position;
                        Tweener tweener = myGameobject[j - 1, i].GetComponent<RectTransform>().DOMove(position1, 0.1f);//掉落的时间
                        tweener.SetEase(Ease.Linear);
                        //tweener.SetUpdate(true);
                        myGameobject[j, i].transform.position = position2;
                        tweener.onComplete = delegate()
                        {
                            temp++;
                            if (temp == count)
                            {
                                if (flag)
                                {
                                    test();
                                }
                            }
                        };

                        myGameobject[j, i] = obj2;
                        myGameobject[j - 1, i] = obj1;//调换A、B对象之间的位置  完成下落                            
                    }
                    else
                    {
                        Tweener tweener = myGameobject[j, i].GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f);//变出珠子的时间
                        myGameobject[j, i].GetComponent<Image>().sprite = mySprite[Random.Range(0,6)];//删除后的珠子随机颜色
                        tweener.onComplete = delegate()
                        {
                            if (count == 0)
                            {
                                list.Clear();
                                tempObj = null;
                                stop = true;
                                times = 0;
                            }
                        };
                    }
                }
            }

        }
        if (!flag)
        {
            list.Clear();
            tempObj = null;
            stop = true;
            times = 0;
        }
    }

    void CreateImage()
    {
        Screen_w = this.GetComponent<RectTransform>().rect.width;
        Screen_h = this.GetComponent<RectTransform>().rect.height;
        border_x = (Screen_w - (pix_w) * 6)/2;
        border_y = ((Screen_h - (pix_h) * 5)/2)+10;//8
        myGameobject = new GameObject[5, 6];
        int num = 0;
        for (int i = 0; i < myGameobject.GetLength(0); i++)
        {
            for (int j = 0; j < myGameobject.GetLength(1); j++)
            {
                Vector3 position;
                position = new Vector3(border_x + (pix_w)*j +pix_w/2 - Screen_w/2,   -border_y-(pix_h)*i - pix_h/2 + Screen_h/2,   -1);
              //  position = new Vector3(j*90,i*-90,0);z
                var tmp = GameObject.Instantiate(imagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                myGameobject[i, j] = tmp;
                myGameobject[i, j].transform.SetParent(transform);
                tmp.transform.localScale = Vector3.one;
                tmp.transform.localPosition = position;
                myGameobject[i, j].name = "Pic_" + num;
                num++;
                Image image = myGameobject[i, j].GetComponent<Image>();
                image.sprite = mySprite[Random.Range(0, 6)];
                myGameobject[i, j].AddComponent<Pic>();
                myGameobject[i, j].AddComponent<Button>();
            }
        }
    }

    void createHeroes() 
    {
        heroesObject = new GameObject[6];
        int num = 0;
        for (int i = 0;i < 6; i++)
        {
            Vector3 position;
            position = new Vector3(i * 120-300, 420, 0);
            var tmp = GameObject.Instantiate(heroesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            heroesObject[i] = tmp;
            heroesObject[i].transform.SetParent(transform);
            tmp.transform.localScale = Vector3.one;
            tmp.transform.localPosition = position;
            heroesObject[i].name = "heroes_" + num;
            num++;
            Image image = heroesObject[i].GetComponent<Image>();
            image.sprite = heroesSprite[0];
            heroesObject[i].AddComponent<Pic>();
            heroesObject[i].AddComponent<Button>();
        }
    }

    void createMonster()
    {
        monsterObject = new GameObject[1];
        int num = 0;
        for (int i = 0; i < 1; i++)
        {
            Vector3 position;
            position = new Vector3(i * 120 , 800, 0);
            var tmp = GameObject.Instantiate(monsterPrefab,Vector3.zero,Quaternion.identity) as GameObject;
            monsterObject[i] = tmp;
            monsterObject[i].transform.SetParent(transform);
            tmp.transform.localScale = Vector3.one;
            tmp.transform.localPosition = position;
            monsterObject[i].name = "monster_" + num;
            num++;
            Image image = monsterObject[i].GetComponent<Image>();
            image.sprite = monsterSprite[0];
            monsterObject[i].AddComponent<Pic>();
            monsterObject[i].AddComponent<Button>();
        }
    }

    void attackMonster()
    {
        if (isAttack)
        {
            float totalDamage=0;
            float damage = 0;
            float currentHealth = 0;
            float ratio;
            float width;
            GameObject aaa = GameObject.Find("monsterHealth");
            RectTransform bbb = aaa.GetComponent<RectTransform>();
            width = bbb.sizeDelta.x;
            Debug.Log(width);
            for (int i = 0; i < getBlockName.Count; i++)
            {
                totalDamage += (getBlockNum[i] * 5);
            }
            currentHealth = totalDamage / 100;
            ratio = currentHealth * width;
            bbb.sizeDelta = new Vector3(100-ratio, bbb.sizeDelta.y,0);
            Debug.Log(totalDamage);
        }
        countDownStart = false;
        isAttack = false;
        
        //Debug.Log(aaa.transform.position.x);
        getBlockName.Clear();
        getBlockNum.Clear();
        
    }
    
    void collectBall()
    {
        if (isCollect)
        {
            getBlockName.Add(obj.GetComponent<Image>().sprite.ToString());
            getBlockNum.Add(list.Count);
        }
        isCollect = false;
    }

    void clickHeroesEffect()
    {
        if (shineLocation <= 1)
        {
            shineWidth += 0.1f;
            shineLocation += 0.1f;
            clickHeroesMaterial.SetFloat("_ShineWidth", shineWidth);
            clickHeroesMaterial.SetFloat("_ShineLocation", shineLocation);
        }
        else
        {
            if (shineWidth >= 0.5f)
            {
                shineWidth -= 0.1f;
                clickHeroesMaterial.SetFloat("_ShineWidth", shineWidth);
            }
           
        }

    }
}
