using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour 
{
	public float forwardThrust = 800;
	private float rotationSpeed = 360;
	public float rotationalThrust = 720;
	public float handling = 0;
	public float power = 0;

	int numparts = 0;
	List<ModularPart> parts;
	List<GameObject> openConnectors;
	List<Weapon> weapons1;


	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
	// Use this for initialization
	void Start () 
	{
		parts = new List<ModularPart>();
		openConnectors = new List<GameObject>();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float xinput = Input.GetAxis("X");
		float yinput = Input.GetAxis("Y") * -1;
		if(xinput < .3 && xinput > -.3)//deadzone
		{
			xinput = 0;
		}
		if(yinput < .3 && yinput > -.3) //deadzone
		{
			yinput = 0;
		}
		float forcex = Mathf.Sin (this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad)* -1;
		float forcey = Mathf.Cos (this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
		Debug.DrawRay(this.transform.position, new Vector2(forcex, forcey),Color.blue);
		Vector2 velcomp = new Vector2(forcey, forcex * -1);
		float agl = Vector2.Angle(new Vector2(forcex, forcey), this.rigidbody2D.velocity);
		Vector3 cross = Vector3.Cross(new Vector2(forcex, forcey), this.rigidbody2D.velocity);
		if(cross.z < 0)
		{
			agl = 360 - agl;
		}

		velcomp *= Mathf.Sin(agl * Mathf.Deg2Rad) * this.rigidbody2D.velocity.magnitude * handling; 
		Debug.DrawRay(this.transform.position, velcomp, Color.red);
		//Debug.Log(Mathf.Sin(agl * Mathf.Deg2Rad) * this.rigidbody2D.velocity.magnitude + "   ||   " + velcomp.magnitude);

		float mag = Mathf.Sqrt(Mathf.Pow(xinput, 2) + Mathf.Pow (yinput, 2));
		this.rigidbody2D.AddForce((new Vector2(forcex, forcey) * mag * forwardThrust) + (velcomp * rigidbody2D.mass));
		float angle = Mathf.Atan2(yinput, xinput)*Mathf.Rad2Deg;
		angle -= 90;
		rotationSpeed = rotationalThrust*10/this.rigidbody2D.mass;
		if(xinput != 0 || yinput != 0)
		{
			this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler (0, 0, angle), rotationSpeed * Time.deltaTime);
		}

		//this.rigidbody2D.AddTorque(rotationalThrust * xinput * -1);
		Debug.DrawRay(transform.position, new Vector2(xinput, yinput));
	
	}

	public void addPart(ModularPart part)
	{
		numparts++;
		part.belong = this;
		part.transform.parent = this.transform;
		forwardThrust += part.thrust;
		rotationalThrust += part.rotthrust;
		power += part.power;
		handling += part.handling;
		this.rigidbody2D.mass += part.mass;

		if(parts == null)
		{
			parts = new List<ModularPart>();
		}
		openConnectors = new List<GameObject>();
		for(int x = 0; x < part.connectors.Length; x++)
		{
			foreach(ModularPart mp in parts)
			{
				for(int y = 0; y < mp.connectors.Length; y++)
				{
					if(part.connectedto[x] == null && mp.connectedto[y] == null && Vector2.Distance(part.connectors[x].transform.position, mp.connectors[y].transform.position) < .1f)
					{
						//connect them.
						part.connectedto[x] = mp;
						mp.connectedto[y] = part;

					}
				}
			}
			if(part.connectedto[x] == null)
			{
				openConnectors.Add(part.connectors[x]);
			}
		}
		foreach(ModularPart mp in parts)
		{
			for(int y = 0; y < mp.connectors.Length; y++)
			{
				if(mp.connectedto[y] == null)
				{
					openConnectors.Add(mp.connectors[y]);
				}
			}
		}
		//Type-specific stuff
		if(part.type == ModularPart.Type.Weapon)
		{
			if(weapons1 == null)
			{
				weapons1 = new List<Weapon>();
			}
			weapons1.Add((Weapon)part);
		}


		parts.Add(part);
	}
	public List<GameObject> getOpenConnectors()
	{
		return openConnectors;
	}
	public void enableColliders()
	{
		foreach(ModularPart p in parts)
		{
			p.GetComponent<PolygonCollider2D>().enabled = true;
		}
	}
	public void disableColliders()
	{
		foreach(ModularPart p in parts)
		{
			p.GetComponent<PolygonCollider2D>().enabled = false;
		}
	}
}
