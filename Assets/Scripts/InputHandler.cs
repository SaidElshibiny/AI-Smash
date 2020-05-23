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


using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour {
  float horizontal;
  float vertical;
  bool jump;
  float lastJumpTime;
  bool isJumping;
  bool attack;

  public float maxJumpDuration = 0.2f;
  bool didAttack;
  public bool useUI = true;

  public float GetVerticalAxis() {
    return vertical;
  }
  public float GetHorizontalAxis() {
    return horizontal;
  }
  public bool GetJumpButtonDown() {
    return jump;
  }
  public bool GetAttackButtonDown() {
    return attack;
  }

  void Update() {
    if (useUI) {
      if (didAttack) {
        didAttack = attack = false;
      } else if (attack) {
        didAttack = true;
      }
    } else {
      horizontal = Input.GetAxisRaw("Horizontal");
      vertical = Input.GetAxisRaw("Vertical");
      attack = Input.GetButtonDown("Attack");
      if(!jump && !isJumping && Input.GetButton("Jump")) {
        jump = true;
        lastJumpTime = Time.time;
        isJumping = true;
      } else if(!Input.GetButton("Jump")) {
        jump = false;
        isJumping = false;
      }
    }
    if(jump && Time.time > lastJumpTime + maxJumpDuration) {
      jump = false;
    }
  }

  public void DidPressAttack(BaseEventData data) {
    attack = true;
    didAttack = false;
  }
  
  public void DidPressJump(BaseEventData data) {
    if (!jump) {
      jump = true;
      lastJumpTime = Time.time;
    }
  }
  
  public void DidReleaseJump(BaseEventData data) {
    jump = false;
  }

  public Vector2 VectorForPadDirection(ActionDPad.ActionPadDirection padDirection) {
    float maxX = 1.0f;
    float maxY = 1.1f;
    switch (padDirection) {
      case ActionDPad.ActionPadDirection.None:
        return Vector2.zero;
      case ActionDPad.ActionPadDirection.Up:
        return new Vector2(0, maxY);
      case ActionDPad.ActionPadDirection.UpRight:
        return new Vector2(maxX, maxY);
      case ActionDPad.ActionPadDirection.Right:
        return new Vector2(maxX, 0);
      case ActionDPad.ActionPadDirection.DownRight:
        return new Vector2(maxX, -maxY);
      case ActionDPad.ActionPadDirection.Down:
        return new Vector2(0, -maxY);
      case ActionDPad.ActionPadDirection.DownLeft:
        return new Vector2(-maxX, -maxY);
      case ActionDPad.ActionPadDirection.Left:
        return new Vector2(-maxX, 0);
      case ActionDPad.ActionPadDirection.UpLeft:
        return new Vector2(-maxX, maxY);
      default:
        return Vector2.zero;
    } 
  }

  public void OnActionPadChangeDirection(ActionDPad.ActionPadDirection direction) {
    Vector2 directionVector = VectorForPadDirection(direction);
    horizontal = directionVector.x;
    vertical = directionVector.y;
  }

}