using UnityEngine;
using System.Collections;

public class ModularPart : MonoBehaviour 
{
	public enum Type{Cockpit, Reactor, Engine, Weapon, Subsystem, Turret, Hull};
	public string partname;
	public GameObject[] connectors;
	public ModularPart[] connectedto;
	public int mass;
	public int power;
	public int thrust;
	public int rotthrust;
	public float handling;
	public Type type;

	public Ship belong;


	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
