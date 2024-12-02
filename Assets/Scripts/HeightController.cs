using UnityEngine;

public class HeightController : MonoBehaviour
{
    public Transform pivot; // L'oggetto che rappresenta il pivot (genitore dei punti di attacco)
    public Transform ariete; // L'oggetto ariete
    public HingeJoint joint1;
    public HingeJoint joint2;

    void Update()
    {
        // Modifica la posizione del pivot per cambiare la lunghezza
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("UpArrow premuto");
            pivot.position += Vector3.up * Time.deltaTime; // Alza il pivot (accorcia il pendolo)
            Debug.Log("accorcia il pendolo");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("DownArrow premuto");
            pivot.position -= Vector3.up * Time.deltaTime; // Abbassa il pivot (allunga il pendolo)
            Debug.Log("allunga il pendolo");
        }

        // Aggiorna i Connected Anchor per allineare la fisica
        joint1.connectedAnchor = ariete.position;
        joint2.connectedAnchor = ariete.position;
    }
}
