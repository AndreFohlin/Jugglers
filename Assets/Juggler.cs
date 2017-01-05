using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Juggler : MonoBehaviour {
	public Ball currentBall;
	public List<Ball> ballsToCatch = new List<Ball>();

	public Juggler targetJuggler;

	public State state = State.LOOKING_FOR_BALL;

	public enum State {
		LOOKING_FOR_BALL,
		FETCHING,
		THROWING,
		CATCHING
	};

	public NavMeshAgent agent;
	// Use this for initialization
	void Start () {
		agent = gameObject.GetComponent<NavMeshAgent>() as NavMeshAgent;
	}
	
	// Update is called once per frame
	void Update () {

		switch( state ){
		case State.LOOKING_FOR_BALL:
			currentBall = FindAvailableBall();
			if(ballsToCatch.Count > 0){
				state = State.CATCHING;
			} else if(currentBall){
				currentBall.owner = this;
				agent.SetDestination( currentBall.transform.position );
				state = State.FETCHING;
			}
			break;
		case State.FETCHING:
			if(currentBall && !agent.pathPending){
				agent.SetDestination( currentBall.transform.position );
				if(agent.remainingDistance < 0.1f ){
					Debug.Log("Throwing");
					state = State.THROWING;
				}
			}
			break;
		case State.THROWING:
			targetJuggler = FindJuggler();
			if(targetJuggler){
				ThrowBallTo(currentBall,targetJuggler);
				targetJuggler = null;
				if(ballsToCatch.Count>0)
					state = State.CATCHING;
				else
					state = State.LOOKING_FOR_BALL;
			}
			break;
		case State.CATCHING:
			if( currentBall == null ) {
				currentBall = ballsToCatch[0];
			}else{
				state = State.FETCHING;
				agent.SetDestination( currentBall.transform.position );
			}



			break;
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
		ball.owner = targetJuggler;
		Vector3 newBallPos = ball.transform.position;
		newBallPos.y = transform.position.y;

		Vector3 dirToTarget = target.transform.position - this.transform.position;
		Rigidbody rigidBody = ball.GetComponent<Rigidbody>() as Rigidbody;
		dirToTarget.y = 4;
		rigidBody.AddForce(dirToTarget * 1.7f, ForceMode.Impulse);
		target.AssignBall(ball);
		ball = null;
	}

	public void AssignBall(Ball b){
		ballsToCatch.Add(b);

	}
}
