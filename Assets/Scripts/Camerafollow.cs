using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Tähän raahataan pelaaja
    public float smoothing = 5f;    // Kuinka pehmeästi kamera seuraa
    public Vector3 offset;          // Kameran etäisyys pelaajasta

    void Start()
    {
        // Jos et aseta offsetia käsin, koodi laskee sen nykyisen etäisyyden mukaan
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate() // LateUpdate on paras kameralle, jotta hahmo ehtii liikkua ensin
    {
        if (target == null) return;

        // Lasketaan paikka, johon kameran pitäisi mennä
        Vector3 targetCamPos = target.position + offset;

        // Liikutetaan kameraa pehmeästi kohti kohdetta
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}