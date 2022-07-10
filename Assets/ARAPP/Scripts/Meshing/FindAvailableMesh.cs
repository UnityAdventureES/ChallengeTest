using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAvailableMesh
{
    public FindAvailableMesh(float scanPerPerimeter)
    {
        this.scanPerPerimeter = scanPerPerimeter;
    }

    public enum ScanLevel
    {
        High,
        Medium,
        Low
    }

    private float scanPerPerimeter;

    public List<Vector2> scanPoints = new List<Vector2>();

    float oldValueX = 0;
    float oldValueY = 0;

    public ScanLevel ScanLevelMesh
    {
        set
        {
            SelectScanLevel(value);
        }
    }

    private void SelectScanLevel(ScanLevel scanLevel)
    {
        switch (scanLevel)
        {
            case ScanLevel.High:
                SetScanArea(150);
                break;
            case ScanLevel.Medium:
                SetScanArea(200);
                break;
            case ScanLevel.Low:
                SetScanArea(300);
                break;
            default:
                break;
        }
    }

    private void SetScanArea(float scanValue)
    {
        oldValueX = scanPerPerimeter + scanValue;
        oldValueY = scanPerPerimeter + scanValue;
        GenerateScanPoint(scanValue);
    }

    public void GenerateScanPoint(float scanValue)
    {
        var xPoints = 2 * scanPerPerimeter / scanValue;
        var yPoints = 2 * scanPerPerimeter / scanValue;

        for (int i = 0; i < xPoints; i++)
        {
            var xValue = oldValueX - scanValue;
            oldValueX = xValue;

            for (int j = 0; j <= yPoints; j++)
            {
                var yValue = oldValueY - scanValue;
                oldValueY = yValue;
                scanPoints.Add(new Vector2(xValue, yValue));
            }
            oldValueY = scanPerPerimeter + scanValue;
        }
    }
}
