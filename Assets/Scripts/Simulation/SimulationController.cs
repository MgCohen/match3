using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [SerializeField] private BoardViewController view;
    [SerializeField] private BoardController controller;
    [SerializeField] private BoardAnimationController animations;
    [SerializeField] private BoardConfig config;

    [SerializeField] private TMPro.TMP_InputField input;

    private bool ready = true;
    private bool simulating = false;

    private void Start()
    {
        animations.SequenceStarted += () => ready = false;
        animations.SequenceEnded += () => ready = true;
    }

    public void SimulateOneMove()
    {
        float x = Random.Range(0, (int)config.BoardSize.x);
        float y = Random.Range(0, (int)config.BoardSize.y);
        Vector2 index = new Vector2(x, y);
        Vector2 dir;
        do
        {
            dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).GetUnidirectional01();
        } 
        while (!IsInsideLimits(index + dir));

        Board board = controller.GetCurrentBoard();
        TileData targetTile = board.GetTile(index);
        Debug.Log(targetTile);
        Debug.Log(dir);
        BoardEvents.TrySwapTile(targetTile, dir);
    }

    private bool IsInsideLimits(Vector2 targetIndex)
    {
        return targetIndex.x >= 0 && targetIndex.x < config.BoardSize.x && targetIndex.y >= 0 && targetIndex.y < config.BoardSize.y;
    }

    public void StartSimulation()
    {
        if (simulating)
        {
            return;
        }
        simulating = true;
        StartCoroutine(Simulation());
    }

    public void StopSimulation()
    {
        simulating = false;
    }

    private IEnumerator Simulation()
    {
        while (simulating)
        {
            if (ready)
            {
                SimulateOneMove();
            }
            yield return null;
        }
    }

    public void SimulateX()
    {
        int amount = int.Parse(input.text);
        view.ToggleView(false);
        for (int i = 0; i < amount; i++)
        {
            SimulateOneMove();
        }
        view.ToggleView(true);
        BoardEvents.SetBoard(controller.GetCurrentBoard());
    }
}
