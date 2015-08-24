using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Destinations
{
	public Transform thingMoving;
 	public Queue<Destination> list = new Queue<Destination>();
	public Destination current;
	Card card;
	public Destinations(Transform thingMoving, Card card)
	{
		this.thingMoving = thingMoving;
		this.card = card;
	}
	public int count {
		get{ return list.Count;}
	}
	public void Next()
	{
		if(list.Count > 0) 
		{
			current = list.Dequeue();
		}	
		else current = null;
	}
	public void Add(Vector3 coords, float speed, float delay, AnimationMode mode)
	{
		list.Enqueue(new Destination(thingMoving, this, coords,speed,delay,mode,card));
	}
	public void Update()
	{
		if(current == null && count > 0)
		{
			Next ();
		}
		else if(current != null)
		{
			current.Move ();
		}
	}
}
