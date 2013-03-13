using UnityEngine;

public class AttackAction : Action {
	
	public Controllable Target { get; set; }
	public AIAttacker Attacker { get; set; }
	
	public AttackAction(Controllable actor, Controllable target) : base(actor) {
		Target = target;
		Attacker = Actor.GetComponent<AIAttacker>();
	}
	
	public override void Start() {
		base.Start();
		
		Attacker.Attack(Target.gameObject);
	}

	public override void Update() {
		base.Update();
		
		if(!Attacker.IsAttacking()) {
			Finish();
		}
	}

	public override void Finish() {
		base.Finish();
	}
	
	public override void Abort() {
		base.Abort();
		Attacker.StopAttacking();
	}
}
