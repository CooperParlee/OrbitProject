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
        float projrot = 360 / year * DEGREEACC;
        float projdel = projrot / 360 * year;

        while (true)
        {
            for(int _i = 0; _i <= year / DEGREEACC / StellarMovement.timescale; _i++)
            {
                if (StellarMovement.paused == true)
                {
                    _i -= 1;
                    break;
                }
                Vector3 newpos;
                float apdifferential = (float)(apoapsis - periapsis);
                double _x, _y = 0, _z;

                float currot = _i * projrot;
                _x = Mathf.Sin(currot/90) * apdifferential * 0.5f + periapsis;
                _z = Mathf.Cos(currot/90) * apdifferential * 0.5f + periapsis;

                newpos = new Vector3((float)_x + SOI.transform.position.x, (float)_y + SOI.transform.position.y, (float)_z + SOI.transform.position.z);
                body.transform.position = newpos;
                yield return new WaitForSeconds(projdel);
            }
            yield return new WaitForSeconds(projdel);
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
        StartCoroutine(DoOrbit());
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
