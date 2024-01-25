using DG.Tweening;
using UnityEngine;

namespace UnityFramework.UI
{
    public class UIAnimationRect : MonoBehaviour
    {
        public float timeAnimScale = 0.3f;
        public float timeDelayScale = 0.05f;

        public float timeAnimRight = 0.3f;
        public float timeDelayRight = 0.05f;
        public float timeDelayRightNext = 0f;

        public float timeAnimLeft = 0.3f;
        public float timeDelayLeft = 0.05f;
        public float timeDelayLeftNext = 0f;

        public float timeAnimTop = 0.3f;
        public float timeDelayTop = 0.05f;
        public float timeDelayTopNext = 0f;

        public float timeAnimBot = 0.3f;
        public float timeDelayBot = 0.05f;
        public float timeDelayBotNext = 0f;

        public RectTransform[] rectAnimScale;
        public RectTransform[] rectAnimRight;
        public RectTransform[] rectAnimLeft;
        public RectTransform[] rectAnimTop;
        public RectTransform[] rectAnimBot;

        Vector3 scaleStart = new Vector3(0.0f, 0.0f, 0.0f);

        private void OnEnable()
        {
            AnimScaleIn();
            AnimRightIn();
            AnimLeftIn();
            AnimTopIn();
            AnimBotIn();
        }

        void AnimScaleIn()
        {
            for (int i = 0; i < rectAnimScale.Length; i++)
            {
                if (rectAnimScale[i] == null) continue;
                rectAnimScale[i].localScale = scaleStart;
                rectAnimScale[i].DOScale(Vector3.one, timeAnimScale).SetEase(Ease.OutBack).SetDelay(timeDelayScale + timeDelayScale * i);
            }
        }

        void AnimRightIn()
        {
            for (int i = 0; i < rectAnimRight.Length; i++)
            {
                if (rectAnimRight[i] == null) continue;
                Vector2 vector2 = rectAnimRight[i].anchoredPosition;
                rectAnimRight[i].anchoredPosition = new Vector2(vector2.x + 1000, vector2.y);
                rectAnimRight[i].DOAnchorPosX(vector2.x, timeAnimRight).SetEase(Ease.OutCubic).SetDelay(timeDelayRight + timeDelayRightNext * i);
            }
        }

        void AnimLeftIn()
        {
            for (int i = 0; i < rectAnimLeft.Length; i++)
            {
                if (rectAnimLeft[i] == null) continue;
                Vector2 vector2 = rectAnimLeft[i].anchoredPosition;
                rectAnimLeft[i].anchoredPosition = new Vector2(vector2.x - 1000, vector2.y);
                rectAnimLeft[i].DOAnchorPosX(vector2.x, timeAnimLeft).SetEase(Ease.OutCubic).SetDelay(timeDelayLeft + timeDelayLeftNext * i);
            }
        }

        void AnimTopIn()
        {
            for (int i = 0; i < rectAnimTop.Length; i++)
            {
                if (rectAnimTop[i] == null) continue;
                Vector2 vector2 = rectAnimTop[i].anchoredPosition;
                rectAnimTop[i].anchoredPosition = new Vector2(vector2.x, vector2.y + 1000);
                rectAnimTop[i].DOAnchorPosY(vector2.y, timeAnimTop).SetEase(Ease.OutCubic).SetDelay(timeDelayTop + timeDelayTopNext * i);
            }
        }

        void AnimBotIn()
        {
            for (int i = 0; i < rectAnimBot.Length; i++)
            {
                if (rectAnimBot[i] == null) continue;
                Vector2 vector2 = rectAnimBot[i].anchoredPosition;
                rectAnimBot[i].anchoredPosition = new Vector2(vector2.x, vector2.y - 1000);
                rectAnimBot[i].DOAnchorPosY(vector2.y, timeAnimBot).SetEase(Ease.OutCubic).SetDelay(timeDelayBot + timeDelayBotNext * i);
            }
        }

        public void AnimLeftOut()
        {
            for (int i = 0; i < rectAnimRight.Length; i++)
            {
                if (rectAnimRight[i] == null) continue;
                Vector2 vector2 = rectAnimRight[i].anchoredPosition;
                rectAnimRight[i].anchoredPosition = new Vector2(vector2.x, vector2.y);
                rectAnimRight[i].DOAnchorPosX(vector2.x - 1500, timeAnimRight).SetEase(Ease.OutCubic).SetDelay(timeDelayRight + timeDelayRightNext * i * 2);
            }
        }
    }
}