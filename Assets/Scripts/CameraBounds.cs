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

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour {

  public float minVisibleX;
  public float maxVisibleX;
  private float minValue;
  private float maxValue;
  public float cameraHalfWidth;

  public Camera activeCamera;
  public Transform cameraRoot;

  public Transform leftBounds;
  public Transform rightBounds;

  public float offset;

  public Transform introWalkStart;
  public Transform introWalkEnd;
  public Transform exitWalkEnd;


  void Start() {

    activeCamera = Camera.main;

    cameraHalfWidth = Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x -
      activeCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x) * 0.5f;
    minValue = minVisibleX + cameraHalfWidth;
    maxValue = maxVisibleX - cameraHalfWidth;

    Vector3 position; 
    position = leftBounds.transform.localPosition;
    position.x = transform.localPosition.x - cameraHalfWidth;
    leftBounds.transform.localPosition = position;
    
    position = rightBounds.transform.localPosition;
    position.x = transform.localPosition.x + cameraHalfWidth;
    rightBounds.transform.localPosition = position;

    position = introWalkStart.transform.localPosition;
    position.x = transform.localPosition.x - cameraHalfWidth - 2.0f;
    introWalkStart.transform.localPosition = position;

    position = introWalkEnd.transform.localPosition;
    position.x = transform.localPosition.x - cameraHalfWidth + 2.0f;
    introWalkEnd.transform.localPosition = position;

    position = exitWalkEnd.transform.localPosition;
    position.x = transform.localPosition.x + cameraHalfWidth + 2.0f;
    exitWalkEnd.transform.localPosition = position;

  }

  public void SetXPosition(float x) {
    Vector3 trans = cameraRoot.position;
    trans.x = Mathf.Clamp(x + offset, minValue, maxValue);
    cameraRoot.position = trans;
  }

  public void CalculateOffset(float actorPosition) {
    offset = cameraRoot.position.x - actorPosition;
    SetXPosition(actorPosition);
    StartCoroutine(EaseOffset());
  }

  private IEnumerator EaseOffset() {
    while (offset != 0) {
      offset = Mathf.Lerp(offset, 0, 0.1f);
      if (Mathf.Abs(offset) < 0.05f) {
        offset = 0; 
      }
      yield return new WaitForFixedUpdate();
    }
  }

  public void EnableBounds(bool isEnabled) {
    rightBounds.GetComponent<Collider>().enabled = isEnabled;
    leftBounds.GetComponent<Collider>().enabled = isEnabled;
  }
}