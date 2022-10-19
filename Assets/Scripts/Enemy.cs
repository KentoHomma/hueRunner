using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public static Enemy instance;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

        if(transform.position.z < -10f)
        {
            GameManager.instance.ScoreUp();
            GameManager.instance.LevelManage();
            Destroy(gameObject);
        }
    }
}
