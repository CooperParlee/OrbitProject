using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class CelestialObject : MonoBehaviour
{
    public static List<GameObject> cellestialobjects = new List<GameObject>();
    private static string[] plidregistrar;
    private readonly float DEGREEACC = 0.025f;

    public GameObject SOI;
    public float day;
    public float year;
    public double apoapsis;
    public double periapsis;
    public GameObject body;

    public IEnumerator DoDay(/*GameObject body, float day*/)
    {
        float projrot = 360 / day * DEGREEACC;
        float projdel = projrot / 360 * day;
        print("Projected rotation: " + projrot);
        print("Projected delay: " + projdel);
        while (true)
        {
            if (StellarMovement.paused == true)
            {
                yield return null;
            }
            for (int _i = 0; _i <= day / DEGREEACC * StellarMovement.timescale; _i++) // Checked
            {
                if (StellarMovement.paused == true)
                {
                    _i -= 1;
                    break;
                }
                
                body.transform.Rotate(new Vector3(0, projrot), Space.Self);
                yield return new WaitForSeconds(projdel); //Delays the function appropriately for the degree of accuracy, day length and of course 360 degrees.
                //For example, Earth day is equal to 5 seconds and this coroutine needs to run 1440 times a second, so the delay would be roughly 3.4ms.
            }
        }
        

    }
    public IEnumerator DoOrbit()
    {
        while (true)
        {
            for(int _i = 0; _i <= DEGREEACC * year / StellarMovement.timescale; _i++)
            {
                if (StellarMovement.paused == true)
                {
                    _i -= 1;
                    break;
                }
                print("Orbit degree reported: " + _i / 4 * StellarMovement.timescale);
                Vector3 move = CalcPosition(body, SOI, day, year, apoapsis, periapsis, _i/4*StellarMovement.timescale, 0);

                //body.transform.position = SOI.transform.position + move;
                yield return new WaitForSeconds(360/DEGREEACC/90);
            }
            yield return new WaitForSeconds(year / DEGREEACC);
        }
    }

    public void BuildCelestialObject(GameObject _body, GameObject _SOI, float _day, float _year, double _apoapsis, double _periapsis)
    {
        body = _body;
        cellestialobjects.Add(body); //Creates a cellestial object under the specified cellestial ID passed through to the function.
        SOI = _SOI;
        year = _year;
        day = _day;
        apoapsis = _apoapsis;
        periapsis = _periapsis;
        StartCoroutine(DoDay());
        //StartCoroutine(DoOrbit());
    }

    private static Vector3 CalcPosition(GameObject body, GameObject SOI, float day, float year, double apoapsis, double periapsis, float degree, float degreeofnodeascension)
    {
        float _xoffset, _yoffset = 0, _zoffset; //How much to offset on the X, Y, Z axis; will be passed to the movement function
        float apdifferential =  (float)(apoapsis - periapsis);
        _xoffset = Mathf.Sin(degree/90) * apdifferential * (float) 0.5 + (float) periapsis;

        _zoffset = Mathf.Cos(degree/90) * (apdifferential) + (float)periapsis;
        Vector3 offset = new Vector3(_xoffset, _yoffset, _zoffset);
        return offset;
    }
    
    
}
