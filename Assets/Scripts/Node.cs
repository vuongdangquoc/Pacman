﻿using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public LayerMask obstacleLayer;
    public List<Vector2> availableDirections { get; private set; }
    void Start()
    {
        
    }

    private void Update()
    {
        this.availableDirections = new List<Vector2>();
        CheckAvailableDirections(Vector2.up);
        CheckAvailableDirections(Vector2.down);
        CheckAvailableDirections(Vector2.left);
        CheckAvailableDirections(Vector2.right);
    }

    private void CheckAvailableDirections(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, direction, 0.8f, obstacleLayer);

        if (hit.collider == null)
        {
            this.availableDirections.Add(direction);
        } 
    }

    public bool IsInsideNode(Vector2 ghostPosition)
    {
        return Vector2.Distance(transform.position, ghostPosition) < 0.1f; // Kiểm tra xem Ghost đã nằm giữa node chưa
    }

}
