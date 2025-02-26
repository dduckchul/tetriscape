using System.Collections;
using UnityEngine;

public interface IMoveable
{
    IEnumerator Fall();
    void Move(Vector2 dir);
}
