using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerSwipe : MonoBehaviour {

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 direction;
    private bool directionChosen;
    public string detected;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    directionChosen = false;
                    break;
                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    break;
                case TouchPhase.Ended:
                    directionChosen = true;
                    break;
            }
        }
        if (directionChosen)
        {
            var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            var delta = Mathf.DeltaAngle(angle, 90.0f);
            if (delta > -45 && delta < 45) {
                detected = "right";
            }
            else if (delta > 45 && delta < 135) {
                detected = "up";
            }
            else if ((delta > 135 && delta < 180) || (delta > -180 && delta < -135)){
                detected = "left";
            }
            else if (delta > -135 && delta < -45) {
                detected = "down";
            }
            directionChosen = false;
        }
    }
}
