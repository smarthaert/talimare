using UnityEngine;
using RAIN.Action;
using RAIN.Core;

public class ZombieMelee : Action {
	
	protected ZombieController controller;
	
	public ZombieMelee() {
        actionName = "ZombieMelee";
    }
	
	public override ActionResult Start(Agent agent, float deltaTime) {
		controller = agent.Avatar.GetComponentInChildren<ZombieController>();
		
        return ActionResult.SUCCESS;
	}

	public override ActionResult Execute(Agent agent, float deltaTime) {
		controller.MeleeAttack();
		
        return ActionResult.SUCCESS;
	}
}
