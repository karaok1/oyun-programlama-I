﻿using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Grid grid;
    public Transform bubblesArea;
    public List<GameObject> bubblesPrefabs;
    public List<GameObject> bubblesInScene;
    public List<string> colorsInScene;

    public float offset = 1f;
    public GameObject leftLine;
    public GameObject rightLine;
    private bool lastLineIsLeft = true;


    private void Start()
    {
        grid = GetComponent<Grid>();
    }

    // Levelin oluşturulduğu yer
    public void GenerateLevel()
    {
        FillWithBubbles(GameObject.FindGameObjectWithTag("InitialLevelScene"), bubblesPrefabs);
        SnapChildrensToGrid(bubblesArea);
        UpdateListOfBubblesInScene();
    }

    private void FillWithBubbles(GameObject go, List<GameObject> bubbles)
    {
        foreach (Transform t in go.transform)
        {
            var bubble = Instantiate(bubbles[(int) (Random.Range(0, bubbles.Count * 1000000f) / 1000000f)],
                bubblesArea);
            bubble.transform.position = t.position;
        }

        Destroy(go);
    }

    public void UpdateListOfBubblesInScene()
    {
        var colors = new List<string>();
        var newListOfBubbles = new List<GameObject>();

        foreach (Transform t in bubblesArea)
        {
            var bubbleScript = t.GetComponent<Bubble>();
            if (colors.Count < bubblesPrefabs.Count && !colors.Contains(bubbleScript.bubbleColor.ToString()))
            {
                var color = bubbleScript.bubbleColor.ToString();
                colors.Add(color);

                foreach (var prefab in bubblesPrefabs)
                    if (color.Equals(prefab.GetComponent<Bubble>().bubbleColor.ToString()))
                        newListOfBubbles.Add(prefab);
            }
        }

        colorsInScene = colors;
        bubblesInScene = newListOfBubbles;
    }

    // En yakın gözeneğe atama yap
    public void SetAsBubbleAreaChild(Transform bubble)
    {
        SnapToNearestGripPosition(bubble);
        bubble.SetParent(bubblesArea);
    }

    #region Singleton

    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    #endregion

    #region Snap to Grid

    private void SnapChildrensToGrid(Transform parent)
    {
        foreach (Transform t in parent) SnapToNearestGripPosition(t);
    }

    public void SnapToNearestGripPosition(Transform t)
    {
        var cellPosition = grid.WorldToCell(t.position);
        t.position = grid.GetCellCenterWorld(cellPosition);
    }

    #endregion

    #region Add new line

    [ContextMenu("AddLine")]
    public void AddNewLine()
    {
        OffsetGrid();
        OffsetBubblesInScene();
        var newLine = lastLineIsLeft ? Instantiate(rightLine) : Instantiate(leftLine);
        FillWithBubbles(newLine, bubblesInScene);
        SnapChildrensToGrid(bubblesArea);
        lastLineIsLeft = !lastLineIsLeft;
    }

    private void OffsetGrid()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - offset);
    }

    private void OffsetBubblesInScene()
    {
        foreach (Transform t in bubblesArea) t.transform.position = new Vector2(t.position.x, t.position.y - offset);
    }

    #endregion
}