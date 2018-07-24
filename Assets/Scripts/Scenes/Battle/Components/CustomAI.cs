using System.Collections;
using UnityEngine;

/**
 * CustomAI will only be called if the main component has an AI component
 * 
 * How to use:
 * - Implement ICustomAI in Goblin.cs for example
 * - Add dynamically this component in Awake (customAI = gameObject.AddComponent<CustomAI>() as CustomAI;)
 * - Right after, call Use(this) in Goblin.cs
 */
[RequireComponent(typeof(AI))]
public class CustomAI : MonoBehaviour {
    ICustomAI iCustomAI;

    public void Use(ICustomAI iCustomAI) {
        this.iCustomAI = iCustomAI;
    }

    public IEnumerator Process() {
        if (iCustomAI != null) {
            yield return iCustomAI.ProcessAI();
        }

        yield return null;
    }
}
