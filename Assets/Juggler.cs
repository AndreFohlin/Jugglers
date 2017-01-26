using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Juggler : MonoBehaviour {



	public List<Ball> ballsToCatch = new List<Ball>();

	public State state;

	public enum State {
		LOOKING_FOR_BALL,
		FETCHING,
		THROWING,
		CATCHING
	};

	private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = gameObject.GetComponent<NavMeshAgent>() as NavMeshAgent;
		state = State.LOOKING_FOR_BALL;
	}
	
	// Update is called once per frame
	void Update () {

		switch( state ){
		case State.LOOKING_FOR_BALL:
			
			if(ballsToCatch.Count > 0 && !agent.pathPending){
				agent.SetDestination( ballsToCatch[0].destination );

			} else {
				Ball newBall = FindAvailableBall();
				if(newBall != null) {
					ballsToCatch.Add(newBall);
					newBall.owner = this;
					agent.SetDestination( newBall.transform.position );
					state = State.FETCHING;
				}
			}
			break;
		case State.FETCHING:
			if(ballsToCatch[0] && !agent.pathPending){
				
				if(agent.remainingDistance < 0.1f ){
					state = State.THROWING;
				}
			}
			break;
		case State.THROWING:
			Juggler targetJuggler = FindJuggler();
			if(targetJuggler){
				Ball b = ballsToCatch[0];
				ballsToCatch.RemoveAt(0);
				ThrowBallTo(b, targetJuggler);

				state = State.LOOKING_FOR_BALL;
			}
			break;
		/*case State.CATCHING:
			
			if( currentBall == null ) {
				currentBall = ballsToCatch[0];
			}else{
				state = State.FETCHING;
				agent.SetDestination( currentBall.transform.position );
			}

			break;
		*/
		}
	}

	void OnCollisionEnter(Collision c) {
		Debug.Log(c.gameObject.name);
		if(c.gameObject.tag == "Throwable") {
			Debug.Log("Ball hit the juggler");
		}
	}

	private Ball FindAvailableBall() {
		Ball[] balls = GameObject.FindObjectsOfType<Ball>();

		for (int i = 0; i < balls.Length; ++i ) {
			Ball b = balls[i];
			if (b.IsAvailable()) {
				return b;
			}
		}
		return null;
	}

	private Juggler FindJuggler() {
		Juggler[] jugglers = GameObject.FindObjectsOfType<Juggler>();

		for (int i = 0; i < jugglers.Length; ++i ) {
			Juggler j = jugglers[i];
			if (j != this) {
				return j;
			}
		}
		return null;
	}

	public void ThrowBallTo(Ball ball, Juggler target){
		ball.owner = target;
		Vector3 newBallPos = ball.transform.position;
		newBallPos.y = transform.position.y;

		Vector3 dirToTarget = target.transform.position - this.transform.position;
		Rigidbody rigidBody = ball.GetComponent<Rigidbody>() as Rigidbody;
		dirToTarget.y = 4;
		rigidBody.position = newBallPos;
		rigidBody.velocity = BallisticVel(ball.transform, target.transform, 45.0f);
		ball.destination = target.transform.position;
		target.AssignBall(ball);
		ball = null;
	}

	public void AssignBall(Ball b){
		ballsToCatch.Add(b);

	}

	private Vector3 BallisticVel( Transform projectile, Transform target, float angle) { 
		Vector3 dir = target.position - projectile.position; 
		// get target direction 
		float h = dir.y; 
		// get height difference 
		dir.y = 0; 
		// retain only the horizontal direction 
		float dist = dir.magnitude;
		// get horizontal distance 
		float a = angle * Mathf.Deg2Rad; 
		// convert angle to radians 
		dir.y = dist * Mathf.Tan(a);
		// set dir to the elevation angle 
		dist += h / Mathf.Tan(a); 
		// correct for small height differences 
		// calculate the velocity magnitude 
		float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a)); 
		return vel * dir.normalized; 
	}


}
