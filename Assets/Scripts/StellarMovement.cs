using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//@Author: Cooper Parlee; designed 2018 for year 8 physical science
//I don't annotate, sorry :P

//Never mind that. Smart 9th-grade Cooper came in and fixed that. :P

//For my reference; 1au = 10 units of distance
//
//Time scale adjustable. How many seconds it takes to simulate: 24 hours (base 1000ms)
public class StellarMovement : MonoBehaviour
{
    public static float timescale = 10f;

    private readonly float EARTHDAYPROPORTIONAL = 5f;
    public readonly float ORBITDEGREES = 360f;
    public readonly float SUNDAYPROPORTIONAL = 26.6f;
    public readonly float MOONORBITPROPORTIONAL = -25.322f;
    public readonly float EARTHORBITPROPORTIONAL = 365.25f;
    public readonly float MOONDAYPROPORTIONAL = 27f;

    //The actual high and low points of the Earth's orbit.
    public readonly float EARTHAPOAPSIS = 101.67f;
    public readonly float EARTHPERIAPSIS = 0.98328f;

    //The apoapsis of the Moon's orbit.
    public readonly float MOONAPOAPSIS = 0.257003846f;
    public readonly float MOONPERIAPSIS = 0.257003846f;

    public readonly List<string> GUIOPTIONS = new List<string> { "Complete view", "Track moon", "Lunar eclipse view", "Solar eclipse view", "Top-down", "Track sun", "Follow earth" };

    //Object references.
    public GameObject earthObj;
    public GameObject sunObj;
    public GameObject moonObj;
    public GameObject cam;
    public Button button;
    public Text daygui;
    public Dropdown dropdown;
    public Toggle toggle;
    public Text slidertext;

    public static bool paused = false;
    public bool stoporbitalprecision;
    protected internal float days = 0;
    public static bool timer;
    protected internal Vector3 Completecamposition;
    public readonly Vector3 SOLARECLIPSEVIEW;
    protected internal Quaternion Completecamrotation;
    readonly Vector3 TOPDOWNCAMPOSITION = new Vector3(0, 19.8f, 0);
    readonly Vector3 TOPDOWNCAMROT = new Vector3(90, 0, 0);
    //public readonly Vector3 SOLARECLIPSEROTATION = new Vector3();
    protected internal float currentearthdegree;
    protected internal float currentmoondegree;
    public readonly float MOONWOBBLER = 0.601291397268118083069041844924f; // Distance/degree proportional to distance/degree; size proportional to size
    protected internal double moonheight;
    public readonly double MOONAPSIDALPRECISION = 3232.6054f;
    protected internal float floatheight;
    float lunarxdifferential;
    float lunarydifferential;
    protected internal readonly Vector3 FOLLOWDISTANCE = new Vector3(5, 0, 0);
    protected internal readonly Vector3 FOLLOWROTATIONEULER = new Vector3(0, 90, 0);
    protected internal bool vis = true;

    StarSystem sol;
    CelestialObject earth;
    CelestialObject moon;
    // Use this for initialization
    void Start()
    {
        Completecamposition = cam.transform.position;
        Completecamrotation = cam.transform.rotation;

        StarSystem sol = gameObject.AddComponent(typeof(StarSystem)) as StarSystem;
        CelestialObject earth = earthObj.AddComponent<CelestialObject>() as CelestialObject;
        //StarSystem sol = new StarSystem(sunObj);
        //CelestialObject earth = new CelestialObject(earthObj, sunObj, EARTHDAYPROPORTIONAL, EARTHORBITPROPORTIONAL, EARTHAPOAPSIS, EARTHPERIAPSIS);
        CelestialObject moon = new CelestialObject(moonObj, earthObj, MOONDAYPROPORTIONAL, MOONORBITPROPORTIONAL, MOONAPOAPSIS, MOONPERIAPSIS);

        /*
        StartCoroutine(DoEarthRotation());
        StartCoroutine(DoSunRotation());
        StartCoroutine(DoLunar());
        StartCoroutine(DoEarthOrbit());
        StartCoroutine(DoEcliptic());
        */
    }

    // Update is called once per frame
    void Update()
    {
        daygui.text = "Day: " + days.ToString();
        dropdown.ClearOptions();
        dropdown.AddOptions(GUIOPTIONS);
    }
    public void OnValueChanged()
    {
        StartCoroutine(ToggleCam());
    }
    public void TogglePause()
    {
        string addtext = "false";
        switch (paused)
        {
            
            case false:
                paused = true;

                 addtext = "true";
            break;
            case true:
                paused = false;

                addtext = "false";
            break;
        }
        button.GetComponentInChildren<Text>().text = "Paused: " + addtext;
    }
    public void ChangeScale(float value)
    {
        timescale = value;
        slidertext.GetComponentInChildren<Text>().text = timescale + "x";
    }
    public void TogglePresession()
    {
        stoporbitalprecision = toggle.isOn;
        print(stoporbitalprecision);
    }
    public void ToggleVis()
    {
        print(vis);
        if (vis == false)
        {
            vis = true;
            moonObj.GetComponentInChildren<Renderer>().receiveShadows = true;
            
        }
        else
            if (vis == true)
        {
            vis = false;
            moonObj.GetComponentInChildren<Renderer>().receiveShadows = false;
            print("1");
        }

        print("Changed " + vis);
    }
    /*
    IEnumerator DoEarthOrbit()
    {
        while (true)
        {
            for(int x = 0; x <=DEGREEACC/timescale*90; x++)
            {
                yield return new WaitForSeconds(Mathf.Abs(EARTHORBITPROPORTIONAL / DEGREEACC) / timescale);
                while (paused)
                {
                    x = x - 1;
                    yield return null;
                }
                print("why are you running?");
                currentearthdegree = -x / DEGREEACC * ORBITDEGREES;
                float ydifferential = Mathf.Sin((currentearthdegree/90)) * EARTHORBITRADII;
                float xdifferential = Mathf.Cos((currentearthdegree/90)) * -EARTHORBITRADII;
                earth.transform.position = sun.transform.position + new Vector3(xdifferential, 0, ydifferential);                
            }
        }
    }
    */
    IEnumerator ToggleCam()
    {
        while (true)
        {
            switch (dropdown.value)
            {
                case 0:
                    //position and rotation
                    //float degree = Mathf.Atan((lunarxdifferential/MOONORBITRADII) / (lunarydifferential/MOONORBITRADII))*90;
                    //position and rotation
                    cam.transform.position = Completecamposition;
                    cam.transform.rotation = Completecamrotation;
                    yield break;
                case 1:

                    cam.transform.position = earthObj.transform.position;
                    cam.transform.LookAt(moonObj.transform.position);
                    break;
                case 2:
                    //position and rotation
                    cam.transform.position = earthObj.transform.position;
                    cam.transform.LookAt(moonObj.transform.position);

                    break;
                case 3:
                    //position and rotation
                    cam.transform.position = earthObj.transform.position;
                    cam.transform.LookAt(sunObj.transform.position);

                    break;
                case 4:
                    //position and rotation
                    cam.transform.position = TOPDOWNCAMPOSITION;
                    cam.transform.eulerAngles = TOPDOWNCAMROT;
                    break;
                case 5:
                    cam.transform.position = earthObj.transform.position;
                    cam.transform.LookAt(sunObj.transform.position);
                    break;
                case 6:
                    cam.transform.eulerAngles = FOLLOWROTATIONEULER;
                    cam.transform.position = earthObj.transform.position - FOLLOWDISTANCE;
                    break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    /*
    IEnumerator DoEarthRotation()
    {
        while (true) { 
            for (int x = 0; x <= DEGREEACC/timescale/36; x++)
            {
                yield return new WaitForSeconds(EARTHDAYPROPORTIONAL / DEGREEACC * 36);
                while (paused)
                {
                    yield return null; 
                }
                //   earth.transform.Rotate(new Vector3(x*1, 23.5f, 0), Space.Self); lolol
                earth.transform.Rotate(new Vector3(0, -EARTHDAYPROPORTIONAL / DEGREEACC*36 * ORBITDEGREES * timescale * (1f/Time.deltaTime/60f), 0), Space.Self);
            }
           days = days + 1;

        yield return new WaitForSeconds(EARTHDAYPROPORTIONAL / DEGREEACC);
        }
    }

    IEnumerator DoSunRotation()
    {
        while (true)
        {
            for (int x = 0; x <= DEGREEACC/timescale; x++)
            {
                yield return new WaitForSeconds(EARTHDAYPROPORTIONAL / DEGREEACC * 36);
                while (paused)
                {
                    yield return null;
                }
                sun.transform.Rotate(new Vector3(0, EARTHDAYPROPORTIONAL / DEGREEACC * ORBITDEGREES * timescale * (1f / Time.deltaTime / 60f), 0), Space.Self);
                yield return new WaitForSeconds(EARTHDAYPROPORTIONAL/ DEGREEACC*36);
            }

        }
    }

    IEnumerator DoLunar() {
        while (true) {
            for (int x = 0; x <= DEGREEACC/timescale * 90; x++)
            {
                yield return new WaitForSecondsRealtime(Mathf.Abs(MOONORBITPROPORTIONAL * EARTHDAYPROPORTIONAL) / DEGREEACC / timescale / 2);
                while (paused)
                {
                    yield return null;
                }
                currentmoondegree = x / DEGREEACC * ORBITDEGREES;
                lunarydifferential = Mathf.Sin(currentmoondegree / 90*timescale) * MOONORBITRADII;
                lunarxdifferential = Mathf.Cos(currentmoondegree / 90*timescale) * MOONORBITRADII;
                if (stoporbitalprecision)
                {
                    floatheight = 0;
                }
                moon.transform.position = earth.transform.position + new Vector3(lunarxdifferential, floatheight, lunarydifferential);
                moon.transform.Rotate(new Vector3(0, MOONDAYPROPORTIONAL / DEGREEACC * ORBITDEGREES * timescale * (1f / Time.deltaTime / 60f)) / 90, Space.Self);
                
            }
        }

    }
    IEnumerator DoEcliptic()
    {
        while (true)
        {
            for (float i = -MOONWOBBLER; i <= MOONWOBBLER; i = i + (MOONWOBBLER / Mathf.Abs(MOONORBITPROPORTIONAL) / 2))
            {
                while (paused || stoporbitalprecision) 
                {
                    i = i - 1;
                    yield return new WaitForSecondsRealtime(Mathf.Abs(MOONORBITPROPORTIONAL * EARTHDAYPROPORTIONAL) / DEGREEACC / timescale);
                    floatheight = 0;
                    yield break;
                }

                yield return new WaitForSecondsRealtime(Mathf.Abs(MOONORBITPROPORTIONAL * EARTHDAYPROPORTIONAL) / DEGREEACC / timescale);
                floatheight = i;
            }
            for (float i = MOONWOBBLER; i >= -MOONWOBBLER; i = i - (MOONWOBBLER / Mathf.Abs(MOONORBITPROPORTIONAL) / 2))
            {
                while (paused || stoporbitalprecision)
                {
                    i = i - 1;
                    yield return new WaitForSecondsRealtime(Mathf.Abs(MOONORBITPROPORTIONAL * EARTHDAYPROPORTIONAL) / DEGREEACC / timescale);
                    yield break;
                }
                yield return new WaitForSecondsRealtime(Mathf.Abs(MOONORBITPROPORTIONAL * EARTHDAYPROPORTIONAL) / DEGREEACC / timescale);
                floatheight = i;
            }
        }
    }

    */
}
