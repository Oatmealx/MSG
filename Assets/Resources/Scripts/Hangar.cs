using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hangar : MonoBehaviour 
{
	enum Mode{Menu, SelectPartToAdd, PlacePart, SelectPartToRemove, RemovePart};
	public Camera c;
	public GameObject menu;
	public GameObject partlist;
	private GameObject partListElement;
	private GameObject selectedPartToAdd;
	GameObject ship = null;
	int partlistindex = 0;
	bool hidepartlist;
	float partlisttargetY = 0;

	Mode mode;
	int menuindex = 0;
	public GUITexture marker;
	int menusize = 3;
	bool hidemenu;

	public ModularPart[] availableparts;
	List<GameObject> openConnectors;
	int openconnectorsindex = 0;
	int partconnectorindex = 0;


	private float delay = .2f;
	private float counter;
	// Use this for initialization
	void Start () 
	{
		mode = Mode.Menu;
		openConnectors = new List<GameObject>();
		partListElement = (GameObject)Resources.Load ("Prefabs/UIElements/PartListElement");
		createPartList();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(counter < delay)
		{
			counter+= Time.deltaTime;
		}
		if((Input.GetAxis("Z") > 0 && c.orthographicSize < 20) || (Input.GetAxis("Z") < 0 && c.orthographicSize > 2))
		{
			c.orthographicSize += Input.GetAxis("Z") * Time.deltaTime * 5;
		}
		float yinput = Input.GetAxis("Y");
		float xinput = Input.GetAxis("X");
		if(yinput > -.7 && yinput < .7) //dead
		{
			yinput = 0;
		}
		if(xinput > -.7 && xinput < .7) //dead
		{
			xinput = 0;
		}
		if(yinput < 0)
		{
			if(counter >= delay)
			{
				counter = 0;
				up();
			}
		}
		if(yinput > 0)
		{
			if(counter >= delay)
			{
				counter = 0;
				down();
			}
		}
		if(xinput > 0)
		{
			if(counter >= delay)
			{
				counter = 0;
				right();
			}
		}
		if(xinput < 0)
		{
			if(counter >= delay)
			{
				counter = 0;
				left();
			}
		}
		if(Input.GetButtonDown("A"))
		{
			confirm ();
		}
		if(Input.GetButtonDown("B"))
		{
			back();
		}
		if(hidemenu && menu.transform.position.x >-.08)
		{
			menu.transform.Translate(new Vector3(-.5f * Time.deltaTime, 0, 0));
		}
		if(!hidemenu && menu.transform.position.x < 0)
		{
			menu.transform.Translate(new Vector3(.5f * Time.deltaTime, 0, 0));

		}
		if(hidepartlist && partlist.transform.position.x > -.2)
		{
			partlist.transform.Translate(new Vector3(-.5f * Time.deltaTime, 0, 0));
		}
		if(!hidepartlist && partlist.transform.position.x < 0)
		{
			partlist.transform.Translate(new Vector3(.5f * Time.deltaTime, 0, 0));
		}
		if(partlist.transform.position.y != partlisttargetY)
		{
			partlist.transform.Translate(0, (partlisttargetY - partlist.transform.position.y)/3f, 0);
		}
	}
	private void up()
	{
		//Debug.Log ("UP");
		if(mode == Mode.Menu)
		{
			menuindex++;
			menuindex %= menusize;
			marker.pixelInset = new Rect(48, 18+33*menuindex, 16, 16);
		}
		else if(mode ==  Mode.SelectPartToAdd)
		{
			partlist.transform.GetChild(partlistindex).transform.Translate (-.01f, 0, 0);
			partlistindex--;
			if(partlistindex < 0)
			{
				partlistindex = availableparts.GetLength(0)-1;
			}
			Debug.Log (partlistindex);
			partlist.transform.GetChild(partlistindex).transform.Translate (.01f, 0, 0);
		}
		else if(mode == Mode.PlacePart)
		{
			if(ship != null)
			{
				openconnectorsindex--;
				if(openconnectorsindex < 0)
				{
					openconnectorsindex = openConnectors.Count-1;
				}
				findSelectedPartTransform();
			}
		}
	}
	private void down()
	{
		//Debug.Log("DOWN");
		if(mode == Mode.Menu)
		{
			menuindex--;
			if(menuindex < 0)
			{
				menuindex = menusize - 1;
			}
			marker.pixelInset = new Rect(48, 18+33*menuindex, 16, 16);
		}
		else if(mode == Mode.SelectPartToAdd)
		{
			partlist.transform.GetChild(partlistindex).transform.Translate (-.01f, 0, 0);
			partlistindex++;
			partlistindex %= availableparts.GetLength(0);
			//Debug.Log (partlistindex);
			partlist.transform.GetChild(partlistindex).transform.Translate (.01f, 0, 0);
		}
		else if(mode == Mode.PlacePart)
		{
			if(ship != null)
			{
				openconnectorsindex ++;
				openconnectorsindex %= openConnectors.Count;
				findSelectedPartTransform();
			}
		}
	}
	private void left()
	{
		//Debug.Log ("LEFT");
		if(mode == Mode.PlacePart)
		{
			if(ship != null)
			{
				partconnectorindex--;
				if(partconnectorindex < 0)
				{
					partconnectorindex = selectedPartToAdd.GetComponent<ModularPart>().connectors.Length - 1;
				}
				findSelectedPartTransform();
			}
		}
	}
	private void right()
	{
		//Debug.Log ("RIGHT");
		if(mode == Mode.PlacePart)
		{
			if(ship != null)
			{
				partconnectorindex++;
				partconnectorindex %= selectedPartToAdd.GetComponent<ModularPart>().connectors.Length;
				findSelectedPartTransform();
			}
		}
	}
	private void confirm()
	{
		//Debug.Log ("CONFIRM");
		if(mode==Mode.Menu)
		{
			if(menuindex == 0)
			{
				//Debug.Log ("Add");
				hidemenu = true;
				mode = Mode.SelectPartToAdd;
				hidepartlist = false;
				partlist.transform.GetChild(partlistindex).transform.Translate (.01f, 0, 0);
			}
			if(menuindex == 1)
			{
				//Debug.Log ("Remove");
			}
			if(menuindex == 2)
			{
				//Debug.Log ("Test");
				launch();
			}
		}
		else if(mode == Mode.SelectPartToAdd)
		{
			selectPart();
			mode = Mode.PlacePart;
			hidepartlist = true;
			//Debug.Log ("SelectedPart");
		}
		else if(mode == Mode.PlacePart)
		{
			addPart ();
			mode = Mode.SelectPartToAdd;
			hidepartlist = false;
		}

	}
	private void back()
	{
		//Debug.Log ("BACK");
		if(mode==Mode.SelectPartToAdd)
		{
			hidemenu = false;
			hidepartlist = true;
			partlist.transform.GetChild(partlistindex).transform.Translate (-.01f, 0, 0);
			mode = Mode.Menu;
		}
		if(mode == Mode.PlacePart)
		{
			hidepartlist = false;
			mode = Mode.SelectPartToAdd;
			Destroy(selectedPartToAdd.gameObject);
		}
	}
	void createPartList()
	{
		Debug.Log("Creating Part List...");
		int numparts = availableparts.GetLength(0);
		Debug.Log (numparts);
		for(int x = 0; x < numparts; x++)
		{
			GameObject ple = (GameObject)Instantiate(partListElement, new Vector3(0, 1, 0), this.transform.rotation);
			ple.transform.GetChild(1).GetComponent<GUIText>().text = availableparts[x].name;
			ple.GetComponent<GUITexture>().pixelInset = new Rect(0, -58 + (x * -49), 128, 48);
			ple.transform.GetChild(0).GetComponent<GUITexture>().pixelInset = new Rect(5, -49 + (x * -49), 32, 32);
			ple.transform.GetChild(1).GetComponent<GUIText>().pixelOffset = new Vector2(42, -15 + (x * -49));
			ple.transform.parent = partlist.transform;
		}
		partlist.transform.Translate(-.2f, 0, 0);

		hidepartlist = true;
	}
	void selectPart()
	{
		selectedPartToAdd = (GameObject)(Instantiate(Resources.Load("Prefabs/Parts/" + availableparts[partlistindex].name)));
		if(ship == null)
		{
			/*
			ship = selectedPartToAdd;
			ship.AddComponent("Rigidbody2D");
			ship.rigidbody2D.gravityScale = 0;
			ship.rigidbody2D.drag = 1;
			ship.rigidbody2D.angularDrag=.5f;
			ship.AddComponent("Ship");
			ship.GetComponent<Ship>().enabled = false;
			*/
		}
		else
		{
			openconnectorsindex = 0;
			partconnectorindex = 0;
			findSelectedPartTransform();
			//selectedPartToAdd.transform.position = openConnectors[openconnectorsindex].transform.position - selectedPartToAdd.GetComponent<ModularPart>().connectors[partconnectorindex].transform.position;
		}
	}
	void addPart()
	{
		if(ship == null)
		{
			ship = selectedPartToAdd;
			ship.name = "PlayerShip";
			ship.AddComponent("Rigidbody2D");
			ship.rigidbody2D.gravityScale = 0;
			ship.rigidbody2D.drag = 1;
			ship.rigidbody2D.angularDrag=.5f;
			ship.rigidbody2D.mass = 0;
			ship.AddComponent("Ship");
			ship.GetComponent<Ship>().forwardThrust = 0;
			ship.GetComponent<Ship>().rotationalThrust = 0;
			ship.GetComponent<Ship>().power = 0;
			ship.GetComponent<Ship>().handling = 0;
			ship.GetComponent<Ship>().enabled = false;
		}

		ship.GetComponent<Ship>().addPart(selectedPartToAdd.GetComponent<ModularPart>());
		openConnectors = ship.GetComponent<Ship>().getOpenConnectors();
	}
	void findSelectedPartTransform()
	{
		GameObject pc = selectedPartToAdd.GetComponent<ModularPart>().connectors[partconnectorindex];
		GameObject oc = openConnectors[openconnectorsindex];

		float pcrot = pc.transform.localRotation.eulerAngles.z;
		float ocrot = oc.transform.rotation.eulerAngles.z;
		/*
		float dist = Mathf.Abs (pc.transform.localPosition.x * Mathf.Cos(pcrot * Mathf.Deg2Rad) + pc.transform.localPosition.y * Mathf.Sin(pcrot * Mathf.Deg2Rad));
		Debug.Log ("PC DIST: " + dist);
		float deltax = Mathf.Cos(ocrot * Mathf.Deg2Rad) * dist;
		float deltay = Mathf.Sin(ocrot * Mathf.Deg2Rad) * dist;

		Debug.Log("deltax: " + deltax);
		Debug.Log("deltay: " + deltay);
		*/

		selectedPartToAdd.transform.rotation = Quaternion.Euler(selectedPartToAdd.transform.rotation.x, selectedPartToAdd.transform.rotation.y, ocrot - pcrot + 180f);
		//selectedPartToAdd.transform.position = oc.transform.position + new Vector3(deltax, deltay, 0);
		selectedPartToAdd.transform.position = oc.transform.position + (selectedPartToAdd.transform.position - pc.transform.position);

	}
	void launch()
	{
		if (ship == null) 
		{

		}
		ship.GetComponent<Ship>().enableColliders();
		ship.GetComponent<Ship>().enabled = true;
		Application.LoadLevel("launch");
	}
}

//first part is the center of the ship
//cannot launch without cockpit/can only be one cockpit
//ship has an error with generating open connectors list - adds duplicates for each part. FIXED
//findselectedparttransform is broken, does not account for delta in the direction perpendicular to the connector
