using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : MonoBehaviour
{
    private CharacterMovement characterMovement;


    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }


    void Update()
    {
        Vector3 input = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        characterMovement.SetInput(input);
    }

}
