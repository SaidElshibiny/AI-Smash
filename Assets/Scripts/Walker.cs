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


using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Actor))]
public class Walker : MonoBehaviour {

  public NavMeshAgent navMeshAgent;
  private NavMeshPath navPath;
  private List<Vector3> corners;

  float currentSpeed;
  float speed;

  private Actor actor;
  private System.Action didFinishWalk;

  void Start() {
    navMeshAgent.updatePosition = false;
    navMeshAgent.updateRotation = false;
    actor = GetComponent<Actor> ();
  }

   public bool MoveTo(Vector3 targetPosition, System.Action callback = null) {
    navMeshAgent.Warp(transform.position);
    didFinishWalk = callback;
    speed = actor.speed;
    navPath = new NavMeshPath();
    bool pathFound = navMeshAgent.CalculatePath(targetPosition, navPath);
    if (pathFound) {
      corners = navPath.corners.ToList();
      return true;
    }
    return false;
  }

  public void StopMovement() {
    navPath = null;
    corners = null;
    currentSpeed = 0;
  }

  protected void FixedUpdate() {
    bool canWalk = actor.CanWalk();
    if (canWalk && corners != null && corners.Count > 0) {
      currentSpeed = speed;
      actor.body.MovePosition( Vector3.MoveTowards(transform.position, corners[0], Time.fixedDeltaTime * speed));
 
      if (Vector3.SqrMagnitude( transform.position - corners[0]) < 0.6f) {
        corners.RemoveAt(0);
      }

      if (corners.Count > 0) {
        currentSpeed = speed;
        Vector3 direction = transform.position - corners[0];
        actor.FlipSprite(direction.x >= 0);
      } else {
        currentSpeed = 0.0f;
        if (didFinishWalk != null) {
          didFinishWalk.Invoke();
          didFinishWalk = null;
        }
      }
    }
    actor.baseAnim.SetFloat("Speed", currentSpeed);
  }
}