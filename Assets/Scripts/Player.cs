using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float dodgeSpeed;
    public float maxX;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();           
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            GameManager.instance.DisplayEndMenu();
            Destroy(gameObject);
        }

        if (col.gameObject.tag == "NotEnemy")
        {
            this.gameObject.GetComponent<Renderer>()
                .sharedMaterial.SetColor("_Color", GameManager.colorObj);
        }
    }
        

    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, 3, 3);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        objPosition.x = Mathf.Clamp(objPosition.x, -maxX, maxX);
        objPosition.y = (float)0.3;
        objPosition.z = (float)-7.7;


        transform.position = objPosition;
    }
}
