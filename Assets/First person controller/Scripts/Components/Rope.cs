using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer lr;
    bool visible;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if(visible)
            lr.SetPosition(0, player.transform.position);
    }

    public void Show(Vector3 to)
    {
        lr.SetPosition(1, to);
        visible = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        visible = false;
        gameObject.SetActive(false);
    }
}
