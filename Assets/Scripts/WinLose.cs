using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLose : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public TextMeshProUGUI healthText;

    public GameObject winText;
    public GameObject loseText;

    public int count;
    public float health;


    // Start is called before the first frame update
    void Start()
    {
        SetCountText();
        SetHealthText();
        winText.SetActive(false);
        loseText.SetActive(false);

        count = gameObject.GetComponent<EnemyAi>().count;
        health = gameObject.GetComponent<PlayerController>().playerHealth;

    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 10)
        {
            winText.SetActive(true);
        }
    }

    void SetHealthText()
    {
        healthText.text = "Health: " + health.ToString();
        if (health <= 0)
        {
            loseText.SetActive(true);
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        // TODO
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
