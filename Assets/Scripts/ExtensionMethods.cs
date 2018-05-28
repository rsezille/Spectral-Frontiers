using UnityEngine;
using UnityEngine.EventSystems;

public static class ExtensionMethods {
    public static void AddListener(this GameObject gameObject, EventTriggerType eventTriggerType, System.Action listener) {
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();

        if (!eventTrigger) {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventTriggerType;
        entry.callback.AddListener(data => listener());
        eventTrigger.triggers.Add(entry);
    }
}
