    using CodeMonkey;
    using UnityEngine;

    public class RadarPing : MonoBehaviour {

        private SRC_Radar SRC;
        private SpriteRenderer spriteRenderer;
        private float disappearTimer;
        private float disappearTimerMax;
        private Color color;
        private Vector3 baseScale; 

        //new system
        private float pingAngle;
        

        private void Awake() {
            SRC = FindAnyObjectByType<SRC_Radar>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            disappearTimerMax = 1f;
            disappearTimer = 0f;

            color = new Color(1, 1, 1, 1f);
            baseScale = transform.localScale;
        }
        private void Start() {
            CMDebug.TextPopup("포착된 각도: " + GetAngle(), transform.position);
        }

        private void Update() {
            disappearTimer += Time.deltaTime;
            transform.localScale = baseScale * SRC.size;

            color.a = Mathf.Lerp(disappearTimerMax, 0f, disappearTimer / disappearTimerMax);
            spriteRenderer.color = color;

            if (disappearTimer >= disappearTimerMax) 
            {

                SRC.SignalList.Remove(this);
                Destroy(gameObject);   
            }
        }

        public void SetColor(Color color) {
            this.color = color;
        }
        public void SetSize(float i)
        {
            transform.localScale = transform.localScale * i;
        }
        public void SetDisappearTimer(float disappearTimerMax) {
            this.disappearTimerMax = disappearTimerMax;
            disappearTimer = 0f;
        }
        public void SetAngle(float angle) 
        {
            pingAngle = angle;
        }
        public float GetAngle() 
        {
            return pingAngle;
        }
        

    }
