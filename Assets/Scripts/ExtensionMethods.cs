using UnityEngine;
using UnityEngine.EventSystems;

public static class ExtensionMethods {
    public static void AddListener(this EventTrigger trigger, EventTriggerType eventTriggerType, System.Action listener) {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventTriggerType;
        entry.callback.AddListener(data => listener());
        trigger.triggers.Add(entry);
    }

    public static void AddEventTriggerAndListener(this GameObject gameObject, EventTriggerType eventTriggerType, System.Action listener) {
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();

        if (!eventTrigger) {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        eventTrigger.AddListener(eventTriggerType, listener);
    }
}
