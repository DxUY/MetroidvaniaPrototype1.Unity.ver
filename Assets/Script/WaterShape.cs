using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;

[ExecuteAlways]
public class WaterShape : MonoBehaviour
{
    [Range(0.01f, 0.1f)] public float springStiffness;
    [Range(0.01f, 0.1f)] public float dampering;
    public float spread = 0.006f;
    public GameObject springPrefab;
    [Range(0, 100f)] public int springsNum;
    Spline spline;
    int cornerCount = 2;
    public Transform wavePoints;
    SpriteShapeController spriteShapeController;

    [SerializeField] List<SpringMovement> springs = new();
    private void Awake()
    {
        spriteShapeController = GetComponent<SpriteShapeController>();
        spline = spriteShapeController.spline;
    }

    void OnValidate()
    {
        // Clean waterpoints 
        StartCoroutine(CreateWaves());
    }
    IEnumerator CreateWaves()
    {
        foreach (Transform child in wavePoints.transform)
        {
            StartCoroutine(Destroy(child.gameObject));
        }
        yield return null;
        SetWaves();
        yield return null;
    }
    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
    private void SetWaves()
    {
        int waterPointCount = spline.GetPointCount();

        for(int i = cornerCount; i < waterPointCount - cornerCount; i++)
        {
            spline.RemovePointAt(cornerCount);
        }

        Vector3 waterTopLeftCorner = spline.GetPosition(1);
        Vector3 waterTopRightCorner = spline.GetPosition(2);

        float waterWidth = waterTopRightCorner.x - waterTopLeftCorner.x;
        float spacingPerWave = waterWidth / (springsNum + 1);

        for(int i = springsNum; i > 0; i--)
        {
            int index = cornerCount;

            float xPosition = waterTopLeftCorner.x + (spacingPerWave * i);
            Vector3 wavePoint = new Vector3(xPosition, waterTopLeftCorner.y, waterTopLeftCorner.z);
            spline.InsertPointAt(index, wavePoint);
            spline.SetHeight(index, 0.1f);
            spline.SetCorner(index, false);
            spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        }

        //Create new springs 
        springs = new();

        for (int i = 0; i <= springsNum + 1; i++)
        {
            int index = i + 1;

            Smoothen(spline, index);

            GameObject wavePoint = Instantiate(springPrefab, wavePoints.transform, false); //create a empty game object and use its transform value
            wavePoint.transform.localPosition = spline.GetPosition(index);
            SpringMovement waterSpring = wavePoint.GetComponent<SpringMovement>();
            waterSpring.Init(spriteShapeController);
            springs.Add(waterSpring);
        }
    }

    private void Smoothen(Spline spline, int index)
    {
        Vector3 position = spline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;

        if (index > 1)
            positionPrev = spline.GetPosition(index - 1);

        if (index - 1 <= springsNum)
            positionNext = spline.GetPosition(index + 1);

        Vector3 forward = gameObject.transform.forward;

        float Scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.5f;
        
        Vector3 leftTangent = (positionPrev - position).normalized * Scale;
        Vector3 rightTangent = (positionNext - position).normalized * Scale;
        
        SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, Scale, out rightTangent, out leftTangent);
        
        spline.SetLeftTangent(index, leftTangent);
        spline.SetRightTangent(index, rightTangent);
    }

    private void FixedUpdate()
    {
        foreach (SpringMovement spring in springs)
        {
            spring.WaveSpringUpdate(springStiffness, dampering);
            spring.WavePointUpdate();
        }      

        UpdateSpring();
    }

    private void UpdateSpring()
    {
        int count = springs.Count;

        float[] left_deltas = new float[count];
        float[] right_deltas = new float[count];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    left_deltas[i] = spread * (springs[i].height - springs[i - 1].height);
                    springs[i - 1].velocity += left_deltas[i];
                    springs[i - 1].height += left_deltas[i];
                }

                if (i < count - 1)
                {
                    right_deltas[i] = spread * (springs[i].height - springs[i + 1].height);
                    springs[i + 1].velocity += right_deltas[i];
                    springs[i + 1].height += right_deltas[i];
                }
            }
        }
    }

    public void Splash(int index, float speed)
    {
        if(index >= 0 && index < springs.Count)
            springs[index].velocity += speed;
    }
}
