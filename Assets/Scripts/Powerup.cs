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
public class Powerup : MonoBehaviour {

  public GameObject rootObject;
  public GameObject shadowSprite;
  public Rigidbody body;
  public int uses = 20;
  public Hero user;
  public SpriteRenderer sprite;

  public AttackData attackData1;
  public AttackData attackData2;
  public AttackData attackData3;

  protected virtual void Update() {
    Vector3 spritePos = shadowSprite.transform.position;
    spritePos.y = 0;
    shadowSprite.transform.position = spritePos;
  }

  public void Use() {
    uses--;
    if (uses <= 0) {
      user.DropWeapon();
      StartCoroutine(DestroyAnimation());
    }
  }
  //2
  protected virtual void SetOpacity(float value) {
    Color color = sprite.color;
    color.a = value;
    sprite.color = color;
  }
  //3
  private IEnumerator DestroyAnimation(int amount = 5) {
    int i = amount;
    while (i > 0) {
      SetOpacity(0.5f);
      yield return new WaitForSeconds(0.2f);
      SetOpacity(1.0f);
      yield return new WaitForSeconds(0.2f);
      i--;
    }
    Destroy(rootObject);
  }

  //4
  public bool CanEquip() {
    return uses > 0;
  }

}