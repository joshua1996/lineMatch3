using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Pic : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventTrigger trigger = this.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = this.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers = new List<EventTrigger.Entry>();

        // 定义需要绑定的事件类型。并设置回调函数
        EventTrigger.Entry entry = new EventTrigger.Entry();
        // 设置 事件类型
        entry.eventID = EventTriggerType.PointerEnter;
        // 设置回调函数
        UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(SendMsg);
        entry.callback.AddListener(callback);

        // 添加事件触发记录到GameObject的事件触发组件
        trigger.triggers.Add(entry);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SendMsg(BaseEventData arg0)
    {
        
        if (this.gameObject.name != "Panel")
        {
            DrawImage.HoverObj = this.gameObject;
        }
    }
}
