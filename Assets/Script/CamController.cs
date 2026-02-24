using UnityEngine;

public class CamController : MonoBehaviour
{

    [SerializeField] private Camera Camera;
    [SerializeField] private Transform Player;
    [SerializeField] private Transform FpsView;
    [Header("Third Person Settings")]
    [SerializeField] private Vector3 tpsOffset = new Vector3(0, 5, -15);
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private float rotationSmoothSpeed = 5f;


    private bool isFpsMode = false;
    void Update()
    {

        // Toggle view on V key press
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFpsMode = !isFpsMode;
        }
    }


    private void LateUpdate()
    {
        if (Player == null) return;

        if (isFpsMode)
        {
            transform.position = FpsView.position;
            transform.rotation = FpsView.rotation;
        }

        else
        {
            Vector3 TargetPos = Player.TransformPoint(tpsOffset);
            transform.position = Vector3.Lerp(transform.position,TargetPos, smoothSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(Player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
        }
    }
}
