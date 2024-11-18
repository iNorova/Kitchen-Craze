using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Scroller : MonoBehaviour

{
    [SerializeField] private RawImage _jpg;
    [SerializeField] private float _x, _y;

    void Update()
{
    _jpg.uvRect = new Rect(_jpg.uvRect.position + new Vector2(_x,_y) * Time.deltaTime,_jpg.uvRect.size);
}



}
