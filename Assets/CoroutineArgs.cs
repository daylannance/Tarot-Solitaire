using System;
using System.Collections.Generic;

public class CoroutineArgs
{
	public CoroutineWithArgs routineDelegate;
	public Dictionary<string,object> parameters;
	public CoroutineArgs(CoroutineWithArgs routineDelegate, Dictionary<string,object> parameters)
	{
		this.routineDelegate = routineDelegate;
		this.parameters = parameters;
	}
}
