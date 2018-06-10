using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{

    public Transform player;

    Vector3 playerPos, v;
    float mousePosX, mousePosY;

    private void Start()
    {
        player = Player.thisPlayer.transform;    
    }

    void LateUpdate()
    {
        mousePosX -= Input.GetAxis("Mouse X") * 0.25f;
        mousePosY = Mathf.Clamp(mousePosY - Input.GetAxis("Mouse Y") * 0.25f, -2, 2);

        float mousePosYToo = mousePosY;
        if (mousePosYToo > 0)
            mousePosYToo *= -1;

        Vector3 toPos = new Vector3(playerPos.x + Mathf.Cos(mousePosX) * (4 + mousePosYToo), playerPos.y + 1 + mousePosY, playerPos.z + Mathf.Sin(mousePosX) * (4 + mousePosYToo));

        transform.position = Vector3.SmoothDamp(transform.position, toPos, ref v, 0.1f);

        playerPos = player.transform.position;
        playerPos.y += 1;

        transform.LookAt(playerPos);
    }

}