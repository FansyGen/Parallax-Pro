using System.Collections.Generic;
using UnityEngine;

namespace FansyGen.ParallaxPro
{
    [DisallowMultipleComponent]
    public class ParallaxManager : MonoBehaviour
    {
        [Header("General Settings")]
        public Transform target;
        public ParallaxDirection parallaxDirection;

        [Header("Layer Settings")]
        public List<GameObject> layerObjects = new List<GameObject>();
        public bool calculateSpeedAutomatically = true;
        public List<float> layerSpeeds = new List<float>();

        [Header("Transition Settings")]
        public TransitionTimingFunction timingFunction;
        public float transitionDuration = 1f;
        public int steps = 20;
        public AnimationCurve bezierCurve;

        private List<float> startPositions = new List<float>();
        private Vector2 boundSize;
        private Vector2 size;

        private float transitionTimer = 0f;

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError("Target not assigned in the inspector!");
                return;
            }

            if (layerObjects.Count == 0)
            {
                Debug.LogError("No layer objects assigned in the inspector!");
                return;
            }

            size = layerObjects[0].transform.localScale;
            boundSize = GetSpriteBoundsSize(layerObjects[0]);

            CalculateStartPositions();
            CalculateLayerSpeeds();
        }

        private void Update()
        {
            if (layerObjects.Count == 0 || layerSpeeds.Count == 0 || layerObjects.Count != layerSpeeds.Count)
                return;

            UpdateTransitionTimer();
            MoveLayers();
        }

        private void UpdateTransitionTimer()
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer >= transitionDuration)
            {
                transitionTimer = transitionDuration;
            }
        }

        private void MoveLayers()
        {
            float t = CalculateTransition();

            for (int i = 0; i < layerObjects.Count; i++)
            {
                float temp = CalculateTempValue(i, t);
                float distance = CalculateDistance(i, t);

                Vector2 newPosition = parallaxDirection == ParallaxDirection.Horizontal
                    ? new Vector2(startPositions[i] + distance, transform.position.y)
                    : new Vector2(transform.position.x, startPositions[i] + distance);

                layerObjects[i].transform.position = newPosition;

                CheckThresholdsAndAdjustStartPosition(i);
            }
        }

        private void CalculateStartPositions()
        {
            for (int i = 0; i < layerObjects.Count; i++)
            {
                startPositions.Add(GetStartPosition(i));
            }
        }

        private void CalculateLayerSpeeds()
        {
            if (calculateSpeedAutomatically)
            {
                for (int i = 0; i < layerObjects.Count; i++)
                {
                    float layerSpeed = 1f - (0.1f * i);
                    layerSpeeds.Add(layerSpeed);
                }
            }
            else
            {
                if (layerSpeeds.Count != layerObjects.Count)
                {
                    Debug.LogError("Layer Speeds not assigned correctly in the inspector!");
                }
            }
        }

        private Vector2 GetSpriteBoundsSize(GameObject layerObject)
        {
            SpriteRenderer spriteRenderer = layerObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.size;
            }

            Debug.LogWarning("SpriteRenderer component not found on the layer object!");
            return Vector2.zero;
        }

        private float GetStartPosition(int index)
        {
            return parallaxDirection == ParallaxDirection.Horizontal ? target.position.x : target.position.y;
        }

        private void CheckThresholdsAndAdjustStartPosition(int index)
        {
            float temp = CalculateTempValue(index, 1f);

            if (temp > CalculateUpperThreshold(index))
            {
                startPositions[index] += CalculateThresholdOffset(index);
            }
            else if (temp < CalculateLowerThreshold(index))
            {
                startPositions[index] -= CalculateThresholdOffset(index);
            }
        }

        private float CalculateTransition()
        {
            float t = transitionTimer / transitionDuration;

            switch (timingFunction)
            {
                case TransitionTimingFunction.Linear:
                    return t;
                case TransitionTimingFunction.Ease:
                    return Mathf.SmoothStep(0f, 1f, t);
                case TransitionTimingFunction.EaseIn:
                    return Mathf.Pow(t, 2f);
                case TransitionTimingFunction.EaseOut:
                    return 1f - Mathf.Pow(1f - t, 2f);
                case TransitionTimingFunction.EaseInOut:
                    return t < 0.5f ? 2f * Mathf.Pow(t, 2f) : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case TransitionTimingFunction.Steps:
                    return Mathf.Floor(t * steps) / steps;
                case TransitionTimingFunction.CubicBezier:
                    return bezierCurve.Evaluate(t);
                default:
                    return t;
            }
        }

        private float CalculateTempValue(int index, float t)
        {
            return parallaxDirection == ParallaxDirection.Horizontal
                ? Mathf.Lerp(GetStartPosition(index), target.position.x * (1f - layerSpeeds[index]), t)
                : Mathf.Lerp(GetStartPosition(index), target.position.y * (1f - layerSpeeds[index]), t);
        }

        private float CalculateDistance(int index, float t)
        {
            return parallaxDirection == ParallaxDirection.Horizontal
                ? Mathf.Lerp(0f, target.position.x * layerSpeeds[index], t)
                : Mathf.Lerp(0f, target.position.y * layerSpeeds[index], t);
        }

        private float CalculateUpperThreshold(int index)
        {
            return parallaxDirection == ParallaxDirection.Horizontal
                ? startPositions[index] + boundSize.x * size.x
                : startPositions[index] + boundSize.y * size.y;
        }

        private float CalculateLowerThreshold(int index)
        {
            return parallaxDirection == ParallaxDirection.Horizontal
                ? startPositions[index] - boundSize.x * size.x
                : startPositions[index] - boundSize.y * size.y;
        }

        private float CalculateThresholdOffset(int index)
        {
            return parallaxDirection == ParallaxDirection.Horizontal
                ? boundSize.x * size.x
                : boundSize.y * size.y;
        }
    }
}