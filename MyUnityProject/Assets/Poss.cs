using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss : MonoBehaviour
{
    public GameObject go;
    float duration = 1.4f;
    IK_Scorpion scorp;

    void Update()
    {
        if(scorp.animPlaying)
        {
            ChangeObjectYPos(transform, go.transform.position.y + 0.5f, duration);
        }
    }

    public static IEnumerator ChangeObjectYPos(Transform transform, float y_target, float duration)
    {
        float elapsed_time = 0; 

        Vector3 pos = transform.position; 

        float y_start = pos.y; 

        while (elapsed_time <= duration) 
        {
            pos.y = Mathf.Lerp(y_start, y_target, elapsed_time / duration); 

            transform.position = pos;

            yield return null; 

            elapsed_time += Time.deltaTime; 
        }
    }

}
