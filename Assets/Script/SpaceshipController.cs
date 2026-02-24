using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private void Update()
    {
        
        Vector3 movement = new Vector3(1f, 0f, 1f) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

}
