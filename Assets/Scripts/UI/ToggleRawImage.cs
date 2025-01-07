using UnityEngine;
using UnityEngine.UI;

public class ToggleRawImage : MonoBehaviour
{
    // La RawImage che vogliamo modificare
    public RawImage rawImage;

    // Le due texture da alternare
    public Texture initialTexture;
    public Texture alternateTexture;

    // Stato corrente
    private bool isUsingInitialTexture = true;

    // Funzione da attaccare al pulsante
    public void ToggleImage()
    {
        if (rawImage == null || initialTexture == null || alternateTexture == null)
        {
            Debug.LogWarning("Assicurati di aver assegnato RawImage e le texture nell'Inspector.");
            return;
        }

        // Cambia la texture
        if (isUsingInitialTexture)
        {
            rawImage.texture = alternateTexture;
        }
        else
        {
            rawImage.texture = initialTexture;
        }

        // Aggiorna lo stato
        isUsingInitialTexture = !isUsingInitialTexture;
    }
}
