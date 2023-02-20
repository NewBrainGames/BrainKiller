using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pick_Up : MonoBehaviour
{
    public float rotationSpeed = 2;
    public AudioSource pickUpAudio;
    private static int coinNumber = 0;
    public bool isPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        pickUpAudio = this.gameObject.GetComponent<AudioSource>();
        
    }

    
    void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            PlayPickUpAudio();
            if (isPlayed)
            {
                isPlayed = false;
                Invoke("Destroy", 1.0f);

            }

            if (col.gameObject.tag == "Player" && this.gameObject.tag == "Coin")
            {
                Destroy(this.gameObject);
                coinNumber++;
                // GameObject.Find("CoinNumber").GetComponent<Text>().text = (coinNumber).ToString();
            }
        }

    }

    private void PlayPickUpAudio()
    {
        if (pickUpAudio)
        {
            pickUpAudio.Play();
            isPlayed = true;
            
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }


}
