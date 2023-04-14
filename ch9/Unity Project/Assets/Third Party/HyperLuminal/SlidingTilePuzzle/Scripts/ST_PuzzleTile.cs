using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ST_PuzzleTile : MonoBehaviour, IPointerClickHandler
{
	// the target position for this tile.
	public Vector3 TargetPosition;

	// is this an active tile?  usually one per game is inactive.
	public bool Active = true;

	// is this tile in the correct location?
	public bool CorrectLocation = false;

	// store this tiles array location.
	public Vector2 ArrayLocation = new Vector2();
	public Vector2 GridLocation = new Vector2();

	void Awake()
	{
		// assign the new target position.
		TargetPosition = this.transform.localPosition;

		// start the movement coroutine to always move the objects to the new target position.
		StartCoroutine(UpdatePosition());
	}

	public  void LaunchPositionCoroutine(Vector3 newPosition)
	{
		// assign the new target position.
		TargetPosition = newPosition;

		// start the movement coroutine to always move the objects to the new target position.
		StartCoroutine(UpdatePosition());
	}

	public IEnumerator UpdatePosition()
	{
		// whilst we are not at our target position.
		while(TargetPosition != this.transform.localPosition)
		{
			// lerp towards our target.
			this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, TargetPosition, 10.0f * Time.deltaTime);
			yield return null;
		}

		// after each move check if we are now in the correct location.
		if(ArrayLocation == GridLocation){CorrectLocation = true;}else{CorrectLocation = false;}

		// if we are not an active tile then hide our renderer and collider.
		if(Active == false)
		{
			this.GetComponent<Renderer>().enabled = false;
			this.GetComponent<Collider>().enabled = false;
		}

		yield return null;
	}

	public void ExecuteAdditionalMove()
	{
		// get the puzzle display and return the new target location from this tile. 
		LaunchPositionCoroutine(this.transform.parent.GetComponent<ST_PuzzleDisplay>().GetTargetLocation(this.GetComponent<ST_PuzzleTile>()));
	}

    //void OnMouseDown()
    public void OnPointerClick(PointerEventData eventData)
    {
		// get the puzzle display and return the new target location from this tile. 
		LaunchPositionCoroutine(this.transform.parent.GetComponent<ST_PuzzleDisplay>().GetTargetLocation(this.GetComponent<ST_PuzzleTile>()));
	}
}
