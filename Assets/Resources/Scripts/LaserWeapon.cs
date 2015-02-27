using UnityEngine;
using System.Collections;

public class LaserWeapon : Weapon 
{
	public GameObject projectile;
	public GameObject projectileloc;
	public float firerate;
	public float accuracy;
	public float projectilespeed;
	private float timer;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(timer < 1/firerate)
		{
			timer += Time.deltaTime;
		}
		//fire ();
	}
	public override void fire()
	{
		float delay = 1/firerate;
		if(timer > delay)
		{
			timer = 0f;
			Quaternion rot = Quaternion.Euler(0, 0, this.transform.rotation.z + Random.Range(-1 * accuracy, accuracy));
			GameObject prjct = (GameObject)Instantiate(projectile, projectileloc.transform.position, rot);
			//Debug.Log (this.transform.rotation.eulerAngles.z);
			float xvel = Mathf.Cos((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad) * projectilespeed;
			//Debug.Log (xvel);
			float yvel = Mathf.Sin((this.transform.rotation.eulerAngles.z +90) * Mathf.Deg2Rad) * projectilespeed;
			//Debug.Log (yvel);
			prjct.rigidbody2D.velocity = new Vector2(xvel, yvel) + this.belong.rigidbody2D.velocity;
		}
	}
}
