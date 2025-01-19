using System.Collections.Generic;
using UnityEngine;

public class WreckingBallWeight_Velocity : MonoBehaviour
{
    private Rigidbody rb;
    private Transform[] children;

    // Configurazione per i diversi tipi di teste
    [System.Serializable]
    public class HeadConfiguration
    {
        public float mass;
        public float drag;
        public float maxSpeed;
    }

    [Header("Configurazioni delle teste")]
    public HeadConfiguration normalHeadConfig;
    public HeadConfiguration spikeHeadConfig;
    public HeadConfiguration impactHeadConfig;

    private Dictionary<int, HeadConfiguration> headConfigs;

    private HeadConfiguration currentConfig;

    void Start()
    {
        // Ottieni il Rigidbody
        rb = GetComponent<Rigidbody>();

        // Ottieni tutti i figli
        int childCount = transform.childCount;
        children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        // Mappa configurazioni agli indici
        headConfigs = new Dictionary<int, HeadConfiguration>
        {
            { 0, normalHeadConfig },
            { 1, spikeHeadConfig },
            { 2, impactHeadConfig }
        };

        // Imposta la configurazione iniziale
        ApplyHeadConfiguration(normalHeadConfig);
    }

    // Metodo pubblico per aggiornare le proprietà del Rigidbody
    public void UpdateRigidbodyProperties(int activeChildIndex)
    {
        if (headConfigs.ContainsKey(activeChildIndex))
        {
            var config = headConfigs[activeChildIndex];
            ApplyHeadConfiguration(config);
        }
        else
        {
            Debug.LogWarning("Indice del figlio attivato non valido!");
        }
    }

    // Applica la configurazione al Rigidbody
    private void ApplyHeadConfiguration(HeadConfiguration config)
    {
        currentConfig = config; // Salva la configurazione attuale
        rb.mass = config.mass;
        rb.linearDamping = config.drag;
        LimitSpeed(config.maxSpeed);
    }

    // Metodo pubblico per ottenere la velocità massima attuale
    public float GetCurrentMaxSpeed()
    {
        return currentConfig != null ? currentConfig.maxSpeed : 0f;
    }

    // Limita la velocità del Rigidbody
    private void LimitSpeed(float maxSpeed)
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}
