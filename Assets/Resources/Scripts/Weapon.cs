using UnityEngine;
using System.Collections;

public abstract class Weapon : ModularPart 
{
	public int max;
	public int current;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	public abstract void fire();
}
