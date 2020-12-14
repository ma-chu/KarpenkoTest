using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;
// Перемещение юнита
public class UnitMovement : MonoBehaviour
{
	[SerializeField]
	private float turnSmoothing = 15f;

	private Unit unit;
	private Transform unitTransform;

	private Vector3 movement;

	void Awake()
	{
		unit = GetComponent("Unit") as Unit;
		unitTransform = GetComponent<Transform>();
	}

	public void Move(Vector3 vector)
    {
        //movement = Camera.main.transform.TransformDirection(movement);
        movement = vector.normalized * unit.MovementSpeed * Time.deltaTime;

		unitTransform.position += movement;

		Rotate(vector);
    }

	public void Rotate(Vector3 rotationTo)
    {
        Quaternion targetRotation = Quaternion.LookRotation(rotationTo, Vector3.up);

        Quaternion newRotation = Quaternion.Lerp(unitTransform.rotation, targetRotation, turnSmoothing * Time.deltaTime);

		unitTransform.rotation = newRotation;
	}
}
