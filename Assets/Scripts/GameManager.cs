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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
  
  public Hero actor;
  public bool cameraFollows = true;
  public CameraBounds cameraBounds;

  public LevelData currentLevelData;
  private BattleEvent currentBattleEvent;
  private int nextEventIndex;
  public bool hasRemainingEvents;

  public List<GameObject> activeEnemies;
  public Transform[] spawnPositions;

  public GameObject currentLevelBackground;

  public GameObject robotPrefab;
  public GameObject bossPrefab;

  public GameObject levelNamePrefab;
  public GameObject gameOverPrefab;
  public RectTransform uiTransform;

  public GameObject loadingScreen;

  public Transform walkInStartTarget;
  public Transform walkInTarget;
  
  public Transform walkOutTarget;
  
  public LevelData[] levels;
  public static int CurrentLevel = 0;

  public LifeBar enemyLifeBar;
  public GameObject goIndicator;

  void Awake() {
    loadingScreen.SetActive(true);
  }

  void Start() {

    cameraBounds.SetXPosition(cameraBounds.minVisibleX);

    nextEventIndex = 0;
    StartCoroutine(LoadLevelData(levels[CurrentLevel]));
  }

  void Update() {
    if (currentBattleEvent == null && hasRemainingEvents) {
      if (Mathf.Abs(currentLevelData.battleData[nextEventIndex].column - cameraBounds.activeCamera.transform.position.x) < 0.2f) {
        PlayBattleEvent(currentLevelData.battleData[nextEventIndex]);
      }
    }
    
    if (currentBattleEvent != null) {
      if (Robot.TotalEnemies == 0) {
        CompleteCurrentEvent();
      }
    }

    if (cameraFollows) {
      cameraBounds.SetXPosition(actor.transform.position.x);
    } 
  }

  private GameObject SpawnEnemy(EnemyData data) {
    
    GameObject enemyObj;
    if (data.type == EnemyType.Boss) {
      enemyObj = Instantiate(bossPrefab);
    } else {
      enemyObj = Instantiate(robotPrefab);
    }

    Vector3 position = spawnPositions[data.row].position;
    position.x = cameraBounds.activeCamera.transform.position.x + (data.offset * (cameraBounds.cameraHalfWidth + 1));
    enemyObj.transform.position = position;

    if (data.type == EnemyType.Robot) {
      enemyObj.GetComponent<Robot>().SetColor(data.color);
    }
    enemyObj.GetComponent<Enemy>().RegisterEnemy();
    return enemyObj;
  }

  private void PlayBattleEvent(BattleEvent battleEventData) {
    currentBattleEvent = battleEventData;
    nextEventIndex++;
    cameraFollows = false;
    cameraBounds.SetXPosition(battleEventData.column);
    foreach (GameObject enemy in activeEnemies) {
      Destroy(enemy);
    }
    activeEnemies.Clear();
    Enemy.TotalEnemies = 0;

    foreach (EnemyData enemyData in currentBattleEvent.enemies) {
      activeEnemies.Add(SpawnEnemy(enemyData));
    }
  }

  private void CompleteCurrentEvent() {
    currentBattleEvent = null;
    cameraFollows = true;
    cameraBounds.CalculateOffset(actor.transform.position.x);
    hasRemainingEvents = currentLevelData.battleData.Count >
    nextEventIndex;

    enemyLifeBar.EnableLifeBar (false);

    if (!hasRemainingEvents) {
      StartCoroutine(HeroWalkout());
    } else {
      ShowGoIndicator();
    }
  }

  private IEnumerator LoadLevelData(LevelData data) {
    cameraFollows = false;
    currentLevelData = data;
    hasRemainingEvents = currentLevelData.battleData.Count > 0;
    activeEnemies = new List<GameObject>();
    yield return null;
    cameraBounds.SetXPosition(cameraBounds.minVisibleX);
    if (currentLevelBackground != null) {
      Destroy(currentLevelBackground);
    }
    currentLevelBackground = Instantiate(currentLevelData.levelPrefab);
    
    cameraBounds.EnableBounds(false);
    actor.transform.position = walkInStartTarget.transform.position;

    yield return new WaitForSeconds(0.1f);

    actor.UseAutopilot (true);
    actor.AnimateTo(walkInTarget.transform.position, false, DidFinishIntro);

    cameraFollows = true;
    ShowTextBanner(currentLevelData.levelName);
    loadingScreen.SetActive(false);
  }

  private void DidFinishIntro() {
    actor.UseAutopilot (false);
    actor.controllable = true;
    cameraBounds.EnableBounds(true);

    ShowGoIndicator();
  }

  private IEnumerator HeroWalkout() {
    cameraBounds.EnableBounds(false);
    cameraFollows = false;
    actor.UseAutopilot (true);
    actor.controllable = false;
    actor.AnimateTo(walkOutTarget.transform.position, true, DidFinishWalkout);
    yield return null;
  }

  private void DidFinishWalkout() {
    CurrentLevel++;
    if (CurrentLevel >= levels.Length) {
      Victory();
    } else {
      StartCoroutine(AnimateNextLevel());
    }

    cameraBounds.EnableBounds(true);
    cameraFollows = false;
    actor.UseAutopilot (false);
    actor.controllable = false;
  }

  private IEnumerator AnimateNextLevel() {
    ShowTextBanner(currentLevelData.levelName + " COMPLETED");
    yield return new WaitForSeconds(3.0f);
    SceneManager.LoadScene("Game");
  }

  private void ShowGoIndicator() {
    StartCoroutine(FlickerGoIndicator(4));
  }
  
  private IEnumerator FlickerGoIndicator(int count = 4) {
    while (count > 0) {
      goIndicator.SetActive(true);
      yield return new WaitForSeconds(0.2f);
      goIndicator.SetActive(false);
      yield return new WaitForSeconds(0.2f);
      count--;
    }
  }

  private void ShowBanner(string bannerText, GameObject prefab) {
    GameObject obj = Instantiate(prefab);
    obj.GetComponent<Text>().text = bannerText;
    RectTransform rectTransform = obj.transform as RectTransform;
    rectTransform.SetParent(uiTransform);
    rectTransform.localScale = Vector3.one;
    rectTransform.anchoredPosition = Vector2.zero;
  }
  
  public void GameOver() {
    ShowBanner("GAME OVER", gameOverPrefab);
  }
  
  public void Victory() {
     ShowBanner("YOU WON", gameOverPrefab);
  }
  
  public void ShowTextBanner(string levelName) {
    ShowBanner(levelName, levelNamePrefab);
  }
}