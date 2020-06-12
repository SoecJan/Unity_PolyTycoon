using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;
    public FitType fitType;

    public bool fitX;
    public bool fitY;

    public override float minWidth => float.IsNaN(rows * cellSize.x) ? base.minWidth : rows * cellSize.x;
    public override float preferredWidth => float.IsNaN(rows * cellSize.x) ? base.minWidth : (rows * cellSize.x) + (spacing.x * (rows+1)) + (rows * (padding.left + padding.right));
    public override float minHeight => float.IsNaN(columns * cellSize.y) ? base.minHeight : rows * cellSize.y;
    public override float preferredHeight => true ? base.preferredHeight : columns * cellSize.y;

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (transform.childCount == 0) return;
        
        // float parentWidth = ((RectTransform)rectTransform.GetChild(0).transform).rect.width * columns;
        float childWidth = ((RectTransform) rectTransform.GetChild(0).transform).rect.width;
        float childHeight = ((RectTransform) rectTransform.GetChild(0).transform).rect.height;
        
        float parentWidth = ((RectTransform) transform.parent).rect.width;
        float parentHeight = ((RectTransform) transform.parent).rect.height;

        switch (fitType)
        {
            case FitType.Uniform:
                fitX = true;
                fitY = true;
                float sqrRt = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
                break;
            case FitType.Width:
                columns = Mathf.CeilToInt(parentWidth / childWidth);
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
            case FitType.Height:
                rows = Mathf.CeilToInt(parentHeight / childHeight);
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float) columns);
        }
        
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float) rows);
        }

        rows = rows < 1 ? 1 : rows;
        columns = columns < 1 ? 1 : columns;

        // float sqrRt = Mathf.Sqrt(transform.childCount);
        // rows = Mathf.CeilToInt(sqrRt);
        // columns = Mathf.CeilToInt(sqrRt);

        

        // Debug.Log(parentHeight);
        
        float cellWidth = (parentWidth / (float) columns) - ((spacing.x / (float) columns) *  (columns - 1)) -
                          (padding.left / (float) columns) - (padding.right / (float) columns);
        float cellHeight = (parentHeight / (float) rows) - ((spacing.y / (float) rows) * (rows - 1)) -
                           (padding.top / (float) rows) - (padding.bottom / (float) rows);

        
        // cellSize.x = cellWidth;
        // cellSize.y = cellHeight;
        cellSize.x = cellWidth > ((RectTransform) rectTransform.GetChild(0).transform).rect.width ? ((RectTransform) rectTransform.GetChild(0).transform).rect.width : cellWidth;
        cellSize.y = cellHeight > ((RectTransform) rectTransform.GetChild(0).transform).rect.height ? ((RectTransform) rectTransform.GetChild(0).transform).rect.height : cellHeight;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            int rowCount = i / columns;
            int columnCount = i % columns;

            RectTransform item = rectChildren[i];

            float xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            float yPos = (cellSize.y * rowCount) + (spacing.y * columnCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}