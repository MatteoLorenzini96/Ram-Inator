using UnityEngine;
using UnityEngine.UI;

public class ReplaceChildWithPrefab : MonoBehaviour
{
    [System.Serializable]
    public class UIObjectPrefabPair
    {
        public Button uiButton; // Pulsante UI associato
        public GameObject prefab; // Prefab da sostituire
    }

    public Transform arieteHead; // Posizione "ArieteHead"
    public UIObjectPrefabPair[] uiObjectPrefabPairs; // Lista di pulsanti UI e prefab associati

    private void Start()
    {
        // Associa un listener a ogni pulsante nella lista
        foreach (var pair in uiObjectPrefabPairs)
        {
            if (pair.uiButton != null)
            {
                pair.uiButton.onClick.AddListener(() => ReplaceChild(pair.prefab));
            }
        }
    }

    private void ReplaceChild(GameObject prefab)
    {
        if (arieteHead == null)
        {
            Debug.LogError("ArieteHead non è assegnato!");
            return;
        }

        // Rimuove tutti i figli dall'oggetto ArieteHead
        foreach (Transform child in arieteHead)
        {
            Destroy(child.gameObject);
        }

        // Istanzia il nuovo prefab come figlio di ArieteHead
        if (prefab != null)
        {
            GameObject newChild = Instantiate(prefab, arieteHead);
            newChild.transform.localPosition = Vector3.zero; // Imposta la posizione locale a (0, 0, 0)
            newChild.transform.localRotation = Quaternion.identity; // Imposta la rotazione locale a (0, 0, 0)
        }
        else
        {
            Debug.LogError("Prefab non assegnato!");
        }
    }
}
