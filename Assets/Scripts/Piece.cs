using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Piece : MonoBehaviour
{
    [SerializeField] private float snapTolerance;

    private Vector3 _originalPosition;
    private Transform _target;
    private Image _image;

    private void Start()
    {
        _originalPosition = transform.position;
        _image = GetComponent<Image>();
    }

    public void Drag()
    {
        transform.position = Input.mousePosition;
    }

    public void Drop()
    {
        CheckMatch();
    }

    private void CheckMatch()
    {
         float distance = Vector3.Distance(transform.position, _target.position);
         Debug.Log($"[{GetType()}]::Distance to Target: {distance}");       
         
         if (distance <= snapTolerance)
             Move(_target.position);
         else
             Move(_originalPosition);
    }

    private void Move(Vector3 position)
    {
        transform.position = position;
    }

    public void Set(Sprite sprite,Transform target)
    {
        _image.sprite = sprite;
        _target = target;
    }
}
