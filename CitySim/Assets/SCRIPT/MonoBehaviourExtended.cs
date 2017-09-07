using UnityEngine;

static public class MonoBehaviourExtensionForMonoBehaviourTransform {

	static public T GetOrAddComponent<T> (this Component child) where T: Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
            result = child.gameObject.AddComponent<T>();
        return result;
    }
}
