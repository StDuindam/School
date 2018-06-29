using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReturnType { Success, Running, Failure };

public abstract class BTNode: MonoBehaviour {

    public abstract ReturnType Run();

}
