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
using UnityEngine.UI;
public class LifeBar : MonoBehaviour {
  public Image fillImage;
  public Image thumbnailImage;
  public Sprite[] fillSprites;

  void Start() {
    SetProgress(1.0f);
  }
  
  private Sprite SpriteForProgress(float progress) {
    if (progress >= 0.5f) {
      return fillSprites[0];
    }
    if (progress >= 0.25f) {
      return fillSprites[1];
    }
    return fillSprites[2];
  }
  //3
  public void SetThumbnail(Sprite image, Color color) {
    thumbnailImage.sprite = image;
    thumbnailImage.color = color;
  }
  //4
  public void SetProgress(float progress) {
    fillImage.fillAmount = progress;
    fillImage.sprite = SpriteForProgress(progress);
  }
  //5
  public void EnableLifeBar(bool enabled) {
    foreach (Transform tr in transform) {
      tr.gameObject.SetActive(enabled);
    }
  }
}