﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class CelestialObject : MonoBehaviour
{
    public static List<GameObject> cellestialobjects = new List<GameObject>();
    private static string[] plidregistrar;
    private readonly float DEGREEACC = 1440f;

    public GameObject SOI;
    public float day;
    public float year;
    public double apoapsis;
    public double periapsis;
    public GameObject body;

    public IEnumerator DoDay(/*GameObject body, float day*/)
    {
        while (true)
        {
            if (StellarMovement.paused == true)
            {
                yield return null;
            }
            for (int _i = 0; _i <= DEGREEACC * day / StellarMovement.timescale; _i++)
            {
                body.transform.Rotate(new Vector3(0, 360/DEGREEACC), Space.Self);
                yield return new WaitForSeconds(day/DEGREEACC); //Delays the function appropriately for the degree of accuracy, day length and of course 360 degrees.
                //For example, Earth day is equal to 5 seconds and this coroutine needs to run 1440 times a second, so the delay would be roughly 3.4ms.
            }
        }
        yield return 1;

    }
   

    public void BuildCelestialObject(GameObject _body, GameObject _SOI, float _day, float _year, double _apoapsis, double _periapsis)
    {
        body = _body;
        print(body.name);
        cellestialobjects.Add(body); //Creates a cellestial object under the specified cellestial ID passed through to the function.
        print(_day);

        SOI = _SOI;
        year = _year;
        day = _day;
        apoapsis = _apoapsis;
        periapsis = _periapsis;
        StartCoroutine(DoDay());
    }

    private float[] CalcPosition(GameObject body, GameObject SOI, float day, float year, double apoapsis, double periapsis, float degree)
    {
        float _xoffset, _yoffset, _zoffset; //How much to offset on the X axis; will be passed to the movement function

        _xoffset = Mathf.Sin(degree);
        float percentapoapsis = (degree + 180) / 360;  //Finds how close the body is to 180--which will always be its apoapsis, unfortunately.
        float periapsialoffset = percentapoapsis * (float)(apoapsis - periapsis); //Multiplies that by the difference of the apoapsial radii and the periapsial radii then adds the periapsial radii.
        _yoffset = Mathf.Cos(degree) * ((float)periapsis + periapsialoffset);
        _zoffset = 0;
        float[] offset = {_xoffset, _yoffset, _zoffset};
        return offset;
    }
    
    
}
