using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    public GameOverManager gameOverManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOverManager.GameOver();
        }
    }
}