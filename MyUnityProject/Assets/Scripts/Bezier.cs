using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    //public
    [Range(2, 20)]
    public float distanciaMax = 5f;
    [Range(1, 5)]
    public float alturaControll = 3f;
    [Range(0, 20)]
    public float descensoMax = 10f;

    //constantes
    private const int NUMERO_DE_ESFERAS = 20;

    //private
    private Vector3 _puntoFinal, _control;
    private Vector3[] _points;
    private LineRenderer _lineR;

    void Start()
    {
        _lineR = GetComponent<LineRenderer>();
        _lineR.positionCount = NUMERO_DE_ESFERAS;

        _points = new Vector3[NUMERO_DE_ESFERAS];

        /* for (int i = 0; i < NUMERO_DE_ESFERAS; i++)
            {
                esferas[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                esferas[i].transform.localScale = Vector3.one * 0.1f;
            }
        */
    }

    void Update()
    {
        CalculateCurve();
    }

    private void CalculateCurve()
    {
        for (int i = 0; i < NUMERO_DE_ESFERAS; i++)
        {
            _puntoFinal = transform.position + transform.forward * distanciaMax;

            _control = transform.position + (_puntoFinal - transform.position) / 2;

            _control.y += alturaControll;

            _puntoFinal.y = transform.position.y - descensoMax;

            float t = (float)i / (float)(NUMERO_DE_ESFERAS - 1);

            _points[i] = CalculateBezierPoint(t, transform.position, _control, _puntoFinal);
        }
        _lineR.SetPositions(_points);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        Vector3 point = u * u * p0;
        point += 2 * t * u * p1;
        point += t * t * p2;

        return point;
    }
}

