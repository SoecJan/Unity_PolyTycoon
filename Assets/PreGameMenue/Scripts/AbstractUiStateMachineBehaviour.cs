using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractUiStateMachineBehaviour : StateMachineBehaviour
{
	override public void OnStateExit(Animator animator,
		AnimatorStateInfo stateInfo,
		int layerIndex)
	{
		if (stateInfo.IsName("ExitAnimation"))
		{
			
		}
	}
}
