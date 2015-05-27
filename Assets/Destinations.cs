using System;
using System.Collections.Generic;
using UnityEngine;

public class Destinations
{
	public Transform thingMoving;
 	Queue<Destination> list = new Queue<Destination>();
	public Destination current;
	public Destinations(Transform thingMoving)
	{
		this.thingMoving = thingMoving;
	}
	public int Count {
		get{ return list.Count;}
	}
	public void Next()
	{
		if(list.Count > 0) 
		{
			current = list.Dequeue();
			current.Initialize ();
		}	
	}
	public void Add(Vector3 coords, float speed, float delay)
	{
		list.Enqueue(new Destination(thingMoving, this, coords,speed,delay));
	}
	public void Update()
	{
		if(current == null && Count > 0)
		{
			Next ();
		}
		else if(current != null)
		{
			current.Move ();
		}
	}
}
