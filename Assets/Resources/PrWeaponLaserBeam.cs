using UnityEngine;
using System.Collections;

public class PrWeaponLaserBeam : MonoBehaviour {

    private bool isActive = false;
    public float timeToLive = 1.0f;
    private float actualTimeToLive = 0.0f;

    public float startWidth = 0.5f;
    public float endWidth = 0.5f;

    public AnimationCurve actualIntensityCurve;
    private float curveTime = 0.0f;
    private float actualintensity = 0.0f;
    private LineRenderer actualLine;
    private Color startColor = Color.white;
    private Color endColor = Color.white;

    public Transform startPointParent;

	// Use this for initialization
	void Start () {
        actualLine = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

	    if (isActive && actualLine)
        {
            if (actualTimeToLive > 0.0f)
            {
                actualTimeToLive -= Time.deltaTime;
                curveTime += Time.deltaTime / timeToLive;
                actualintensity = actualIntensityCurve.Evaluate(curveTime);
                actualLine.startColor = startColor * actualintensity;
                actualLine.startColor = endColor * actualintensity;
                //actualLine.SetColors(startColor * actualintensity, endColor * actualintensity); OLD 5.3 version
            }
            else
            {
                ResetLaser();
            }
            
        }
	}

    public void SetPositions(Vector3 start, Vector3 end)
    {
        actualLine.SetPosition(0, start);
        actualLine.SetPosition(1, end);
        actualLine.material.SetFloat("_Tile", Vector3.Distance(start, end));
    }

    void LateUpdate()
    {
        /*if (actualLine && startPointParent)
        {
            actualLine.SetPosition(0, startPointParent.position);
        }*/
    }


    void ResetLaser()
    {
        actualLine.enabled = false;
        actualTimeToLive = timeToLive;
        isActive = false;
        actualLine.startColor = startColor;
        actualLine.startColor = endColor;
        //actualLine.SetColors(startColor, endColor);  OLD 5.3 version
        curveTime = 0.0f;
    }

    void InitializeLine()
    {
        if (GetComponent<LineRenderer>())
        {
            actualLine = GetComponent<LineRenderer>();
        }
    }
    public void InitializeLine(float widthFactor, Transform initialPos)
    {
        if (GetComponent<LineRenderer>())
        {
            actualLine = GetComponent<LineRenderer>();
            actualLine.startWidth = startWidth * widthFactor;
            actualLine.endWidth = endWidth * widthFactor;
            //actualLine.SetWidth(startWidth * widthFactor, endWidth * widthFactor); OLD 5.3 version
            
        }
        startPointParent = initialPos;
    }

    public void Activate(float time)
    {
        if (!actualLine)
            InitializeLine();

        ResetLaser();

        isActive = true;
        timeToLive = time;
        actualLine.enabled = true;
    }
}
