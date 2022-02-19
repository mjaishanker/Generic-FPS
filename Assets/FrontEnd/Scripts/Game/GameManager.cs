using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    //public GameObject[] enemiesOnScene;

    [SerializeField] HashSet<GameObject> enemies = new HashSet<GameObject>();
    [SerializeField] TextMeshProUGUI enemiesDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (enemiesDisplay != null)
        {
            enemiesDisplay.SetText(enemies.Count.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Enemies: " + enemies.Count);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void AddEnemyToList(GameObject enemy)
    {
        enemies.Add(enemy);
        if (enemiesDisplay != null)
        {
            enemiesDisplay.SetText(enemies.Count.ToString());
        }
    }

    public void RemoveEnemyToList(GameObject enemy)
    {
        enemies.Remove(enemy);
        if (enemiesDisplay != null)
        {
            enemiesDisplay.SetText(enemies.Count.ToString());
        }
    }

}
