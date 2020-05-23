/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ActionDPad : UIBehaviour, IBeginDragHandler,
IEndDragHandler, IDragHandler, IPointerDownHandler,
IPointerUpHandler { 
  public enum ActionPadDirection {
    Up = 1,
    UpRight = 2,
    Right = 3,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft,
    None = 999
  };

  [SerializeField]
  float radius = 1;
  [HideInInspector]
  bool isHeld;
//3
  [SerializeField]
  Sprite[] directionalSprites;
//4
  [Serializable]
  public class JoystickMoveEvent : UnityEvent<ActionPadDirection> { }
  public JoystickMoveEvent OnValueChange;

  private ActionPadDirection UpdateTouchSprite(Vector2 direction)
  {
  //1
    float angle = Mathf.Atan2(direction.x, direction.y) *
  Mathf.Rad2Deg;
  //2
    if (angle < 0) {
      angle += 360;
    }
  //3
    ActionPadDirection currentPadDirection =
  ActionPadDirection.None;
    if (angle <= 22.5f || angle > 337.5f) {
      currentPadDirection = ActionPadDirection.Up;
    } else if (angle > 22.5 && angle <= 67.5) {
      currentPadDirection = ActionPadDirection.UpRight;
    } else if (angle > 67.5 && angle <= 112.5) {
      currentPadDirection = ActionPadDirection.Right;
    } else if (angle > 112.5 && angle <= 157.5) {
      currentPadDirection = ActionPadDirection.DownRight;
    } else if (angle > 157.5 && angle <= 202.5) {
      currentPadDirection = ActionPadDirection.Down;
    } else if (angle > 202.5 && angle <= 247.5) {
      currentPadDirection = ActionPadDirection.DownLeft;
    } else if (angle > 247.5 && angle <= 292.5) {
      currentPadDirection = ActionPadDirection.Left;
    } else if (angle > 292.5 && angle <= 337.5) {
      currentPadDirection = ActionPadDirection.UpLeft;
    }

    int index = 0;
    if (currentPadDirection != ActionPadDirection.None) {
      index = (int)currentPadDirection;
    }
    GetComponent<Image>().sprite = directionalSprites[index];
    return currentPadDirection;
  }

  public void OnBeginDrag(PointerEventData eventData) {
    if (!IsActive()) {
      return; 
    }
  
    RectTransform thisRect = transform as RectTransform;
    Vector2 touchDir;
    bool didConvert = RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
  
    if (touchDir.sqrMagnitude > radius * radius) {
      touchDir.Normalize();
      isHeld = true;
      ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
      OnValueChange.Invoke(currentDirection);
    }
  }
  
  public void OnEndDrag(PointerEventData eventData) {
    OnValueChange.Invoke(ActionPadDirection.None);
     GetComponent<Image>().sprite = directionalSprites[0];
  }
  
  public void OnDrag(PointerEventData eventData) {
    if (isHeld) {
      RectTransform thisRect = transform as RectTransform;
      Vector2 touchDir;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
      touchDir.Normalize();
      ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
      OnValueChange.Invoke(currentDirection);
    }
  }

  public void OnPointerDown(PointerEventData eventData) {
    RectTransform thisRect = transform as RectTransform;
    Vector2 touchDir;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
    touchDir.Normalize();
    
    ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
    OnValueChange.Invoke(currentDirection);
  }

  public void OnPointerUp(PointerEventData eventData) {
    OnValueChange.Invoke(ActionPadDirection.None);
    GetComponent<Image>().sprite = directionalSprites[0];
  }


}